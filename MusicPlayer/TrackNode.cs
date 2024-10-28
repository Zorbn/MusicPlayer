using System.Collections.ObjectModel;

namespace MusicPlayer;

public class TrackNode
{
    public string Title { get; }
    public string ShortTitle => DisplayTitle.Shorten(Title);
    public ObservableCollection<TrackNode> Children { get; } = [];

    public Track? Track { get; }

    public TrackNode(Track track)
    {
        Track = track;
        Title = track.Title;
    }

    public TrackNode(string directoryName, ObservableCollection<TrackNode> children)
    {
        Title = directoryName;
        Children = children;
    }
}