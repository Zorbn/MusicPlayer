using System;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.Wave;

namespace MusicPlayer;

public class AudioPlayer : ObservableObject, IDisposable
{
    public ObservableCollection<QueuedTrack> QueuedTracks { get; set; } = [];

    public float Progress
    {
        set
        {
            if (_audioFile is null) return;

            _audioFile.Position = (long)(_audioFile.Length * value);
        }

        get
        {
            if (_audioFile is null) return 0;

            return _audioFile.Position / (float)_audioFile.Length;
        }
    }

    public float Volume
    {
        set => _outputDevice.Volume = value;
        get => _outputDevice.Volume;
    }

    public string ProgressText =>
        _audioFile is null ? "0 / 0" : $"{ProgressTimeToString(_audioFile.CurrentTime)} / {ProgressTimeToString(_audioFile.TotalTime)}";

    public string PausePlaySymbol => _outputDevice.PlaybackState == PlaybackState.Playing ? "Pause" : "Play";

    private readonly WaveOutEvent _outputDevice = new();
    private AudioFileReader? _audioFile;
    private readonly DispatcherTimer _timer;
    private bool _didStopManually;

    public AudioPlayer()
    {
        _outputDevice.Volume = 0.2f;
        _outputDevice.PlaybackStopped += (sender, args) =>
        {
            if (_didStopManually)
            {
                _didStopManually = false;
                return;
            }

            if (QueuedTracks.Count > 0) QueuedTracks.RemoveAt(0);

            PlayFirst();
        };

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };

        _timer.Tick += (sender, args) =>
        {
            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged(nameof(ProgressText));
        };

        _timer.Start();
    }

    private void Stop()
    {
        _outputDevice.Stop();

        OnPropertyChanged(nameof(PausePlaySymbol));
    }

    public void PlayFirst()
    {
        Stop();

        if (QueuedTracks.Count > 0)
        {
            Play(QueuedTracks[0]);
        }
    }

    private async void Play(QueuedTrack track)
    {
        Stop();

        if (_audioFile is not null)
        {
            await _audioFile.DisposeAsync();
        }

        _audioFile = new AudioFileReader(track.Track.FilePath);

        _outputDevice.Init(_audioFile);
        _outputDevice.Play();

        OnPropertyChanged(nameof(PausePlaySymbol));
    }

    public void Dispose()
    {
        _timer.Stop();

        _outputDevice.Dispose();
        _audioFile?.Dispose();
    }

    public void PausePlay()
    {
        if (_outputDevice.PlaybackState == PlaybackState.Playing)
        {
            _outputDevice.Pause();
        }
        else if (_audioFile is not null)
        {
            _outputDevice.Play();
        }
        else
        {
            PlayFirst();
        }

        OnPropertyChanged(nameof(PausePlaySymbol));
    }

    public void RemoveTrack(QueuedTrack track)
    {
        for (var i = 0; i < QueuedTracks.Count; i++)
        {
            if (!QueuedTracks[i].Equals(track)) continue;

            QueuedTracks.RemoveAt(i);

            if (i == 0 && _outputDevice.PlaybackState == PlaybackState.Playing)
            {
                _didStopManually = true;
                PlayFirst();
            }

            return;
        }
    }

    private static string ProgressTimeToString(TimeSpan timeSpan)
    {
        var text = timeSpan.ToString(@"hh\:mm\:ss").TrimStart(' ', ':', '0');

        return text.Length == 0 ? "0" : text;
    }
}