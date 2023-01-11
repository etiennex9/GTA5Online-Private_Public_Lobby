using GTA5_Private_Public_Lobby.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace GTA5_Private_Public_Lobby.Services.Implementation
{
    public class SettingsService : ISettingsService
    {
        private readonly IIpHelperService _ipHelperService;
        private readonly ILogService _logService;

        private readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        public SettingsService(IIpHelperService ipHelperService, ILogService logService)
        {
            _ipHelperService = ipHelperService;
            _logService = logService;
        }

        public Settings Load()
        {
            if (File.Exists(_path))
            {
                try
                {
                    var json = File.ReadAllText(_path);
                    var settings = JsonSerializer.Deserialize<Settings>(json);

                    return settings with
                    {
                        Whitelist = settings.Whitelist?.Where(_ipHelperService.ValidateIp).ToList() ?? new()
                    };
                }
                catch (Exception ex)
                {
                    _logService.LogException(ex);
                }
            }

            return new()
            {
                Whitelist = new()
            };
        }

        public void Save(Settings settings)
        {
            var json = JsonSerializer.Serialize(settings);

            File.WriteAllText(_path, json);
        }
    }
}
