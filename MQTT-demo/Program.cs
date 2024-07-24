using MQTTnet.Client;
using MQTTnet;
using MQTT_demo.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddMvc();
builder.Services.AddWebOptimizer(pipeline =>
{
    pipeline.MinifyJsFiles("js/a.js", "js/b.js", "js/c.js");
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllersWithViews();
builder.Services.Configure<MqttSettings>(builder.Configuration.GetSection("MqttSettings"));
builder.Services.AddSingleton<IMqttClient>(sp =>
{
    var mqttSettings = sp.GetRequiredService<IOptions<MqttSettings>>().Value;
    var logger = sp.GetRequiredService<ILogger<IMqttClient>>();
    var factory = new MqttFactory();
    var mqttClient = factory.CreateMqttClient();
    var options = new MqttClientOptionsBuilder()
        .WithClientId(mqttSettings.ClientId)
        .WithWebSocketServer(mqttSettings.Server)
        //.WithCredentials(mqttSettings.Username, mqttSettings.Password)
        .WithCleanSession()
        .Build();

    mqttClient.ConnectedAsync += async e => logger.LogInformation("Connected to MQTT broker via WebSocket.");
    mqttClient.DisconnectedAsync += async e => logger.LogInformation("Disconnected from MQTT broker.");

    try
    {
        mqttClient.ConnectAsync(options, System.Threading.CancellationToken.None).Wait();
    }
    catch (Exception ex)
    {
        logger.LogError($"Error connecting to MQTT broker: {ex.Message}");
    }

    return mqttClient;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();
app.Run();
