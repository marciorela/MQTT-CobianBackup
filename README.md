# MQTT-MQTT-CobianBackup
Serviço do windows que avisa ao Home Assistant através do MQTT, a situação dos backups feitos pelo CobianBackup.
O serviço verifica os últimos 2 arquivos da pasta de log.
Cada conjunto de backups é criado como uma entidade de domínio sensor dentro do Home Assistant.

## Configuração
appsettings.json:

```json
{
  "Folders": {
    "CobianLog": "<pasta onde estão os logs do CobianBackup>",
  },
  "Service": {
    "Delay": "<tempo de espera entre as chamadas (default 30000)>"
  }
  "MQTT": {
    "Server": "<IP do servidor MQTT>",
    "Username": "<Usuario do MQTT>",
    "Password": "<Senha do MQTT>"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  }
}
```
