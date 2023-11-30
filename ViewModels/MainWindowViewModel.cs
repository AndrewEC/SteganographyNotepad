namespace SteganographyNotepad.ViewModels;

using System;
using System.ComponentModel;
using System.Reactive;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using SteganographyApp.Common.Arguments;
using SteganographyNotepad.Models;
using SteganographyNotepad.Store;

public class MainWindowViewModel : ViewModelBase
{
    public SettingsViewModel Settings { get; } = new();
    public CleanViewModel CleanSettings { get; } = new();
    public ReactiveCommand<Unit, Unit> LoadTextCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveTextCommand { get; }

    public MainWindowViewModel()
    {
        LoadTextCommand = ReactiveCommand.Create(LoadText);
        SaveTextCommand = ReactiveCommand.Create(SaveText);
        Settings.PropertyChanged += OnPropertyChanged;
    }

    private int selectedTabIndex = 0;
    public int SelectedTabIndex
    {
        get => selectedTabIndex;
        set => this.RaiseAndSetIfChanged(ref selectedTabIndex, value);
    }

    private bool isActionEnabled = false;
    public bool IsActionEnabled
    {
        get => isActionEnabled;
        set => this.RaiseAndSetIfChanged(ref isActionEnabled, value);
    }

    private string textContent = "";
    public string TextContent
    {
        get => textContent;
        set => this.RaiseAndSetIfChanged(ref textContent, value);
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(Settings.CoverImages))
        {
            return;
        }
        IsActionEnabled = Settings.CoverImages.Length > 0;
        CleanSettings.CoverImages = Settings.CoverImages;
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
