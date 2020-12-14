using CodeSwine_Solo_Public_Lobby.Services;
using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace CodeSwine_Solo_Public_Lobby
{
    public partial class MainWindow : Window
    {
        private readonly FirewallService _firewallService;
        private readonly HotkeyService _hotkeyService;
        private readonly LocalIpService _localIpService;
        private readonly IpHelperService _ipHelperService;
        private readonly SettingsService _settingsService;

        private readonly ViewModel _viewModel = new();
        private bool _loading = true;

        public MainWindow(FirewallService firewallService, HotkeyService hotkeyService, IpHelperService ipHelperService, LocalIpService localIpService, SettingsService whitelistService)
        {
            _firewallService = firewallService;
            _hotkeyService = hotkeyService;
            _ipHelperService = ipHelperService;
            _localIpService = localIpService;
            _settingsService = whitelistService;

            InitializeComponent();
            Loaded += MainWindow_Loaded;

            DataContext = _viewModel;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var settings = _settingsService.Load();

            _viewModel.PublicIp = _localIpService.PublicIpAddress;
            _viewModel.LanIps = string.Join(", ", _localIpService.LanIpAddresses);
            _viewModel.AllowLanIps = settings.AllowLan;

            foreach (var ip in settings.Whitelist)
            {
                _viewModel.Whitelist.Add(IPAddress.Parse(ip));
            }

            _loading = false;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _hotkeyService.Init(this);
            _hotkeyService.Register(ModifierKeys.Control, Key.F10, OnHotKeyPressed);
        }

        protected override void OnClosed(EventArgs e)
        {
            _hotkeyService.UnregisterAll();
            _firewallService.DeleteRules();

            base.OnClosed(e);
        }

        private void Save()
        {
            if (!_loading)
            {
                _settingsService.Save(new()
                {
                    Whitelist = _viewModel.Whitelist.Select(ip => ip.ToString()).ToList(),
                    AllowLan = _viewModel.AllowLanIps
                });
            }
        }

        private void UpdateRules()
        {
            if (!_loading)
            {
                var blacklist = _ipHelperService.GetBlacklistString(_viewModel.Whitelist, _localIpService.LanIpAddresses);

                _viewModel.ErrorMessage =
                    _firewallService.UpsertRules(blacklist, _viewModel.Active)
                        ? string.Empty
                        : _firewallService.ErrorMessage;
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (IpHelperService.ValidateIp(txbIpToAdd.Text, out var address)
                && !_viewModel.Whitelist.Contains(address))
            {
                _viewModel.Whitelist.Add(address);

                Save();
                UpdateRules();
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lsbAddresses.SelectedIndex != -1)
            {
                _viewModel.Whitelist.Remove(IPAddress.Parse(lsbAddresses.SelectedItem.ToString()));

                Save();
                UpdateRules();
            }
        }

        private void ButtonToggleRules_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Active = !_viewModel.Active;
            UpdateRules();
        }

        private void AllowLan_Checked(object sender, RoutedEventArgs e)
        {
            Save();
            UpdateRules();
        }

        private void OnHotKeyPressed()
        {
            _viewModel.Active = !_viewModel.Active;
            UpdateRules();

            System.Media.SystemSounds.Hand.Play();
        }
    }
}
