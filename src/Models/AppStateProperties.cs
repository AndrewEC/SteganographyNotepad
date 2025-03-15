namespace SteganographyNotepad.Models;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using SteganographyNotepad.Log;

/// <summary>
/// A global container for the shared properties of the app.
/// </summary>
[SuppressMessage("Ordering Rules", "SA1201", Justification = "Reviewed.")]
public sealed class AppStateProperties : INotifyPropertyChanged
{
    private readonly ConsoleLogger<AppStateProperties> logger = new();

    private bool isActionEnabled = false;

    /// <summary>
    /// Gets or sets a value indicating whether any button that affects the state
    /// of the cover images should be enabled.
    /// </summary>
    public bool IsActionEnabled
    {
        get => isActionEnabled;
        set => UpdateIfChanged(nameof(IsActionEnabled), ref isActionEnabled, value);
    }

    private int selectedTabIndex = 0;

    /// <summary>
    /// Gets or sets the index of the currently selected tab in the tab control.
    /// </summary>
    public int SelectedTabIndex
    {
        get => selectedTabIndex;
        set => UpdateIfChanged(nameof(SelectedTabIndex), ref selectedTabIndex, value);
    }

    private string textContent = string.Empty;

    /// <summary>
    /// Gets or sets the plain text content the user has been entered into the main
    /// edit area or has been loaded from one of the images.
    /// </summary>
    public string TextContent
    {
        get => textContent;
        set => UpdateIfChanged(nameof(TextContent), ref textContent, value);
    }

    private CoverImage[] coverImages = [];

    /// <summary>
    /// Gets or sets the array of images where text data will be loaded or written to.
    /// </summary>
    public CoverImage[] CoverImages
    {
        get => coverImages;
        set => UpdateIfChanged(nameof(CoverImages), ref coverImages, value);
    }

#pragma warning disable SA1201
    /// <summary>
    /// An event handler to emit when properties of this global state
    /// have been changed.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore SA1201

    private void UpdateIfChanged<T>(string propName, ref T currentValue, T nextValue)
    {
        if ((currentValue == null && nextValue == null) || (currentValue?.Equals(nextValue) ?? false))
        {
            return;
        }

        logger.Log($"Property [{propName}] changed from [{currentValue}] to [{nextValue}].");

        currentValue = nextValue;
        PropertyChanged?.Invoke(this, new(propName));
    }
}