namespace SteganographyNotepad.Models;

/// <summary>
/// A model representing the arguments used with the Steganography component
/// to encode text to an image or decode text from an image.
/// </summary>
public sealed class SettingsModel
{
    /// <summary>
    /// Gets or sets the password used to encrypt the text content.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the random seed used to randomize the text content being saved.
    /// </summary>
    public string RandomSeed { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of dummies to be added to the fully encoded text content.
    /// </summary>
    public string DummyCount { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of additional times the password and other key arguments
    /// will be hashed.
    /// </summary>
    public string AdditionalHashes { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a flag indicating if the text content should be gzip compressed.
    /// </summary>
    public string IsCompressionEnabled { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the images that the text content will be written to.
    /// </summary>
    public CoverImage[] CoverImages { get; set; } = [];
}