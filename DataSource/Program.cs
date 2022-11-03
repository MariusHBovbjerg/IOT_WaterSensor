using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

namespace DataSource;

class Program
{
    public static async Task Main(string[] args)
    {
        var mqttFactory = new MqttFactory();

        using (var mqttClient = mqttFactory.CreateMqttClient())
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(Environment.GetEnvironmentVariable("MQTT_Broker") ?? "localhost", 1883)
                .Build();

            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            var json = JsonConvert.SerializeObject(new { clientid = Guid.NewGuid(), timestamp = DateTime.Now });

            var message = new MqttApplicationMessageBuilder()
                .WithTopic("water")
                .WithPayload(json)
                .Build();

            await mqttClient.PublishAsync(message, CancellationToken.None);

            await mqttClient.DisconnectAsync();

            Console.WriteLine("MQTT application message is published.");
        }
    }
}