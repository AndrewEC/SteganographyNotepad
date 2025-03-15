namespace SteganographyNotepad.ViewModels;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using ReactiveUI;
using SteganographyNotepad.Models;
using SteganographyNotepad.Views;

/// <summary>
/// The view model for <see cref="SettingsView"/>.
/// </summary>
[SuppressMessage("Ordering Rules", "SA1201", Justification = "Reviewed")]
public class SettingsViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes the settings view model.
    /// </summary>
    /// <param name="appState">The root application state. This view model will listen
    /// for changes on the <see cref="AppStateProperties.IsActionEnabled"/> property.</param>
    /// <param name="loadTextCallback">The callback to be invoked when the load button
    /// has been clicked so text can be loaded from the selected cover images.</param>
    public SettingsViewModel(AppStateProperties appState, ReactiveCommand<Unit, Unit> loadTextCallback)
    {
        this.appState = appState;
        appState.PropertyChanged += OnAppStateChanged;
        CoverImageSettingsViewDataContext = new(appState);
        LoadTextCommand = loadTextCallback;
    }

    private readonly AppStateProperties appState;

    private void OnAppStateChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender != appState)
        {
            return;
        }

        if (e.PropertyName == nameof(appState.IsActionEnabled))
        {
            IsActionEnabled = appState.IsActionEnabled;
        }
    }

    /// <summary>
    /// Gets the data context for the cover image settings panel.
    /// </summary>
    public CoverImageSettingsViewModel CoverImageSettingsViewDataContext { get; }

    /// <summary>
    /// Gets the command to be invoked when the "Load" button has been clicked.
    /// </summary>
    public ReactiveCommand<Unit, Unit> LoadTextCommand { get; }

    private bool isActionEnabled = false;

    /// <summary>
    /// Gets or sets a value indicating whether the "Load" button should be enabled.
    /// </summary>
    public bool IsActionEnabled
    {
        get => isActionEnabled;
        set
        {
            this.RaiseAndSetIfChanged(ref isActionEnabled, value);
        }
    }

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
        CoverImages = CoverImageSettingsViewDataContext.CoverImages,
        IsCompressionEnabled = IsCompressionEnabled.ToString(),
    };
}