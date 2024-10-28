using System;

namespace MusicPlayer;

public class QueuedTrack(Track track, AudioPlayer audioPlayer) : IEquatable<QueuedTrack>
{
    public Track Track => track;

    private readonly Guid _id = Guid.NewGuid();

    public bool Equals(QueuedTrack? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return _id.Equals(other._id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((QueuedTrack)obj);
    }

    public override int GetHashCode()
    {
        return _id.GetHashCode();
    }

    public void Remove()
    {
        audioPlayer.RemoveTrack(this);
    }
}