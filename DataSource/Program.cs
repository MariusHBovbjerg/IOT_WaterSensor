using System.Text;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

namespace DataSource;

class Program
{
    
    private static Dictionary<Guid, int> _guids = new() {
        {Guid.Parse("0ddc7bef-865f-42ba-85b6-1de79b3df96e"), 70},
        {Guid.Parse("0ddc7bef-865f-42ba-85b6-1de78b3df96f"), 50},
        {Guid.Parse("2a7f264d-b158-4d10-b6a9-cc9b4dd8b725"), 30}};
    
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

        for (;;)
        {
            foreach (var guid in _guids)
            { 
                var newMoisture = Math.Clamp(guid.Value + random.Next(-1, 2), 0, 100);
                
                _guids[guid.Key] = newMoisture;
                
                var json = JsonConvert.SerializeObject(new { clientid = guid.Key, timestamp = DateTime.Now, Moisture = newMoisture});

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic("water")
                    .WithPayload(json)
                    .Build();
                    
                await mqttClient.PublishAsync(message, CancellationToken.None);
            }

            Console.WriteLine("MQTT application message is published.");
            Thread.Sleep(20000);
        }
    }
    
    private static async Task ConsumePublishedMessage(MqttApplicationMessageReceivedEventArgs e)
    {
        var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
        
        if (!_guids.ContainsKey(Guid.Parse(e.ApplicationMessage.Topic))) return;
        
        Console.WriteLine("Binding key is: " + message);
    }
}