using MQTTnet.Client;
using MQTTnet;
using System.Text.Json;
using MQTT_Testes.Domain;
using MQTT_Testes.Services;
using MR.MQTT.Service;
using MR.MQTT.Domain;
using MQTT_Testes.Domain.Enums;

namespace MQTT_Testes
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _config;
        private readonly CobianLogService _cobianService;
        private readonly MQTTService _mqtt;
        private List<Device> _devices = new();

        public Worker(ILogger<Worker> logger, IConfiguration config, CobianLogService cobianService, MQTTService mqtt)
        {
            _logger = logger;
            _config = config;
            _cobianService = cobianService;
            _mqtt = mqtt;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            var _delay = _config.GetValue<int>("Service:Delay", 30000);

            while (!stoppingToken.IsCancellationRequested)
            {

                var sets = _cobianService.ListAllBackups();
                foreach (var set in sets)
                {
                    var device = GetDevice(Environment.MachineName + "_backup_set_" + set.Time, Environment.MachineName.ToUpper() + " CobianBackup set " + set.Time);
                    device.Attributes = set;

                    await _mqtt.Register(device);
                    await _mqtt.SendState(device, set.Status.ToString());
                }

                await Task.Delay(_delay, stoppingToken);
            }
        }

        private Device GetDevice(string id, string? name = null)
        {
            var device = _devices.FirstOrDefault(x => x.Id == id);
            if (device == null)
            {
                device = new Device(id, name);
                _devices.Add(device);
            }
            return device;

        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await UnregisterDevices();
        }

        private async Task UnregisterDevices()
        {
            foreach (var device in _devices)
            {
                await _mqtt.UnRegister(device);
            }
        }
    }
}