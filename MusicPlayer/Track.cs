using System.IO;

namespace MusicPlayer;

public class Track(string filePath)
{
    public string Title { get; } = Path.GetFileNameWithoutExtension(filePath);
    public string ShortTitle => DisplayTitle.Shorten(Title);
    public string FilePath { get; } = filePath;
}