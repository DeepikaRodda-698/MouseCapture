using MouseCapture.Helpers;
using MouseCapture.Models;
using MouseCapture.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MouseCapture.ViewModels
{
    public sealed class MainViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly MouseHookService _mouseHook;
        private bool _isRunning;
        private readonly int _maxEvents = 1000;

        public ObservableCollection<MouseEventInfo> Events { get; } = new();

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public bool IsRunning
        {
            get => _isRunning;
            private set
            {
                if (value == _isRunning) return;
                _isRunning = value;
                OnPropertyChanged();
                (StartCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (StopCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public MainViewModel() : this(new MouseHookService()) { }

        public MainViewModel(MouseHookService mouseHook)
        {
            _mouseHook = mouseHook ?? throw new ArgumentNullException(nameof(mouseHook));
            _mouseHook.MouseEventReceived += OnMouseEvent;

            StartCommand = new RelayCommand(Start, () => !IsRunning);
            StopCommand = new RelayCommand(Stop, () => IsRunning);
        }

        private void OnMouseEvent(object? sender, MouseEventInfo e)
        {
            Events.Add(e);
            if (Events.Count > _maxEvents) Events.RemoveAt(0);
        }

        private void Start()
        {
            _mouseHook.Start();
            IsRunning = true;
        }

        private void Stop()
        {
            _mouseHook.Stop();
            IsRunning = false;
        }

        public void Dispose()
        {
            _mouseHook.MouseEventReceived -= OnMouseEvent;
            _mouseHook.Dispose();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}