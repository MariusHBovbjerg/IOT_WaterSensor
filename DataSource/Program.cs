using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;

namespace DataSource;

class Program
{
    
    private static List<Guid> _guids = new List<Guid> {Guid.Parse("0ddc7bef-865f-42ba-85b6-1de78b3df96f"),
        Guid.Parse("b9d8a995-3328-4bc2-abeb-7c1106b1c172"),
        Guid.Parse("2a7f264d-b158-4d10-b6a9-cc9b5dd8b726"),
        Guid.Parse("91f0fd13-d2e7-42e0-92d4-cf4cfe8780ee"),
        Guid.Parse("9004a44b-ced0-400c-8890-ae8d090b499b"),
        Guid.Parse("bcf74c51-427f-4135-baf1-58a7bcbd9b17"),
        Guid.Parse("3da962cc-0e4c-4775-96f8-3b4a948b3e8e"),
        Guid.Parse("702d1d15-cb35-4dcf-8a58-7274c4480f06"),
        Guid.Parse("259dfd85-3766-467a-9594-9432b6d8bccf"),
        Guid.Parse("f49b09c3-6640-434c-974d-1182e21bb8f5")};
    
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

            var json = JsonConvert.SerializeObject(new
                { clientid = _guids[random.Next(0,9)], timestamp = DateTime.Now, Moisture = random.Next(0, 100) });

            var message = new MqttApplicationMessageBuilder()
                .WithTopic("water")
                .WithPayload(json)
                .Build();

            await mqttClient.PublishAsync(message, CancellationToken.None);

            await mqttClient.DisconnectAsync();

            Console.WriteLine("MQTT application message is published.");
            Thread.Sleep(1000);
        }
    }
}