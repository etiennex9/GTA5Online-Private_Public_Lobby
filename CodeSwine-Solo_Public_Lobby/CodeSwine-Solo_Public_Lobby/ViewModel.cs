using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;

namespace CodeSwine_Solo_Public_Lobby
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<IPAddress> Whitelist { get; } = new();

        private bool _active;
        public bool Active
        {
            get => _active;
            set
            {
                _active = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Active)));
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
            }
        }

        private string _wanIp;
        public string WanIps
        {
            get => _wanIp;
            set
            {
                _wanIp = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WanIps)));
            }
        }

        private string _lanIps;
        public string LanIps
        {
            get => _lanIps;
            set
            {
                _lanIps = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LanIps)));
            }
        }

        private bool _allowLanIps;
        public bool AllowLanIps
        {
            get => _allowLanIps;
            set
            {
                _allowLanIps = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AllowLanIps)));
            }
        }
    }
}