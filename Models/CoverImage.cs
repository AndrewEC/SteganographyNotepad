namespace SteganographyNotepad.Models;

public sealed class CoverImage(string path, string displayName)
{
    public string Path { get; set; } = path;
    public string DisplayName { get; set; } = displayName;
}