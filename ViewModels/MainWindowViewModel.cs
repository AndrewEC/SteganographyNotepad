namespace SteganographyNotepad.ViewModels;

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using SteganographyApp.Common.Arguments;
using SteganographyNotepad.Models;
using SteganographyNotepad.Store;
using SteganographyNotepad.Views;

/// <summary>
/// The view model for <see cref="MainWindow"/>.
/// </summary>
[SuppressMessage("Ordering Rules", "SA1201", Justification = "Useless Rule")]
public class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
        LoadTextCommand = ReactiveCommand.Create(LoadText);
        SaveTextCommand = ReactiveCommand.Create(SaveText);
        Settings = new(CoverImageSettings);
        CoverImageSettings.PropertyChanged += OnPropertyChanged;
    }

    /// <summary>
    /// Gets the view model for the cover image settings.
    /// </summary>
    public CoverImageSettingsViewModel CoverImageSettings { get; } = new();

    /// <summary>
    /// Gets the view model for the main settings panel.
    /// </summary>
    public SettingsViewModel Settings { get; }

    /// <summary>
    /// Gets the settings of the clean images panel.
    /// </summary>
    public CleanViewModel CleanSettings { get; } = new();

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
        set => this.RaiseAndSetIfChanged(ref selectedTabIndex, value);
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

    private string textContent = string.Empty;

    /// <summary>
    /// Gets or sets the user's custom text.
    /// </summary>
    public string TextContent
    {
        get => textContent;
        set => this.RaiseAndSetIfChanged(ref textContent, value);
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(CoverImageSettings.CoverImages))
        {
            return;
        }
        IsActionEnabled = CoverImageSettings.CoverImages.Length > 0;
        CleanSettings.CoverImages = CoverImageSettings.CoverImages;
    }

    private async void SaveText()
    {
        IsActionEnabled = false;
        try
        {
            IInputArguments arguments = FormDecodeArguments();
            await Encoder.EncodeDataToImagesAsync(TextContent, arguments);
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard("Save Error", e.Message, ButtonEnum.Ok).ShowAsync();
        }
        finally
        {
            IsActionEnabled = true;
        }
    }

    private async void LoadText()
    {
        IsActionEnabled = false;
        try
        {
            IInputArguments arguments = FormDecodeArguments();
            TextContent = await Decoder.DecodeTextFromImagesAsync(arguments);
            SelectedTabIndex = 1;
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard("Load Error", e.Message, ButtonEnum.Ok).ShowAsync();
        }
        finally
        {
            IsActionEnabled = true;
        }
    }

    private IInputArguments FormDecodeArguments()
    {
        SettingsModel settingsConfig = Settings.GetLoadSettings();
        string[] cliArguments = Arguments.FormCliArguments(settingsConfig);
        var parser = new CliParser();
        if (!parser.TryParseArgs(out StorageArguments arguments, cliArguments))
        {
            throw new Exception($"Could not parse settings. Cause: [{parser.LastError.Message}]");
        }
        return arguments.ToCommonArguments();
    }
}
