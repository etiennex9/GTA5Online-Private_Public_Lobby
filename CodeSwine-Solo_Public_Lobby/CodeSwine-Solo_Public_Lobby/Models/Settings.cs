using System.Collections.Generic;

namespace GTA5_Private_Public_Lobby.Models
{
    public record Settings
    {
        public List<string> Whitelist { get; init; }

        public bool AllowLan { get; init; }
    }
}
