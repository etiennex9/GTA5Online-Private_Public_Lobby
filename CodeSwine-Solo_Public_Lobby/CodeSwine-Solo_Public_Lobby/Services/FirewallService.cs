using NetFwTypeLib;
using System;
using System.Net.Sockets;

namespace CodeSwine_Solo_Public_Lobby.Services
{
    public class FirewallService
    {
        private const string _inboundRuleName = "GTA5 CodeSwine - Private Public Lobby Inbound";
        private const string _outboundRuleName = "GTA5 CodeSwine - Private Public Lobby Outbound";

        private readonly LogService _logService;

        public FirewallService(LogService logService)
        {
            _logService = logService;
        }

        public string ErrorMessage { get; private set; }

        public bool UpsertRules(string blacklist, bool enabled)
        {
            if (string.IsNullOrWhiteSpace(blacklist))
            {
                blacklist = "*";
            }

            // Try to update existing rules
            try
            {
                var firewallPolicy = InitFwPolicy();

                var inboundRule = firewallPolicy.Rules.Item(_inboundRuleName);
                var outboundRule = firewallPolicy.Rules.Item(_outboundRuleName);

                inboundRule.Enabled = enabled;
                inboundRule.RemoteAddresses = blacklist;
                outboundRule.Enabled = enabled;
                outboundRule.RemoteAddresses = blacklist;

                return true;
            }
            catch
            {
                // Rules probably didn't exist, try to create them
                return CreateRules(blacklist, enabled);
            }
        }

        /// <summary>
        /// Removes CodeSwine Inbound & Outbound firewall rules at program exit.
        /// </summary>
        public bool DeleteRules()
        {
            try
            {
                var firewallPolicy = InitFwPolicy();

                firewallPolicy.Rules.Remove(_inboundRuleName);
                firewallPolicy.Rules.Remove(_outboundRuleName);

                return true;
            }
            catch (Exception e)
            {
                _logService.LogException(e);

                ErrorMessage = "Run this program as administrator!";

                return false;
            }
        }

        private bool CreateRules(string blacklist, bool enabled)
        {
            try
            {
                CreateInbound(blacklist, enabled);
                CreateOutbound(blacklist, enabled);

                return true;
            }
            catch (Exception e)
            {
                _logService.LogException(e);

                ErrorMessage = "Run this program as administrator!";

                return false;
            }
        }

        private static void CreateInbound(string blacklist, bool enabled)
        {
            var firewallRule = InitFwRule();
            firewallRule.Name = _inboundRuleName;
            firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;
            firewallRule.RemoteAddresses = blacklist;
            firewallRule.Enabled = enabled;

            var firewallPolicy = InitFwPolicy();
            firewallPolicy.Rules.Add(firewallRule);
        }

        private static void CreateOutbound(string blacklist, bool enabled)
        {
            var firewallRule = InitFwRule();
            firewallRule.Name = _outboundRuleName;
            firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
            firewallRule.RemoteAddresses = blacklist;
            firewallRule.Enabled = enabled;

            var firewallPolicy = InitFwPolicy();
            firewallPolicy.Rules.Add(firewallRule);
        }

        private static INetFwRule InitFwRule()
        {
            var firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));

            firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            firewallRule.Protocol = (int)ProtocolType.Udp;
            firewallRule.InterfaceTypes = "All";
            firewallRule.LocalPorts = "6672";

            return firewallRule;
        }

        private static INetFwPolicy2 InitFwPolicy() => (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
    }
}
