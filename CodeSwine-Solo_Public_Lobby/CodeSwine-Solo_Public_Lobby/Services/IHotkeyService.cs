using System;
using System.Windows;
using System.Windows.Input;

namespace GTA5_Private_Public_Lobby.Services
{
    public interface IHotkeyService : IDisposable
    {
        void Init(Window window);

        bool Register(ModifierKeys modifier, Key key, Action action);

        void Unregister(ModifierKeys modifier, Key key);

        void UnregisterAll();
    }
}
