using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace CodeSwine_Solo_Public_Lobby.Services
{
    public interface ICurrentIpService
    {
        IReadOnlyCollection<IPAddress> WanIpAddresses { get; }

        IReadOnlyCollection<IPAddress> LanIpAddresses { get; }

        Task RefreshIps();
    }
}
