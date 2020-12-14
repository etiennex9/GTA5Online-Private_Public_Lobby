using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CodeSwine_Solo_Public_Lobby.Services
{
    public class IpHelperService
    {
        public string GetBlacklistString(IEnumerable<IPAddress> whitelist)
        {
            var sortedAddresses = whitelist.OrderBy(GetIntFromIp).ToList();

            return ConstructRange(sortedAddresses);
        }

        public static bool ValidateIp(string ip) => ValidateIp(ip, out _);

        public static bool ValidateIp(string ip, out IPAddress address)
        {
            return IPAddress.TryParse(ip, out address)
                && address.AddressFamily
                    is AddressFamily.InterNetwork
                    or AddressFamily.InterNetworkV6;
        }

        private uint GetIntFromIp(IPAddress address)
        {
            var ip = address.GetAddressBytes();

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(ip);
            }

            return BitConverter.ToUInt32(ip, 0);
        }

        private static IPAddress GetIpFromInt(uint ip)
        {
            var newBytes = new IPAddress(ip).GetAddressBytes();
            Array.Reverse(newBytes);
            return new IPAddress(newBytes);
        }

        private IPAddress Substract(IPAddress address) => GetIpFromInt(GetIntFromIp(address) - 1);

        private string ConstructRange(List<IPAddress> list)
        {
            if (list.Count > 0)
            {
                var scope = new StringBuilder("0.0.0.0-");
                var isContiguous = false;

                for (var i = 0; i < list.Count; i++)
                {
                    if (!isContiguous)
                    {
                        scope.Append(Substract(list[i]));
                        scope.Append(',');
                    }

                    var next = GetIntFromIp(list[i]) + 1;
                    isContiguous = i < list.Count - 1 && next == GetIntFromIp(list[i + 1]);

                    if (!isContiguous)
                    {
                        scope.Append(GetIpFromInt(next));
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