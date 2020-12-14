using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace CodeSwine_Solo_Public_Lobby.Services
{
    public class LocalIpService
    {
        private readonly LogService _logService;

        private string _publicIpAddress;
        private List<string> _lanIpAddresses;

        public LocalIpService(LogService logService)
        {
            _logService = logService;
        }

        public string PublicIpAddress => _publicIpAddress ??= GetPublicIpAddress();

        public List<string> LanIpAddresses => _lanIpAddresses ??= GetLanIpAddresses().ToList();

        public void RefreshIps()
        {
            _publicIpAddress = null;
            _lanIpAddresses = null;
        }

        private string GetPublicIpAddress()
        {
            // Try for ipv6 first, but if that fails get ipv4
            try
            {
                return new WebClient().DownloadString("https://ipv6.icanhazip.com").Trim();
            }
            catch (Exception ex)
            {
                _logService.LogException(ex);
            }

            try
            {
                return new WebClient().DownloadString("https://ipv4.icanhazip.com").Trim();
            }
            catch (Exception ex)
            {
                _logService.LogException(ex);
            }

            return "Unable to fetch IP.";
        }

        private IEnumerable<string> GetLanIpAddresses()
        {
            IEnumerable<IPAddress> ipAddresses;

            try
            {
                ipAddresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            }
            catch (Exception ex)
            {
                _logService.LogException(ex);
                yield break;
            }

            foreach (var ipAddress in ipAddresses)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    yield return ipAddress.ToString();
                }
            }
        }
    }
}
