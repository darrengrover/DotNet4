using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Threading;
using TagReaderShared;

namespace CardReaderMonitor
{
    public sealed class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly DispatcherTimer _timer;
        private SharedReaderStatus _sharedStatus;

        public ObservableCollection<ReaderStatusViewModel> Readers { get; private set; }

        private bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            private set
            {
                if (_isConnected == value) return;
                _isConnected = value;
                OnPropertyChanged();
                OnPropertyChanged("ConnectionStatusText");
                OnPropertyChanged("ConnectionStatusColor");
            }
        }

        public string ConnectionStatusText
        {
            get { return IsConnected ? "Connected to Service" : "Service Unavailable"; }
        }

        public Brush ConnectionStatusColor
        {
            get
            {
                return IsConnected
                    ? new SolidColorBrush(Color.FromRgb(39, 174, 96))  // green
                    : new SolidColorBrush(Color.FromRgb(231, 76, 60)); // red
            }
        }

        private static readonly TimeSpan UiTick = TimeSpan.FromMilliseconds(100);

        public MainViewModel()
        {
            Readers = new ObservableCollection<ReaderStatusViewModel>();

            // Try to attach immediately (non-blocking overall; throws only if fatal)
            TryAttachSharedMemory();

            _timer = new DispatcherTimer { Interval = UiTick };
            _timer.Tick += OnTick;
            _timer.Start();
        }

        private void OnTick(object sender, EventArgs e)
        {
            // If we have no mapping or it flipped unhealthy, attempt re-attach.
            if (_sharedStatus == null || !_sharedStatus.IsHealthy)
            {
                TryAttachSharedMemory();
                if (_sharedStatus == null || !_sharedStatus.IsHealthy)
                {
                    IsConnected = false;
                    return; // Skip read this tick; we'll try again next tick
                }
            }

            // We believe we're attached; try a read. Any IPC error inside flips IsHealthy=false.
            try
            {
                var list = _sharedStatus.ReadAll();

                // Service is reachable (even if no data yet)
                IsConnected = true;

                SyncReaders(list);
            }
            catch
            {
                DropConnection();
            }
        }

        private void TryAttachSharedMemory()
        {
            if (_sharedStatus != null && _sharedStatus.IsHealthy) return;

            try
            {
                if (_sharedStatus != null) _sharedStatus.Dispose();
                _sharedStatus = SharedReaderStatus.OpenConsumerWithRetry(1, 0); // open only; do not CreateOrOpen
                IsConnected = _sharedStatus.IsHealthy;
            }
            catch
            {
                DropConnection(); // remains null; retry next tick
            }
        }

        private void DropConnection()
        {
            IsConnected = false;
            try { if (_sharedStatus != null) _sharedStatus.Dispose(); } catch { }
            _sharedStatus = null;
        }

        private void SyncReaders(IEnumerable<ReaderStatusDto> dtos)
        {
            var dict = (dtos ?? Enumerable.Empty<ReaderStatusDto>())
           .ToDictionary(d => d.LocationId); ;

            // Add/update existing
            foreach (var kv in dict)
            {
                var existing = Readers.FirstOrDefault(r => r.LocationId == kv.Key);
                if (existing == null)
                {
                    var vm = new ReaderStatusViewModel();
                    vm.Update(kv.Value);
                    Readers.Add(vm);
                }
                else
                {
                    existing.Update(kv.Value);
                }
            }

            // remove stale
            for (int i = Readers.Count - 1; i >= 0; i--)
            {
                if (!dict.ContainsKey(Readers[i].LocationId))
                    Readers.RemoveAt(i);
            }
        }

        public void Dispose()
        {
            if (_timer != null) _timer.Stop();
            if (_sharedStatus != null) _sharedStatus.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(name));
        }
    }

}
