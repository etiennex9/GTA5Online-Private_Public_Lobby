namespace GTA5_Private_Public_Lobby.Services
{
    public interface IFirewallService
    {
        string ErrorMessage { get; }

        bool UpsertRules(string blacklist, bool enabled);

        bool DeleteRules();
    }
}
