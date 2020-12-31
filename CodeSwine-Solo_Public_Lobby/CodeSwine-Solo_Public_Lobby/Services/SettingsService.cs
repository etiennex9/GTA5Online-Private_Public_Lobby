﻿using CodeSwine_Solo_Public_Lobby.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CodeSwine_Solo_Public_Lobby.Services
{
    public class SettingsService
    {
        private readonly IpHelperService _ipHelperService;
        private readonly LogService _logService;

        private readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        public SettingsService(IpHelperService ipHelperService, LogService logService)
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
