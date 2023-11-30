namespace SteganographyNotepad.Models;

public sealed class SettingsModel
{
    public string Password { get; set; } = string.Empty;
    public string RandomSeed { get; set; } = string.Empty;
    public string DummyCount { get; set; } = string.Empty;
    public string AdditionalHashes { get; set; } = string.Empty;
    public string IsCompressionEnabled { get; set; } = string.Empty;
    public CoverImage[] CoverImages { get; set; } = [];
}