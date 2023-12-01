namespace SteganographyNotepad.ViewModels;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

/// <summary>
/// Utility class to help get a reference to the main application window.
/// </summary>
public static class Lookup
{
    /// <summary>
    /// Attempts to return a reference to the main Window.
    /// </summary>
    /// <returns>A reference to the main app window if it can be found, otherwise null.</returns>
    public static Window? GetMainWindow()
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
