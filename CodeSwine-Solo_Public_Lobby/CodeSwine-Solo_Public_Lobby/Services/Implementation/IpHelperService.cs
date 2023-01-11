using GTA5_Private_Public_Lobby.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GTA5_Private_Public_Lobby.Services.Implementation
{
    public class IpHelperService : IIpHelperService
    {
        public IEnumerable<IPAddress> GetExtendedWhitelist(IEnumerable<IPAddress> wanIPs, IEnumerable<IPAddress> lanIPs)
        {
            return lanIPs
                .Select(ip => GetSubnet(ip.ToString()))
                .Distinct()
                .SelectMany(GetAllIpsForSubnet)
                .Concat(wanIPs);
        }

        public string GetBlacklistString(IEnumerable<IPAddress> whitelist)
        {
            var sortedAddresses = whitelist
                .Distinct()
                .Select(ComparableIPAddress.From)
                .Sort()
                .ToList();

            return ConstructRange(sortedAddresses);
        }

        public bool ValidateIp(string ip) => ValidateIp(ip, out _);

        public bool ValidateIp(string ip, out IPAddress address)
        {
            return IPAddress.TryParse(ip, out address)
                && address.AddressFamily
                    is AddressFamily.InterNetwork
                    or AddressFamily.InterNetworkV6;
        }

        private static string GetSubnet(string ip) => string.Join(".", ip.Split('.').Take(3));

        private static IEnumerable<IPAddress> GetAllIpsForSubnet(string subnet)
        {
            for (var i = 0; i <= 255; i++)
            {
                yield return IPAddress.Parse($"{subnet}.{i}");
            }
        }

        private static string ConstructRange(List<ComparableIPAddress> list)
        {
            if (list.Count > 0)
            {
                var scope = new StringBuilder("0.0.0.0-");
                var isContiguous = false;

                for (var i = 0; i < list.Count; i++)
                {
                    if (!isContiguous)
                    {
                        scope.Append(list[i] - 1);
                        scope.Append(',');
                    }

                    var next = list[i] + 1;
                    isContiguous = i < list.Count - 1 && next == list[i + 1];

                    if (!isContiguous)
                    {
                        scope.Append(next);
                        scope.Append('-');
                    }
                }

                scope.Append("255.255.255.255");

                return scope.ToString();
            }

            return "*";
        }
    }
}
