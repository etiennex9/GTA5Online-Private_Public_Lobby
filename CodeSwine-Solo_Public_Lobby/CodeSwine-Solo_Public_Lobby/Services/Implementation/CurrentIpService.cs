using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace CodeSwine_Solo_Public_Lobby.Services.Implementation
{
    public class CurrentIpService : ICurrentIpService
    {
        private readonly ILogService _logService;

        private List<string> _wanIpAddresses;
        private List<string> _lanIpAddresses;

        public CurrentIpService(ILogService logService)
        {
            _logService = logService;
        }

        public List<string> WanIpAddresses => _wanIpAddresses ??= GetWanIpAddresses().ToList();

        public List<string> LanIpAddresses => _lanIpAddresses ??= GetLanIpAddresses().ToList();

        public void RefreshIps()
        {
            _wanIpAddresses = null;
            _lanIpAddresses = null;
        }

        private IEnumerable<string> GetWanIpAddresses()
        {
            if (DownloadString("https://ipv6.icanhazip.com", out var ipv6))
            {
                yield return ipv6;
            }

            if (DownloadString("https://ipv4.icanhazip.com", out var ipv4))
            {
                yield return ipv4;
            }

            bool DownloadString(string uri, out string value)
            {
                try
                {
                    value = new WebClient().DownloadString(uri).Trim();
                    return true;
                }
                catch (Exception ex)
                {
                    _logService.LogException(ex);

                    value = null;
                    return false;
                }
            }
        }

        private IEnumerable<string> GetLanIpAddresses()
        {
            foreach (var ipAddress in TryGet())
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    yield return ipAddress.ToString();
                }
            }

            IEnumerable<IPAddress> TryGet()
            {
                try
                {
                    return Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                }
                catch (Exception ex)
                {
                    _logService.LogException(ex);

                    return Enumerable.Empty<IPAddress>();
                }
            }
        }
    }
}
