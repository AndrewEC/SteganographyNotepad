namespace SteganographyNotepad.ViewModels;

using ReactiveUI;
using SteganographyNotepad.Models;

internal enum SwapDirection
{
    Up = -1,
    Down = 1
}

public class SettingsViewModel(CoverImageSettingsViewModel coverImageSettings) : ViewModelBase
{
    public CoverImageSettingsViewModel CoverImageSettings { get; } = coverImageSettings;

    private string password = "";
    public string Password
    {
        get => password;
        set => this.RaiseAndSetIfChanged(ref password, value);
    }

    private string randomSeed = "";
    public string RandomSeed
    {
        get => randomSeed;
        set => this.RaiseAndSetIfChanged(ref randomSeed, value);
    }

    private string dummyCount = "";
    public string DummyCount
    {
        get => dummyCount;
        set => this.RaiseAndSetIfChanged(ref dummyCount, value);
    }

    private string additionalHashes = "";
    public string AdditionalHashes
    {
        get => additionalHashes;
        set => this.RaiseAndSetIfChanged(ref additionalHashes, value);
    }

    private bool isCompressionEnabled = false;
    public bool IsCompressionEnabled
    {
        get => isCompressionEnabled;
        set => this.RaiseAndSetIfChanged(ref isCompressionEnabled, value);
    }

    public SettingsModel GetLoadSettings() => new()
    {
        Password = Password,
        RandomSeed = RandomSeed,
        DummyCount = DummyCount,
        AdditionalHashes = AdditionalHashes,
        CoverImages = CoverImageSettings.CoverImages,
        IsCompressionEnabled = IsCompressionEnabled.ToString()
    };
}