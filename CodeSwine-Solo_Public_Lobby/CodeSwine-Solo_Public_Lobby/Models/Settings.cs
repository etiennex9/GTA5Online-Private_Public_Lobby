using System.Collections.Generic;

namespace CodeSwine_Solo_Public_Lobby.Models
{
    public record Settings
    {
        public List<string> Whitelist { get; init; }
    }
}
