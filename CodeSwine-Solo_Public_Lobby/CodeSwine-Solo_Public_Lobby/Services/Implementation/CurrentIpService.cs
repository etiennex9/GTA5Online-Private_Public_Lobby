using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CodeSwine_Solo_Public_Lobby.Services.Implementation
{
    public class CurrentIpService : ICurrentIpService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogService _logService;

        public CurrentIpService(IHttpClientFactory httpClientFactory, ILogService logService)
        {
            _httpClientFactory = httpClientFactory;
            _logService = logService;
        }

        public IReadOnlyCollection<IPAddress> WanIpAddresses { get; private set; }

        public IReadOnlyCollection<IPAddress> LanIpAddresses { get; private set; }

        public async Task RefreshIps()
        {
            WanIpAddresses = await GetWanIpAddresses();
            LanIpAddresses = await GetLanIpAddresses();
        }

        private async Task<IReadOnlyCollection<IPAddress>> GetWanIpAddresses()
        {
            var results = await Task.WhenAll(
                DownloadString("https://ipv4.icanhazip.com"),
                DownloadString("https://ipv6.icanhazip.com")
            );

            return results.Where(r => r.success).Select(r => r.value).ToList();

            async Task<(bool success, IPAddress value)> DownloadString(string uri)
            {
                var httpClient = _httpClientFactory.CreateClient();

                try
                {
                    var rawIp = await httpClient.GetStringAsync(uri);
                    var ip = IPAddress.Parse(rawIp.Trim());

                    return (true, ip);
                }
                catch (Exception ex)
                {
                    _logService.LogException(ex);

                    return (false, null);
                }
            }
        }

        private async Task<IReadOnlyCollection<IPAddress>> GetLanIpAddresses()
        {
            return (await TryGet())
                .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                .ToList();

            async Task<IEnumerable<IPAddress>> TryGet()
            {
                try
                {
                    var hostEntry = await Dns.GetHostEntryAsync(Dns.GetHostName());

                    return hostEntry.AddressList;
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
