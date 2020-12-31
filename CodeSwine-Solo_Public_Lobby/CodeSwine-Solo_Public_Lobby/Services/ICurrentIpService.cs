using System.Collections.Generic;

namespace CodeSwine_Solo_Public_Lobby.Services
{
    public interface ICurrentIpService
    {
        List<string> WanIpAddresses { get; }

        List<string> LanIpAddresses { get; }

        void RefreshIps();
    }
}
