namespace SteganographyNotepad.ViewModels;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using SteganographyApp.Common.Arguments;
using SteganographyApp.Common.IO;
using SteganographyNotepad.Models;
using SteganographyNotepad.Store;
using SteganographyNotepad.Views;

/// <summary>
/// The view model for <see cref="CleanView"/>.
/// </summary>
[SuppressMessage("Ordering Rules", "SA1201", Justification = "Useless Rule")]
public class CleanViewModel : ReactiveObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CleanViewModel"/> class.
    /// </summary>
    public CleanViewModel()
    {
        CleanImagesCommand = ReactiveCommand.Create(CleanImages);
    }

    /// <summary>
    /// Gets the command to react to the click of the Clean Images button.
    /// </summary>
    public ReactiveCommand<Unit, Unit> CleanImagesCommand { get; }

    private bool cleanEnabled = false;

    /// <summary>
    /// Gets or sets a value indicating whether the Clean Images button
    /// is enabled or disabled.
    /// </summary>
    public bool CleanEnabled
    {
        get => cleanEnabled;
        set => this.RaiseAndSetIfChanged(ref cleanEnabled, value);
    }

    private CoverImage[] coverImages = [];

    /// <summary>
    /// Gets or sets the cover images array.
    /// <para>This will also attempt to set <see cref="CleanEnabled"/> to true if the new cover images
    /// array has at least one element. Otherwise, <see cref="CleanEnabled"/> will be set to false.</para>
    /// </summary>
    public CoverImage[] CoverImages
    {
        get => coverImages;
        set
        {
            CleanEnabled = value.Length > 0;
            this.RaiseAndSetIfChanged(ref coverImages, value);
        }
    }

    private static Task CleanImagesAsync(IInputArguments arguments) => Task.Run(() => CleanImages(arguments));

    private static void CleanImages(IInputArguments arguments) => new ImageStore(arguments).CleanImages();

    private async void CleanImages()
    {
        var result = await MessageBoxManager.GetMessageBoxStandard(
            "Clean Images", "Cleaning these images will permanently remove any data store in them. Are you sure you want to continue?", ButtonEnum.OkCancel)
            .ShowAsync();
        if (result != ButtonResult.Ok)
        {
            return;
        }

        CleanEnabled = false;
        try
        {
            IInputArguments arguments = FormArgumentsForClean();
            await CleanImagesAsync(arguments);
        }
        catch (Exception e)
        {
            await MessageBoxManager.GetMessageBoxStandard("Clean Images Error", e.Message, ButtonEnum.Ok).ShowAsync();
        }
        finally
        {
            CleanEnabled = true;
        }
    }

    private IInputArguments FormArgumentsForClean()
    {
        string[] args = Arguments.FormCliArguments(new SettingsModel() { CoverImages = CoverImages });
        var parser = new CliParser();
        if (!parser.TryParseArgs(out StorageArguments arguments, args))
        {
            throw new Exception($"Could not parse input: [{parser.LastError.Message}]");
        }
        return arguments.ToCommonArguments();
    }
}