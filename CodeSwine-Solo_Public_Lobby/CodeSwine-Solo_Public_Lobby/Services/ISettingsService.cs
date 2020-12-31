using CodeSwine_Solo_Public_Lobby.Models;

namespace CodeSwine_Solo_Public_Lobby.Services
{
    public interface ISettingsService
    {
        Settings Load();

        void Save(Settings settings);
    }
}
