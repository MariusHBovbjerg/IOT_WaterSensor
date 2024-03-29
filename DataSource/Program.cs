﻿using System.Text;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

namespace DataSource;

class Program
{

    private static Dictionary<Guid, int> _guids = new() {
        {Guid.Parse("fcab229a-4fe4-4fa1-9f1b-4ce9304a76b6"), 70},
        {Guid.Parse("d617cb1a-0705-451e-a2b8-b8997d3c6820"), 50},
        {Guid.Parse("61d11693-5592-4455-b204-5d4504037fcc"), 30}};

    public static async Task Main(string[] args)
    {
        var mqttFactory = new MqttFactory();
        var random = new Random();

        using var mqttClient = mqttFactory.CreateMqttClient();

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(Environment.GetEnvironmentVariable("MQTT_Broker") ?? "[::1]")
            .Build();

        mqttClient.ApplicationMessageReceivedAsync += ConsumePublishedMessage;

        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .Build();

        foreach (var topic in _guids.Select(guid => new MqttTopicFilterBuilder().WithTopic(guid.Key.ToString())).Select(topicBuilder => topicBuilder.Build()))
        {
            mqttSubscribeOptions.TopicFilters.Add(topic);
        }

        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

        foreach (var guid in _guids)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("bind")
                .WithPayload(guid.Key.ToString())
                .Build();
            await mqttClient.PublishAsync(message, CancellationToken.None);
        }

        for (; ; )
        {
            foreach (var guid in _guids)
            {
                var newMoisture = Math.Clamp(guid.Value + random.Next(-1, 2), 0, 100);

                _guids[guid.Key] = newMoisture;

                var json = JsonConvert.SerializeObject(new { clientid = guid.Key, timestamp = DateTime.Now, Moisture = newMoisture });

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic("water")
                    .WithPayload(json)
                    .Build();

                await mqttClient.PublishAsync(message, CancellationToken.None);
            }

            Console.WriteLine("MQTT application message is published.");
            Thread.Sleep(1000);
        }
    }

    private static async Task ConsumePublishedMessage(MqttApplicationMessageReceivedEventArgs e)
    {
        var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

        if (!_guids.ContainsKey(Guid.Parse(e.ApplicationMessage.Topic))) return;

        Console.WriteLine("Device " + e.ApplicationMessage.Topic + ": Binding key is: " + message + " - WARNING THESE CODES RETURN ONLY ONCE FROM BACKEND");
    }
}