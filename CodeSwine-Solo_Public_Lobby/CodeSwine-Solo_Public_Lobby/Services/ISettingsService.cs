using GTA5_Private_Public_Lobby.Models;

namespace GTA5_Private_Public_Lobby.Services
{
    public interface ISettingsService
    {
        Settings Load();

        void Save(Settings settings);
    }
}
