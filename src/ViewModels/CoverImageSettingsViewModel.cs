namespace SteganographyNotepad.ViewModels;

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive;
using DynamicData;
using ReactiveUI;
using SteganographyNotepad.Log;
using SteganographyNotepad.Models;
using SteganographyNotepad.Store;
using SteganographyNotepad.Views;

/// <summary>
/// The view model for <see cref="CoverImageSettingsView"/>.
/// </summary>
[SuppressMessage("Ordering Rules", "SA1201", Justification = "Reviewed")]
public class CoverImageSettingsViewModel : ReactiveObject
{
    private const int CharacterLimit = 30;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoverImageSettingsViewModel"/> class.
    /// </summary>
    /// <param name="appState">The root application state. This view model will listen
    /// for changes on the <see cref="AppStateProperties.CoverImages"/> property.</param>
    public CoverImageSettingsViewModel(AppStateProperties appState)
    {
        this.appState = appState;
        appState.PropertyChanged += OnAppStateChanged;

        SelectImagesClickCommand = ReactiveCommand.Create(SelectImagesClick);
        RemoveImageCommand = ReactiveCommand.Create<string>(RemoveImage);
        MoveImageUpCommand = ReactiveCommand.Create<string>(MoveImageUp);
        MoveImageDownCommand = ReactiveCommand.Create<string>(MoveImageDown);
    }

    private readonly ConsoleLogger<CoverImageSettingsViewModel> logger = new();

    private readonly AppStateProperties appState;

    /// <summary>
    /// Gets the command triggered by the click of the Select Images button that initiates
    /// the dialog where the user can select images from the file explorer.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SelectImagesClickCommand { get; }

    /// <summary>
    /// Gets the command triggered by the click of the Remove Image button that will attempt
    /// to remove the associated image from the current array of cover images.
    /// </summary>
    public ReactiveCommand<string, Unit> RemoveImageCommand { get; }

    /// <summary>
    /// Gets the command triggered by the click of the Move Up button that will
    /// attempt to move the image up towards the beginning of the array of cover images.
    /// </summary>
    public ReactiveCommand<string, Unit> MoveImageUpCommand { get; }

    /// <summary>
    /// Gets the command triggered by the click of the Move Down button that will
    /// attempt to move the image down towards the end of the array of cover images.
    /// </summary>
    public ReactiveCommand<string, Unit> MoveImageDownCommand { get; }

    private bool coverImageListVisible = false;

    /// <summary>
    /// Gets or sets a value indicating whether the list element containing the list of cover
    /// images should be displayed.
    /// </summary>
    public bool CoverImageListVisible
    {
        get => coverImageListVisible;
        set => this.RaiseAndSetIfChanged(ref coverImageListVisible, value);
    }

    private CoverImage[] coverImages = [];

    /// <summary>
    /// Gets or sets the cover images array.
    /// <para> Upon setting the cover images this will also
    /// attempt to change the cover <see cref="CoverImageListVisible"/> option to true
    /// if there is more than one cover image in the new array otherwise false. </para>
    /// </summary>
    public CoverImage[] CoverImages
    {
        get => coverImages;
        set
        {
            this.RaiseAndSetIfChanged(ref coverImages, value);
            CoverImageListVisible = value.Length > 0;
        }
    }

    private static string DetermineDisplayName(string path)
    {
        if (path.Length < CharacterLimit)
        {
            return path;
        }

        return string.Concat("...", path.AsSpan(path.Length - CharacterLimit));
    }

    private void OnAppStateChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender != appState)
        {
            return;
        }

        if (e.PropertyName == nameof(appState.CoverImages))
        {
            CoverImages = appState.CoverImages;
        }
    }

    private async void SelectImagesClick()
    {
        logger.Log("Presenting image picker dialog.");
        string[] files = await LosslessImagePicker.PickImages();
        if (files.Length == 0)
        {
            return;
        }

        var selectedImages = files.Select(file => new CoverImage(file, DetermineDisplayName(file))).ToArray();
        logger.Log($"User selected [{selectedImages.Length}] images.");
        if (CoverImages.Length == 0)
        {
            appState.CoverImages = selectedImages;
            return;
        }

        var existingImagePaths = CoverImages.Select(image => image.Path).ToList();
        var nextCoverImages = selectedImages.Where(image => !existingImagePaths.Contains(image.Path)).ToList();
        nextCoverImages.AddRange(CoverImages);
        appState.CoverImages = nextCoverImages.ToArray();
    }

    private void MoveImage(string imagePath, Func<int, int> indexMapper)
    {
        CoverImage? image = FindImageWithPath(imagePath);
        if (image == null)
        {
            return;
        }

        int index = CoverImages.IndexOf(image);
        int nextIndex = indexMapper.Invoke(index);
        logger.Log($"Moving image [{imagePath}] from index [{index}] to [{nextIndex}]");
        if (nextIndex < 0 || nextIndex >= CoverImages.Length)
        {
            logger.Log("Image cannot be moved because the next index is outside the image array bounds.");
            return;
        }

        var toUpdate = (CoverImage[])CoverImages.Clone();
        (toUpdate[nextIndex], toUpdate[index]) = (toUpdate[index], toUpdate[nextIndex]);
        appState.CoverImages = toUpdate;
    }

    private void RemoveImage(string path)
    {
        logger.Log($"Removing image at path [{path}]");
        var imageToRemove = FindImageWithPath(path);

        if (imageToRemove == null)
        {
            return;
        }

        appState.CoverImages = CoverImages.Where(image => image != imageToRemove).ToArray();
    }

    private void MoveImageUp(string path) => MoveImage(path, index => index - 1);

    private void MoveImageDown(string path) => MoveImage(path, index => index + 1);

    private CoverImage? FindImageWithPath(string path)
    {
        CoverImage? image = CoverImages.Where(image => image.Path == path).FirstOrDefault();
        if (image == null)
        {
            logger.Log($"Could not find cover image with path: [{path}]");
            return null;
        }

        return image;
    }
}