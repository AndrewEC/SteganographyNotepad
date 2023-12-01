namespace SteganographyNotepad.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using DynamicData;
using ReactiveUI;
using SteganographyNotepad.Models;

public class CoverImageSettingsViewModel : ReactiveObject
{
    private const int CharacterLimit = 30;

    public ReactiveCommand<Unit, Unit> SelectImagesClickCommand { get; }
    public ReactiveCommand<string, Unit> RemoveImageCommand { get; }
    public ReactiveCommand<string, Unit> MoveImageUpCommand { get; }
    public ReactiveCommand<string, Unit> MoveImageDownCommand { get; }

    public CoverImageSettingsViewModel()
    {
        SelectImagesClickCommand = ReactiveCommand.Create(SelectImagesClick);
        RemoveImageCommand = ReactiveCommand.Create<string>(RemoveImage);
        MoveImageUpCommand = ReactiveCommand.Create<string>(MoveImageUp);
        MoveImageDownCommand = ReactiveCommand.Create<string>(MoveImageDown);
    }

    private bool coverImageListVisible = false;
    public bool CoverImageListVisible
    {
        get => coverImageListVisible;
        set => this.RaiseAndSetIfChanged(ref coverImageListVisible, value);
    }

    private CoverImage[] coverImages = [];
    public CoverImage[] CoverImages
    {
        get => coverImages;
        set
        {
            this.RaiseAndSetIfChanged(ref coverImages, value);
            CoverImageListVisible = value.Length > 0;
        }
    }

    public async void SelectImagesClick()
    {
        string[] files = await SelectFiles();
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

    private async Task<string[]> SelectFiles()
    {
        Window? window = Lookup.GetMainWindow();
        if (window == null)
        {
            return [];
        }

        IReadOnlyList<IStorageFile> files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            AllowMultiple = true,
            FileTypeFilter = new List<FilePickerFileType> { LosslessImagePicker() }
        });
        return new List<IStorageFile>(files).Select(file => file.Path.AbsolutePath.ToString()).ToArray();
    }

    private static FilePickerFileType LosslessImagePicker() => new("Lossless Images")
    {
        Patterns = ["*.png", "*.webp"],
        AppleUniformTypeIdentifiers = ["public.image"],
        MimeTypes = ["image/*"]
    };

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

    private static string DetermineDisplayName(string path)
    {
        if (path.Length < CharacterLimit)
        {
            return path;
        }
        return string.Concat("...", path.AsSpan(path.Length - CharacterLimit));
    }
}