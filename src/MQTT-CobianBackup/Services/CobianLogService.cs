using MQTT_Testes.Domain;
using MQTT_Testes.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MQTT_Testes.Services
{
    public class CobianLogService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<CobianLogService> _logger;

        public CobianLogService(IConfiguration config, ILogger<CobianLogService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public List<BackupSetInfo> ListAllBackups()
        {
            // LISTA TODOS OS CONJUNTOS DE BACKUP DA PASTA CONFIGURADA

            var result = new List<BackupSetInfo>();
            var path = _config.GetValue<string>("Folders:CobianLog");
            if (!Directory.Exists(path))
            {
                _logger.LogWarning("Diretório {path} não existe.", path);
            }
            else
            {
                var files = new DirectoryInfo(path)
                    .GetFiles("Cobian Reflector ????-??-??.txt", new EnumerationOptions() { RecurseSubdirectories = false })
                    .OrderByDescending(x => x.Name);

                var arquivosLidos = 0;
                foreach (var file in files)
                {
                    // PARA CADA ARQUIVO DE LOG, PROCURA TODOS OS CONJUNTOS DE BACKUP

                    var sets = GetBackupSets(file.FullName);
                    foreach (var backup in sets)
                    {
                        // SE O SET DE BACKUP JÁ EXISTE, NÃO PRECISA INCLUIR OUTRO, PQ VALE SOMENTE O ÚLTIMO
                        if (!result.Any(x => x.Time == backup.Time)) {
                            result.Add(backup);
                            Console.WriteLine($"{backup.FileName}, {backup.Time}, {backup.Started.ToString()}, {backup.Ended.ToString()}, {backup.Status.ToString()}");
                        }
                    }

                    arquivosLidos++;
                    if (arquivosLidos == 2)
                    {
                        // SOMENTE DOIS ARQUIVOS SÃO SUFICIENTES PARA O CONJUNTO DE BACKUPS
                        break;
                    }
                }
            }

            return result;
        }

        private List<BackupSetInfo> GetBackupSets(string fileName)
        {

            var logFile = "";
            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using var reader = new StreamReader(fileStream);
                logFile = reader.ReadToEnd();
            }

            var result = new List<BackupSetInfo>();
            var lines =  logFile.Split("\r\n"); // File.ReadLines(fileName);
            foreach (var line in lines)
            {
                var msg = line.PadRight(24)[24..];

                if (msg == "A backup has started...")
                {
                    result.Add(new()
                    {
                        Started = DateTime.Parse(line.Substring(4, 19)),
                        FileName = Path.GetFileName(fileName),
                        Time = line.Substring(15, 5),
                        Status = EBackupStatus.Running
                    });
                
                }
                else if (msg == "The backup has ended without errors." && result.Count > 0 && result.Last().Status == EBackupStatus.Running)
                {
                    result.Last().Status = EBackupStatus.Ok;
                    result.Last().Ended = DateTime.Parse(line.Substring(4, 19));
                
                }
                else if (msg == "The backup has ended. There are errors. Consult the log file." && result.Count > 0 && result.Last().Status == EBackupStatus.Running)
                {
                    result.Last().Status = EBackupStatus.Error;
                    result.Last().Ended = DateTime.Parse(line.Substring(4, 19));
                
                }
            }

            return result;
        }

    }
}
