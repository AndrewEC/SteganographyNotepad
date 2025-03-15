namespace SteganographyNotepad.ViewModels;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using ReactiveUI;
using SteganographyNotepad.Models;
using SteganographyNotepad.Views;

/// <summary>
/// The view model for <see cref="EditView"/>.
/// </summary>
[SuppressMessage("Ordering Rules", "SA1201", Justification = "Reviewed")]
public class EditViewModel : ReactiveObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditViewModel"/> class.
    /// </summary>
    /// <param name="appState">The root application state. This view model will listen
    /// for changes on the <see cref="AppStateProperties.IsActionEnabled"/>
    /// and <see cref="AppStateProperties.TextContent"/> properties.</param>
    /// <param name="saveClickCallback">The callback to invoke when the save button is
    /// clicked so text can be written to the cover images.</param>
    public EditViewModel(AppStateProperties appState, ReactiveCommand<Unit, Unit> saveClickCallback)
    {
        SaveTextCommand = saveClickCallback;
        this.appState = appState;
        isActionEnabled = appState.IsActionEnabled;
        appState.PropertyChanged += OnAppStateChanged;
    }

    private readonly AppStateProperties appState;

    /// <summary>
    /// Gets the command to trigger when the "Save" button is clicked.
    /// </summary>
    public ReactiveCommand<Unit, Unit> SaveTextCommand { get; }

    private bool isActionEnabled = true;

    /// <summary>
    /// Gets or sets a value indicating whether the "Save" button should be enabled.
    /// </summary>
    public bool IsActionEnabled
    {
        get => isActionEnabled;
        set
        {
            this.RaiseAndSetIfChanged(ref isActionEnabled, value);
        }
    }

    private string textContent = string.Empty;

    /// <summary>
    /// Gets or sets the value of the text the user has entered.
    /// </summary>
    public string TextContent
    {
        get => textContent;
        set
        {
            this.RaiseAndSetIfChanged(ref textContent, value);
            appState.TextContent = value;
        }
    }

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
            case nameof(appState.TextContent):
                TextContent = appState.TextContent;
                break;
        }
    }
}