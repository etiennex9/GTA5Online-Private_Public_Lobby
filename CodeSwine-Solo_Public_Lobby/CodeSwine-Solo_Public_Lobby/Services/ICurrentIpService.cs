using System.Collections.Generic;
using System.Net;

namespace CodeSwine_Solo_Public_Lobby.Services
{
    public interface ICurrentIpService
    {
        List<IPAddress> WanIpAddresses { get; }

        List<IPAddress> LanIpAddresses { get; }

        void RefreshIps();
    }
}
