using System;
using System.Net;

namespace CodeSwine_Solo_Public_Lobby.Services
{
    public class LocalIpService
    {
        private readonly LogService _logService;

        private string _publicIpAddress;

        public LocalIpService(LogService logService)
        {
            _logService = logService;
        }

        public string PublicIpAddress => _publicIpAddress ??= GetPublicIpAddress();

        public void RefreshIps()
        {
            _publicIpAddress = null;
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
    }
}
