using MQTT_Testes;
using MQTT_Testes.Services;
using MR.Log;
using MR.MQTT.Service;

IHost host = Host.CreateDefaultBuilder(args)
    .MRConfigureLogService()
    .UseWindowsService(options =>
    {
        options.ServiceName = "MQTT Cobian Backup";
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<MQTTService>();
        services.AddSingleton<CobianLogService>();
        services.AddHostedService<Worker>();
    }).Build();

MRLog.ConfigureLogMain();

try
{
    await host.RunAsync();
}
finally
{
    MRLog.CloseAndFlush();
}
