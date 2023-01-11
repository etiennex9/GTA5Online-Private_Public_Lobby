using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace GTA5_Private_Public_Lobby.Services.Implementation
{
    public class HotkeyService : IHotkeyService
    {
        private readonly ILogService _logService;

        private readonly Dictionary<(Key, ModifierKeys), int> _registeredKeys = new();
        private readonly Dictionary<int, Action> _hotkeyActions = new();

        private int _nextHotkeyId = 9000;
        private Window _window = null;
        private HwndSource _source = null;

        public HotkeyService(ILogService logService)
        {
            _logService = logService;
        }

        public void Init(Window window)
        {
            if (_window is not null)
            {
                throw new Exception("Already initialized");
            }

            _window = window;

            var helper = new WindowInteropHelper(_window);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
        }

        public bool Register(ModifierKeys modifier, Key key, Action action)
        {
            if (_registeredKeys.ContainsKey((key, modifier)))
            {
                throw new Exception("Hotkey is already registered");
            }

            var hotkeyId = _nextHotkeyId++;
            _registeredKeys[(key, modifier)] = hotkeyId;
            _hotkeyActions[hotkeyId] = action;

            var vKey = KeyInterop.VirtualKeyFromKey(key);

            var helper = new WindowInteropHelper(_window);
            return RegisterHotKey(helper.Handle, hotkeyId, (uint)modifier, (uint)vKey);
        }

        public void Unregister(ModifierKeys modifier, Key key)
        {
            if (_registeredKeys.Remove((key, modifier), out var hotkeyId))
            {
                _hotkeyActions.Remove(hotkeyId);

                var helper = new WindowInteropHelper(_window);
                UnregisterHotKey(helper.Handle, hotkeyId);
            }
        }

        public void UnregisterAll()
        {
            var registeredKeys = _registeredKeys.Keys.ToList();

            foreach ((var key, var modifier) in registeredKeys)
            {
                Unregister(modifier, key);
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;

            if (msg == WM_HOTKEY)
            {
                var hotkeyId = wParam.ToInt32();

                if (_hotkeyActions.TryGetValue(hotkeyId, out var action))
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        _logService.LogException(ex);
                    }

                    handled = true;
                }
            }

            return IntPtr.Zero;
        }

        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(
            [In] IntPtr hWnd,
            [In] int id,
            [In] uint fsModifiers,
            [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);

        #region IDisposable

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    UnregisterAll();
                    _source.RemoveHook(HwndHook);
                    _source = null;
                    _window = null;
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
