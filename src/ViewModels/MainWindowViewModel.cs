namespace SteganographyNotepad.ViewModels;

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using SteganographyApp.Common;
using SteganographyApp.Common.Arguments;
using SteganographyNotepad.Log;
using SteganographyNotepad.Models;
using SteganographyNotepad.Store;
using SteganographyNotepad.Views;

/// <summary>
/// The view model for <see cref="MainWindow"/>.
/// </summary>
[SuppressMessage("Ordering Rules", "SA1201", Justification = "Reviewed")]
public class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
        LoadTextCommand = ReactiveCommand.Create(LoadText);
        SaveTextCommand = ReactiveCommand.Create(SaveText);

        appState = new();
        appState.PropertyChanged += OnAppStateChanged;

        SettingsViewDataContext = new(appState, LoadTextCommand);
        EditViewDataContext = new(appState, SaveTextCommand);
        CleanViewDataContext = new(appState);
    }

    private readonly ConsoleLogger<MainWindowViewModel> logger = new();

    private readonly AppStateProperties appState;

    /// <summary>
    /// Gets the data context for the EditView.
    /// </summary>
    public EditViewModel EditViewDataContext { get; }

    /// <summary>
    /// Gets the data context for the SettingsView.
    /// </summary>
    public SettingsViewModel SettingsViewDataContext { get; }

    /// <summary>
    /// Gets the data contet for the CleanView.
    /// </summary>
    public CleanViewModel CleanViewDataContext { get; }

    /// <summary>
    /// Gets the command triggered by the click of the load button to initiate the
    /// process of decoding and displaying the text content from the cover images.
    /// </summary>
    public ReactiveCommand<Unit, Unit> LoadTextCommand { get; }

    /// <summary>
    /// Gets the command triggered by the click of the save button to initiate
    /// the process of encoding and storing the text content to the cover images.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SaveTextCommand { get; }

    private int selectedTabIndex = 0;

    /// <summary>
    /// Gets or sets the index of the currently selected tab on the main tab control.
    /// </summary>
    public int SelectedTabIndex
    {
        get => selectedTabIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref selectedTabIndex, value);
            appState.SelectedTabIndex = value;
        }
    }

    private bool isActionEnabled = false;

    /// <summary>
    /// Gets or sets a value indicating whether the load and save buttons should be enabled.
    /// </summary>
    public bool IsActionEnabled
    {
        get => isActionEnabled;
        set => this.RaiseAndSetIfChanged(ref isActionEnabled, value);
    }

    private void OnAppStateChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender != appState)
        {
            return;
        }

        switch (e.PropertyName)
        {
            case nameof(appState.IsActionEnabled):
                IsActionEnabled = appState.IsActionEnabled;
                break;
            case nameof(appState.SelectedTabIndex):
                SelectedTabIndex = appState.SelectedTabIndex;
                break;
            case nameof(appState.CoverImages):
                appState.IsActionEnabled = appState.CoverImages.Length > 0;
                break;
        }
    }

    private async void SaveText()
    {
        logger.Log("Saving text to images.");
        appState.IsActionEnabled = false;
        try
        {
            IInputArguments arguments = FormEncodeOrDecodeArguments();
            await Encoder.EncodeDataToImagesAsync(appState.TextContent, arguments);
        }
        catch (Exception e)
        {
            logger.Error("Could not save text to images.", e);
            await MessageBoxManager.GetMessageBoxStandard("Save Error", e.Message, ButtonEnum.Ok).ShowAsync();
        }
        finally
        {
            appState.IsActionEnabled = true;
        }
    }

    private async void LoadText()
    {
        logger.Log("Loading text from images.");
        appState.IsActionEnabled = false;
        try
        {
            IInputArguments arguments = FormEncodeOrDecodeArguments();
            appState.TextContent = await Decoder.DecodeTextFromImagesAsync(arguments);
            appState.SelectedTabIndex = 1;
        }
        catch (Exception e)
        {
            logger.Error("Could not load text from images.", e);
            await MessageBoxManager.GetMessageBoxStandard("Load Error", e.Message, ButtonEnum.Ok).ShowAsync();
        }
        finally
        {
            appState.IsActionEnabled = true;
        }
    }

    private IInputArguments FormEncodeOrDecodeArguments()
    {
        SettingsModel settingsConfig = SettingsViewDataContext.AsModel();
        string[] cliArguments = Arguments.FormCliArguments(settingsConfig);
        logger.Log($"Forming CLI arguments: [{string.Join(", ", cliArguments)}]");
        var parser = new CliParser();
        if (!parser.TryParseArgs(out StorageArguments arguments, cliArguments))
        {
            logger.Error("Could not parse CLI arguments.", parser.LastError);
            throw new Exception($"Could not parse settings. Cause: [{parser.LastError?.Message ?? "unknown"}]");
        }

        return arguments.ToCommonArguments();
    }
}
