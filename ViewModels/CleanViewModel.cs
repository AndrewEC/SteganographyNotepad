namespace SteganographyNotepad.ViewModels;

using System;
using System.Reactive;
using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using ReactiveUI;
using SteganographyApp.Common.Arguments;
using SteganographyApp.Common.IO;
using SteganographyNotepad.Models;
using SteganographyNotepad.Store;

public class CleanViewModel : ReactiveObject
{
    public ReactiveCommand<Unit, Unit> CleanImagesCommand { get; }

    public CleanViewModel()
    {
        CleanImagesCommand = ReactiveCommand.Create(CleanImages);
    }

    private bool cleanEnabled = false;
    public bool CleanEnabled
    {
        get => cleanEnabled;
        set => this.RaiseAndSetIfChanged(ref cleanEnabled, value);
    }

    private CoverImage[] coverImages = [];
    public CoverImage[] CoverImages
    {
        get => coverImages;
        set
        {
            CleanEnabled = value.Length > 0;
            this.RaiseAndSetIfChanged(ref coverImages, value);
        }
    }

    private async void CleanImages()
    {
        var result = await MessageBoxManager.GetMessageBoxStandard("Clean Images",
            "Cleaning these images will permanently remove any data store in them. Are you sure you want to continue?", ButtonEnum.OkCancel)
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
        string[] args = Arguments.FormCliArguments(new SettingsModel(){ CoverImages = CoverImages });
        var parser = new CliParser();
        if (!parser.TryParseArgs(out StorageArguments arguments, args))
        {
            throw new Exception($"Could not parse input: [{parser.LastError.Message}]");
        }
        return arguments.ToCommonArguments();
    }

    private static Task CleanImagesAsync(IInputArguments arguments) => Task.Run(() => CleanImages(arguments));

    private static void CleanImages(IInputArguments arguments) => new ImageStore(arguments).CleanImages();
}