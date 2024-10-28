namespace MusicPlayer;

public static class DisplayTitle
{
    private const int ShortTitleLength = 60;

    public static string Shorten(string title) => title.Length <= ShortTitleLength ? title : title[..(ShortTitleLength - 3)] + "...";
}