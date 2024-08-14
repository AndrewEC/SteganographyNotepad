namespace SteganographyNotepad.Store;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

/// <summary>
/// Static utility class to assist in allowing the user to select a series of
/// lossless image files.
/// </summary>
internal static class LosslessImagePicker
{
    /// <summary>
    /// Presents a dialog by which a user can pick a series of lossless images on disk
    /// and returns the paths to the files selected.
    /// This will only ensure that the extension of the file matches a compatible image format
    /// but will not actually check that the file selected is actually a supported image.
    /// </summary>
    /// <returns>The array of paths representing the lossless images selected or an empty array
    /// if the user cancelled the dialog.</returns>
    public static async Task<string[]> PickImages()
    {
        Window? window = GetMainWindow();
        if (window == null)
        {
            return [];
        }

        IStorageFolder? startLocation = await window.StorageProvider.TryGetWellKnownFolderAsync(WellKnownFolder.Downloads);

        IReadOnlyList<IStorageFile> files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            AllowMultiple = true,
            FileTypeFilter = new List<FilePickerFileType> { LosslessFilePickerTypes() },
            SuggestedStartLocation = startLocation,
        });
        return new List<IStorageFile>(files).Select(file => file.Path.AbsolutePath.ToString()).ToArray();
    }

    private static FilePickerFileType LosslessFilePickerTypes() => new("Lossless Images")
    {
        Patterns = ["*.png", "*.webp"],
        AppleUniformTypeIdentifiers = ["public.image"],
        MimeTypes = ["image/*"],
    };

    private static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (desktop.MainWindow is Window window)
            {
                return window;
            }
        }
        return null;
    }
}