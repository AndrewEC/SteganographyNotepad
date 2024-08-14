namespace SteganographyNotepad.ViewModels;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive;
using DynamicData;
using ReactiveUI;
using SteganographyNotepad.Models;
using SteganographyNotepad.Store;
using SteganographyNotepad.Views;

/// <summary>
/// The view model for <see cref="CoverImageSettingsView"/>.
/// </summary>
[SuppressMessage("Ordering Rules", "SA1201", Justification = "Useless Rule")]
public class CoverImageSettingsViewModel : ReactiveObject
{
    private const int CharacterLimit = 30;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoverImageSettingsViewModel"/> class.
    /// </summary>
    public CoverImageSettingsViewModel()
    {
        SelectImagesClickCommand = ReactiveCommand.Create(SelectImagesClick);
        RemoveImageCommand = ReactiveCommand.Create<string>(RemoveImage);
        MoveImageUpCommand = ReactiveCommand.Create<string>(MoveImageUp);
        MoveImageDownCommand = ReactiveCommand.Create<string>(MoveImageDown);
    }

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

    private async void SelectImagesClick()
    {
        string[] files = await LosslessImagePicker.PickImages();
        if (files.Length == 0)
        {
            return;
        }

        var selectedImages = files.Select(file => new CoverImage(file, DetermineDisplayName(file))).ToArray();
        if (CoverImages.Length == 0)
        {
            CoverImages = selectedImages;
            return;
        }

        var existingImagePaths = CoverImages.Select(image => image.Path).ToList();
        var nextCoverImages = selectedImages.Where(image => !existingImagePaths.Contains(image.Path)).ToList();
        nextCoverImages.AddRange(CoverImages);
        CoverImages = nextCoverImages.ToArray();
    }

    private void MoveImage(CoverImage coverImage, SwapDirection direction, Predicate<int> canMove)
    {
        int position = CoverImages.IndexOf(coverImage);
        if (!canMove(position))
        {
            return;
        }
        var toUpdate = (CoverImage[])CoverImages.Clone();
        var temp = toUpdate[position];
        toUpdate[position] = toUpdate[position + (1 * (int)direction)];
        toUpdate[position + (1 * (int)direction)] = temp;
        CoverImages = toUpdate;
    }

    private void RemoveImage(string path)
    {
        var imageToRemove = FindImageWithPath(path);
        CoverImages = CoverImages.Where(image => image != imageToRemove).ToArray();
    }

    private void MoveImageUp(string path)
    {
        var imageToMove = FindImageWithPath(path);
        MoveImage(imageToMove, SwapDirection.Up, position => position != -1 && position > 0);
    }

    private void MoveImageDown(string path)
    {
        var imageToMove = FindImageWithPath(path);
        MoveImage(imageToMove, SwapDirection.Down, position => position != -1 && position < CoverImages.Length - 1);
    }

    private CoverImage FindImageWithPath(string path) => CoverImages.Where(image => image.Path == path).FirstOrDefault()
        ?? throw new Exception($"Could not find cover image with path: [{path}]");
}