using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using YoutubeDLSharp;

namespace MusicPlayer.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    public ObservableCollection<TrackNode> SelectedTracks { get; set; } = [];

    [ObservableProperty] private ObservableCollection<TrackNode> _tracks;
    [ObservableProperty] private string _downloadStatus = "";
    [ObservableProperty] private string _downloadUrl = "";

    public AudioPlayer AudioPlayer { get; } = new();

    public MainWindowViewModel()
    {
        Tracks = GetTrackNodes("audio");
    }

    private static ObservableCollection<TrackNode> GetTrackNodes(string directoryPath)
    {
        var children = new ObservableCollection<TrackNode>();

        foreach (var file in Directory.EnumerateFileSystemEntries(directoryPath))
        {
            if (Directory.Exists(file))
            {
                children.Add(new TrackNode(Path.GetFileName(file), GetTrackNodes(file)));
                continue;
            }

            children.Add(new TrackNode(new Track(file)));
        }

        return children;
    }

    public async Task Test()
    {
        var dl = new YoutubeDL
        {
            YoutubeDLPath = "lib/yt-dlp.exe",
            FFmpegPath = "lib/ffmpeg.exe",
            OutputFolder = "audio"
        };

        DownloadStatus = "...";

        var result = await dl.RunAudioDownload(DownloadUrl);
        var path = result.Data;

        if (path is null)
        {
            DownloadStatus = "Failed to download!";
            return;
        }

        DownloadStatus = "Successfully downloaded!";

        Tracks = GetTrackNodes("audio");
    }

    public void AddToQueue()
    {
        foreach (var selectedTrack in SelectedTracks)
        {
            if (selectedTrack.Track is not null)
            {
                AudioPlayer.QueuedTracks.Add(new QueuedTrack(selectedTrack.Track, AudioPlayer));
            }
        }
    }

    public void Dispose()
    {
        AudioPlayer.Dispose();
    }
}