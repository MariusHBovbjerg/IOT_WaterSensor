using IOT_WateringSensor;
using IOT_WateringSensor.Database;
using IOT_WateringSensor.MQTT_Client;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.ConfigureServices();

new Thread(async _ =>
{
    var serviceCollection = builder.Services.BuildServiceProvider();
    var handler = new MqttHandler(serviceCollection.GetRequiredService<WaterSensorDbContext>());
    await handler.Run_Server_With_Logging();
}).Start();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();