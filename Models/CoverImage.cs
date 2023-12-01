namespace SteganographyNotepad.Models;

/// <summary>
/// Represents an image to be used to decode text from or encode text to.
/// </summary>
/// <param name="path">The absolute path to the image on disk.</param>
/// <param name="displayName">The text consisting of the tail end of the path to be
/// displayed on the UI.</param>
public sealed class CoverImage(string path, string displayName)
{
    /// <summary>
    /// Gets or sets the absolute path to the file on disk.
    /// </summary>
    public string Path { get; set; } = path;

    /// <summary>
    /// Gets or sets the text consisting of the tail end of the path to be displayed
    /// on the UI.
    /// </summary>
    public string DisplayName { get; set; } = displayName;
}