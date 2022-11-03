using System.Text;
using IOT_WateringSensor.Data;
using IOT_WateringSensor.Database;
using MQTTnet;
using MQTTnet.Server;
using Newtonsoft.Json;

namespace IOT_WateringSensor.MQTT_Gøj;

public class MqttHandler
{
    private static WaterSensorDbContext _context;

    public MqttHandler(WaterSensorDbContext dbContext)
    {
        _context = dbContext;
        _context.Database.EnsureCreated();
    }
    
    public async Task Run_Server_With_Logging()
    {
        /*
         * This sample starts a simple MQTT server and prints the logs to the output.
         *
         * IMPORTANT! Do not enable logging in live environment. It will decrease performance.
         *
         * See sample "Run_Minimal_Server" for more details.
         */

        var mqttFactory = new MqttFactory();

        var mqttServerOptions = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .Build();

        using var mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);

        mqttServer.InterceptingPublishAsync += 
            async e => await ConsumePublishedMessage(e);

        await mqttServer.StartAsync();
        
        var manualResetEvent = new ManualResetEvent(false);
        manualResetEvent.WaitOne();

        // Stop and dispose the MQTT server if it is no longer needed!
        await mqttServer.StopAsync();
    }

    private async Task ConsumePublishedMessage(InterceptingPublishEventArgs e)
    {
        
        if(!e.ApplicationMessage.Topic.Contains("water"))
            return;
            
        var message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

        var payload = JsonConvert
            .DeserializeObject<SensorDataDto>(message);
            
        Console.WriteLine("Intercepted publish: " + payload.ClientId + " " + payload.TimeStamp + " " + payload.Moisture);

        await _context.SensorData.AddAsync(new SensorData
        {
            ClientId = payload.ClientId,
            TimeStamp = payload.TimeStamp,
            Moisture = payload.Moisture
        });
        
        await _context.SaveChangesAsync();
    }
}