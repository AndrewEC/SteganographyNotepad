namespace SteganographyNotepad.ViewModels;

using System.Diagnostics.CodeAnalysis;
using ReactiveUI;
using SteganographyNotepad.Models;
using SteganographyNotepad.Views;

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

/// <summary>
/// The view model for <see cref="SettingsView"/>.
/// </summary>
/// <param name="coverImageSettings">The data context of the cover images settings panel.</param>
[SuppressMessage("Ordering Rules", "SA1201", Justification = "Useless Rule")]
public class SettingsViewModel(CoverImageSettingsViewModel coverImageSettings) : ViewModelBase
{
    /// <summary>
    /// Gets the data context for the cover image settings panel.
    /// </summary>
    public CoverImageSettingsViewModel CoverImageSettings { get; } = coverImageSettings;

    private string password = string.Empty;

    /// <summary>
    /// Gets or sets the password used to encrypt/decrypt the user's text content.
    /// </summary>
    public string Password
    {
        get => password;
        set => this.RaiseAndSetIfChanged(ref password, value);
    }

    private string randomSeed = string.Empty;

    /// <summary>
    /// Gets or sets the random seed used to randomize the user text content.
    /// </summary>
    public string RandomSeed
    {
        get => randomSeed;
        set => this.RaiseAndSetIfChanged(ref randomSeed, value);
    }

    private string dummyCount = string.Empty;

    /// <summary>
    /// Gets or sets the number of dummies to be inserted into the user
    /// text content.
    /// </summary>
    public string DummyCount
    {
        get => dummyCount;
        set => this.RaiseAndSetIfChanged(ref dummyCount, value);
    }

    private string additionalHashes = string.Empty;

    /// <summary>
    /// Gets or sets the number of additional times the user password, or
    /// other key values, should be hashed.
    /// </summary>
    public string AdditionalHashes
    {
        get => additionalHashes;
        set => this.RaiseAndSetIfChanged(ref additionalHashes, value);
    }

    private bool isCompressionEnabled = false;

    /// <summary>
    /// Gets or sets a value indicating whether the user's text content
    /// should be compressed/decompressed.
    /// </summary>
    public bool IsCompressionEnabled
    {
        get => isCompressionEnabled;
        set => this.RaiseAndSetIfChanged(ref isCompressionEnabled, value);
    }

    /// <summary>
    /// Gets the settings model to configure the encoding or decoding process.
    /// </summary>
    /// <returns>The settings used to configure the encoding or decoding process.</returns>
    public SettingsModel AsModel() => new()
    {
        Password = Password,
        RandomSeed = RandomSeed,
        DummyCount = DummyCount,
        AdditionalHashes = AdditionalHashes,
        CoverImages = CoverImageSettings.CoverImages,
        IsCompressionEnabled = IsCompressionEnabled.ToString(),
    };
}