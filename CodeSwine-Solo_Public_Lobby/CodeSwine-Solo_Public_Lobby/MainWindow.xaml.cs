using GTA5_Private_Public_Lobby.Extensions;
using GTA5_Private_Public_Lobby.Services;
using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace GTA5_Private_Public_Lobby
{
    public partial class MainWindow : Window
    {
        private readonly IConnectionService _connectionService;
        private readonly ICurrentIpService _currentIpService;
        private readonly IFirewallService _firewallService;
        private readonly IHotkeyService _hotkeyService;
        private readonly IIpHelperService _ipHelperService;
        private readonly ISettingsService _settingsService;

        private readonly ViewModel _viewModel = new();
        private readonly DispatcherTimer _timer = new();
        private bool _loading = true;

        public MainWindow(IConnectionService connectionService, ICurrentIpService currentIpService, IFirewallService firewallService, IHotkeyService hotkeyService, IIpHelperService ipHelperService, ISettingsService whitelistService)
        {
            _connectionService = connectionService;
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
            _viewModel.AllowLanIps = settings.AllowLan;
            _viewModel.Whitelist.AddRange(settings.Whitelist.Select(ComparableIPAddress.Parse));

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            _loading = false;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _connectionService.Start(6672);

            _hotkeyService.Init(this);
            _hotkeyService.Register(ModifierKeys.Control, Key.F10, OnHotKeyPressed);
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer.Stop();

            _hotkeyService.UnregisterAll();
            _firewallService.DeleteRules();

            _connectionService.Stop();

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

        private void UpdateExtendedWhitelist()
        {
            if (!_loading)
            {
                _viewModel.ExtendedWhitelist = _viewModel.AllowLanIps
                ? _ipHelperService
                    .GetExtendedWhitelist(_currentIpService.WanIpAddresses, _currentIpService.LanIpAddresses)
                    .Select(ComparableIPAddress.From)
                    .ToList()
                : Enumerable.Empty<IPAddress>();
            }

            UpdateRules();
        }

        private void UpdateRules()
        {
            if (!_loading)
            {
                var whitelist = _viewModel.Whitelist.Concat(_viewModel.ExtendedWhitelist);
                var blacklist = _ipHelperService.GetBlacklistString(whitelist);

                _viewModel.ErrorMessage =
                    _firewallService.UpsertRules(blacklist, _viewModel.Active)
                        ? string.Empty
                        : _firewallService.ErrorMessage;
            }
        }

        private async void ButtonRefreshIps_Click(object sender, RoutedEventArgs e)
        {
            await _currentIpService.RefreshIps();

            _viewModel.WanIps = string.Join(", ", _currentIpService.WanIpAddresses);
            _viewModel.LanIps = string.Join(", ", _currentIpService.LanIpAddresses);

            if (_viewModel.AllowLanIps)
            {
                UpdateExtendedWhitelist();
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (_ipHelperService.ValidateIp(txbIpToAdd.Text, out var address)
                && !_viewModel.Whitelist.Contains(address))
            {
                _viewModel.Whitelist.Add(ComparableIPAddress.From(address));

                Save();
                UpdateRules();
            }
        }

        private void ButtonAddSelected_Click(object sender, RoutedEventArgs e)
        {
            if (lsbActiveConnections.SelectedIndex != -1)
            {
                _viewModel.Whitelist.Add(ComparableIPAddress.Parse(lsbWhitelist.SelectedItem.ToString()));

                Save();
                UpdateRules();
            }
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (lsbWhitelist.SelectedIndex != -1)
            {
                _viewModel.Whitelist.Remove(ComparableIPAddress.Parse(lsbWhitelist.SelectedItem.ToString()));

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
            UpdateExtendedWhitelist();
        }

        private void OnHotKeyPressed()
        {
            ButtonToggleRules_Click(this, null);

            System.Media.SystemSounds.Hand.Play();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var connections = _connectionService.GetActiveConnections(TimeSpan.FromSeconds(15));

            var toRemove = _viewModel.ActiveConnections.Where(existing => !connections.Contains(existing)).ToList();
            var toAdd = connections.Where(c => 
                !_viewModel.ActiveConnections.Contains(c) &&
                !_currentIpService.WanIpAddresses.Contains(c) &&
                !_currentIpService.LanIpAddresses.Contains(c));

            _viewModel.ActiveConnections.RemoveRange(toRemove);
            _viewModel.ActiveConnections.AddRange(toAdd.Select(ComparableIPAddress.From));
        }
    }
}
