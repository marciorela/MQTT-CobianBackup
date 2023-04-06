using MR.MQTT.Domain;

namespace MQTT_Testes.Domain
{
    public class Device : DeviceBase
    {

        public Device(string id, string? name = null) : base(id, name)
        {
            Topic = "homeassistant/sensor/cobianbackup/";
            Device_class = null;
        }
    }
}
