using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

namespace DataSource;

class Program
{
    
    private static Dictionary<Guid, int> _guids = new Dictionary<Guid, int> {
        {Guid.Parse("0ddc7bef-865f-42ba-85b6-1de78b3df96f"), 9},
        {Guid.Parse("b9d8a995-3328-4bc2-abeb-7c1106b1c172"), 3},
        {Guid.Parse("2a7f264d-b158-4d10-b6a9-cc9b5dd8b726"), 1}};
    
    public static async Task Main(string[] args)
    {
        var mqttFactory = new MqttFactory();
        var random = new Random();

        for (;;)
        {
            using var mqttClient = mqttFactory.CreateMqttClient();
            
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(Environment.GetEnvironmentVariable("MQTT_Broker") ?? "localhost", 1883)
                .Build();

            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            foreach (var guid in _guids)
            { 
                var newMoisture = (int) Math.Clamp(Math.Round(guid.Value + ((double)random.Next(-100, 100) / 100)), 0, 100);
                
                _guids[guid.Key] = newMoisture;
                
                var json = JsonConvert.SerializeObject(new { clientid = guid.Key, timestamp = DateTime.Now, Moisture = newMoisture});

                var message = new MqttApplicationMessageBuilder()
                    .WithTopic("water")
                    .WithPayload(json)
                    .Build();
                    
                await mqttClient.PublishAsync(message, CancellationToken.None);
            }
            await mqttClient.DisconnectAsync();

            Console.WriteLine("MQTT application message is published.");
            Thread.Sleep(1000);
        }
    }
}