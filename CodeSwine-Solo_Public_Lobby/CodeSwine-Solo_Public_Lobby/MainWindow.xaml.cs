using CodeSwine_Solo_Public_Lobby.Extensions;
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
        private readonly ICurrentIpService _currentIpService;
        private readonly IFirewallService _firewallService;
        private readonly IHotkeyService _hotkeyService;
        private readonly IIpHelperService _ipHelperService;
        private readonly ISettingsService _settingsService;

        private readonly ViewModel _viewModel = new();
        private bool _loading = true;

        public MainWindow(ICurrentIpService currentIpService, IFirewallService firewallService, IHotkeyService hotkeyService, IIpHelperService ipHelperService, ISettingsService whitelistService)
        {
            _currentIpService = currentIpService;
            _firewallService = firewallService;
            _hotkeyService = hotkeyService;
            _ipHelperService = ipHelperService;
            _settingsService = whitelistService;

            InitializeComponent();
            Loaded += MainWindow_Loaded;

            DataContext = _viewModel;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ButtonRefreshIps_Click(sender, e);

            var settings = _settingsService.Load();
            var whitelist = _ipHelperService.Sort(settings.Whitelist.Select(IPAddress.Parse));

            _viewModel.AllowLanIps = settings.AllowLan;
            _viewModel.Whitelist.AddRange(whitelist);

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
                var blacklist = _ipHelperService.GetBlacklistString(_viewModel.Whitelist, _currentIpService.LanIpAddresses);

                _viewModel.ErrorMessage =
                    _firewallService.UpsertRules(blacklist, _viewModel.Active)
                        ? string.Empty
                        : _firewallService.ErrorMessage;
            }
        }

        private void ButtonRefreshIps_Click(object sender, RoutedEventArgs e)
        {
            _currentIpService.RefreshIps();
            _viewModel.WanIps = string.Join(", ", _currentIpService.WanIpAddresses);
            _viewModel.LanIps = string.Join(", ", _currentIpService.LanIpAddresses);
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_ipHelperService.ValidateIp(txbIpToAdd.Text, out var address)
                && !_viewModel.Whitelist.Contains(address))
            {
                _viewModel.Whitelist.Add(address);

                Save();
                UpdateRules();
            }
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
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
