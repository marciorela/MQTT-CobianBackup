using MQTT_Testes.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MQTT_Testes.Domain
{
    public class BackupSetInfo
    {
        private EBackupStatus _status;

        //public DateTime Agora { get; set; } = DateTime.Now;

        public string FileName { get; set; } = "";
        public DateTime Started { get; set; }
        public DateTime? Ended { get; set; } = null;

        [JsonIgnore]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EBackupStatus Status
        {
            get
            {
                if ((DateTime.Now - Started).TotalHours > 25)
                {
                    return EBackupStatus.Outdated;
                }
                else
                {
                    return _status;
                }
            }
            set
            {
                _status = value;
            }
        }

        [JsonIgnore]
        public string Time { get; set; } = "";
    }
}
