namespace SteganographyNotepad.ViewModels;

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using SteganographyApp.Common;
using SteganographyApp.Common.Arguments;
using SteganographyApp.Common.Injection;
using SteganographyNotepad.Log;
using SteganographyNotepad.Models;
using SteganographyNotepad.Store;
using SteganographyNotepad.Views;

/// <summary>
/// The view model for <see cref="CleanView"/>.
/// </summary>
[SuppressMessage("Ordering Rules", "SA1201", Justification = "Reviewed.")]
public class CleanViewModel : ReactiveObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CleanViewModel"/> class.
    /// </summary>
    /// <param name="appState">The root application state. This view model will listen
    /// for changes on the <see cref="AppStateProperties.IsActionEnabled"/> and
    /// <see cref="AppStateProperties.CoverImages"/> properties.</param>
    public CleanViewModel(AppStateProperties appState)
    {
        this.appState = appState;
        this.appState.PropertyChanged += OnAppStateChanged;
        CleanImagesCommand = ReactiveCommand.Create(CleanImages);
    }

    private readonly AppStateProperties appState;

    private readonly ConsoleLogger<CleanViewModel> logger = new();

    /// <summary>
    /// Gets the command to react to the click of the Clean Images button.
    /// </summary>
    public ReactiveCommand<Unit, Unit> CleanImagesCommand { get; }

    private bool isActionEnabled = false;

    /// <summary>
    /// Gets or sets a value indicating whether the Clean Images button
    /// is enabled or disabled.
    /// </summary>
    public bool IsActionEnabled
    {
        get => isActionEnabled;
        set => this.RaiseAndSetIfChanged(ref isActionEnabled, value);
    }

    private CoverImage[] coverImages = [];

    /// <summary>
    /// Gets or sets the cover images array.
    /// <para>This will also attempt to set <see cref="IsActionEnabled"/> to true if the new cover images
    /// array has at least one element. Otherwise, <see cref="IsActionEnabled"/> will be set to false.</para>
    /// </summary>
    public CoverImage[] CoverImages
    {
        get => coverImages;
        set
        {
            IsActionEnabled = value.Length > 0;
            this.RaiseAndSetIfChanged(ref coverImages, value);
        }
    }

    private static Task CleanImagesAsync(IInputArguments arguments)
        => Task.Run(() => CleanImages(arguments));

    private static void CleanImages(IInputArguments arguments)
        => new ImageCleaner(arguments, ServiceContainer.CreateImageStore(arguments)).CleanImages();

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
            case nameof(appState.CoverImages):
                CoverImages = appState.CoverImages;
                break;
        }
    }

    private async void CleanImages()
    {
        logger.Log("Cleaning images.");

        var result = await MessageBoxManager.GetMessageBoxStandard(
            "Clean Images", "Cleaning these images will permanently remove any data store in them. Are you sure you want to continue?", ButtonEnum.OkCancel)
            .ShowAsync();

        if (result != ButtonResult.Ok)
        {
            logger.Log("User cancelled cleaning process.");
            return;
        }

        appState.IsActionEnabled = false;
        try
        {
            IInputArguments arguments = FormArgumentsForClean();
            await CleanImagesAsync(arguments);
        }
        catch (Exception e)
        {
            logger.Error("Could not clean images.", e);
            await MessageBoxManager.GetMessageBoxStandard("Clean Images Error", e.Message, ButtonEnum.Ok).ShowAsync();
        }
        finally
        {
            appState.IsActionEnabled = true;
        }
    }

    private IInputArguments FormArgumentsForClean()
    {
        string[] args = Arguments.FormCliArguments(new SettingsModel() { CoverImages = CoverImages });
        logger.Log($"Cleaning images using CLI arguments: [{string.Join(", ", args)}]");
        return CliParser.ParseArgs<StorageArguments>(args).ToCommonArguments();
    }
}