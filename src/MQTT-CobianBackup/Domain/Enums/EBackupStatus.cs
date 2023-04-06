using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Testes.Domain.Enums
{
    public enum EBackupStatus
    {
        Running = 0,
        Ok = 1,
        Error = 2,
        Outdated = 3
    }
}
