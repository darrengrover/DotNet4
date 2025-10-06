using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace CardReaderMonitor
{
    public class ReaderStatusViewModel : INotifyPropertyChanged
    {
        public int LocationId { get; set; }
        public int MachineId { get; set; }
        public int SubId { get; set; }
        public string IpAddress { get; set; }

        public string DisplayText => $"{MachineId} ({SubId})";

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusColor));
                OnPropertyChanged(nameof(ConnectionStatus));
                OnPropertyChanged(nameof(ConnectionDotColor));
            }
        }

        private string _ledColor;
        public string LedColor
        {
            get => _ledColor;
            set
            {
                _ledColor = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusColor));
                OnPropertyChanged(nameof(StatusText));
                OnPropertyChanged(nameof(StatusBadgeColor));
            }
        }

        private bool _isActiveStation;
        public bool IsActiveStation
        {
            get => _isActiveStation;
            set
            {
                _isActiveStation = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusText));
            }
        }

        public Brush StatusColor
        {
            get
            {
                if (!IsConnected)
                    return new SolidColorBrush(Color.FromRgb(189, 195, 199)); // Light gray

                return LedColor switch
                {
                    "GREEN" => new SolidColorBrush(Color.FromRgb(144, 238, 144)), // Light green
                    "RED" => new SolidColorBrush(Color.FromRgb(255, 182, 193)),   // Light red
                    "ORANGE" => new SolidColorBrush(Color.FromRgb(255, 218, 185)), // Light orange
                    _ => new SolidColorBrush(Color.FromRgb(189, 195, 199))        // Gray
                };
            }
        }

        public string StatusText
        {
            get
            {
                if (!IsConnected)
                    return "OFFLINE";

                return LedColor switch
                {
                    "GREEN" => IsActiveStation ? "ACTIVE" : "GREEN",
                    "RED" => "IDLE",
                    "ORANGE" => "WARNING",
                    _ => "UNKNOWN"
                };
            }
        }

        public Brush StatusBadgeColor
        {
            get
            {
                if (!IsConnected)
                    return new SolidColorBrush(Color.FromRgb(127, 140, 141)); // Gray

                return LedColor switch
                {
                    "GREEN" => new SolidColorBrush(Color.FromRgb(39, 174, 96)),  // Green
                    "RED" => new SolidColorBrush(Color.FromRgb(231, 76, 60)),    // Red
                    "ORANGE" => new SolidColorBrush(Color.FromRgb(230, 126, 34)), // Orange
                    _ => new SolidColorBrush(Color.FromRgb(127, 140, 141))       // Gray
                };
            }
        }

        public string ConnectionStatus => IsConnected ? "Connected" : "Disconnected";

        public Brush ConnectionDotColor => IsConnected
            ? new SolidColorBrush(Color.FromRgb(39, 174, 96))  // Green
            : new SolidColorBrush(Color.FromRgb(231, 76, 60)); // Red

        private DateTime _lastUpdate;
        public DateTime LastUpdate
        {
            get => _lastUpdate;
            set
            {
                _lastUpdate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(LastUpdateText));
            }
        }
        public void Update(TagReaderShared.ReaderStatusDto dto)
        {
            if (dto == null) return;

            // IDs / addressing
            LocationId = dto.LocationId;   // NOTE: LocationID (capital ID)
            MachineId = dto.MachineId;
            SubId = dto.SubId;
            IpAddress = dto.IpAddress;

            // Status
            IsConnected = dto.IsConnected;                 // matches your VM property name
            LedColor = dto.LedColor ?? "GREY";        // DTO uses CurrentLED
            IsActiveStation = dto.IsActiveStation;

            // Timestamps
            LastUpdate = dto.LastUpdate;

            // Computed label updated from Machine/Sub
            OnPropertyChanged(nameof(DisplayText));
        }
        public string LastUpdateText => $"Updated: {LastUpdate:HH:mm:ss}";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}