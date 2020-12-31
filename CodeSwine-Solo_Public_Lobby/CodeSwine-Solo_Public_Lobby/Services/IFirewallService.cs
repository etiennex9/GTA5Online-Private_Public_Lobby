namespace CodeSwine_Solo_Public_Lobby.Services
{
    public interface IFirewallService
    {
        string ErrorMessage { get; }

        bool UpsertRules(string blacklist, bool enabled);

        bool DeleteRules();
    }
}
