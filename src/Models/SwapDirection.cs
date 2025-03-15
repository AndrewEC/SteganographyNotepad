namespace SteganographyNotepad.Models;

/// <summary>
/// Indicates the direction a given cover image should be moved
/// within the context of the current cover images array.
/// </summary>
internal enum SwapDirection
{
    /// <summary>
    /// Move the image up to the beginning of the cover images array.
    /// </summary>
    Up = -1,

    /// <summary>
    /// Move the image down to the end of the cover images array.
    /// </summary>
    Down = 1,
}
