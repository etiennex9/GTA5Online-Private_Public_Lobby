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

        private List<IPAddress> _wanIpAddresses;
        private List<IPAddress> _lanIpAddresses;

        public CurrentIpService(ILogService logService)
        {
            _logService = logService;
        }

        public List<IPAddress> WanIpAddresses => _wanIpAddresses ??= GetWanIpAddresses().ToList();

        public List<IPAddress> LanIpAddresses => _lanIpAddresses ??= GetLanIpAddresses().ToList();

        public void RefreshIps()
        {
            _wanIpAddresses = null;
            _lanIpAddresses = null;
        }

        private IEnumerable<IPAddress> GetWanIpAddresses()
        {
            if (DownloadString("https://ipv6.icanhazip.com", out var ipv6))
            {
                yield return ipv6;
            }

            if (DownloadString("https://ipv4.icanhazip.com", out var ipv4))
            {
                yield return ipv4;
            }

            bool DownloadString(string uri, out IPAddress value)
            {
                try
                {
                    value = IPAddress.Parse(new WebClient().DownloadString(uri).Trim());
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

        private IEnumerable<IPAddress> GetLanIpAddresses()
        {
            return TryGet().Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);

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
