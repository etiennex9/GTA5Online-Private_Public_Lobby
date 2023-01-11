using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CodeSwine_Solo_Public_Lobby.Services
{
    public interface IIpHelperService
    {
        IEnumerable<IPAddress> GetExtendedWhitelist(IEnumerable<IPAddress> wanIPs, IEnumerable<IPAddress> lanIPs);

        string GetBlacklistString(IEnumerable<IPAddress> whitelist);

        bool ValidateIp(string ip);

        bool ValidateIp(string ip, out IPAddress address);
    }
}