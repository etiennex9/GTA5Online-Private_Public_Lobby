using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CodeSwine_Solo_Public_Lobby.Services
{
    public interface IIpHelperService
    {
        string GetBlacklistString(IEnumerable<IPAddress> whitelist, IEnumerable<string> lanIps);

        IOrderedEnumerable<IPAddress> Sort(IEnumerable<IPAddress> list);

        bool ValidateIp(string ip);

        bool ValidateIp(string ip, out IPAddress address);
    }
}