using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Coachlight.Avalonia.Building;
using Coachlight.Avalonia.Enums;
using Coachlight.Avalonia.Models;
using Coachlight.Gallery.Tours;

namespace Coachlight.Gallery.ViewModels;

/// <summary>The first page, and the owner of its own tour — the one the demo starts and ends with.</summary>
public partial class ProfilePageViewModel : ObservableObject
{
    /// <summary>Coachmark id of the button that hands the tour over to the Media page.</summary>
    public const string OpenMediaTargetId = "openMediaBtn";

    [ObservableProperty] private string _userName = string.Empty;
    [ObservableProperty] private string _selectedLanguage = "English";
    [ObservableProperty] private bool _rememberMe;

    public ObservableCollection<string> Languages { get; } = new() { "English", "Russian", "German", "Spanish" };

    /// <summary>True only while the tour sits on the step that asks the user to press "Open Media".</summary>
    public bool IsWaitingForMediaClick { get; private set; }

    /// <summary>Raised when the user presses "Open Media" — the shell decides whether that is plain navigation or a tour hand-off.</summary>
    public event Action? MediaRequested;

    [RelayCommand]
    private void OpenMedia() => MediaRequested?.Invoke();

    /// <summary>
    /// Builds this page's tour. The step targeting <see cref="OpenMediaTargetId"/> is the
    /// hand-off point: everything after it is what the shell replays once the Media tour is done.
    /// </summary>
    public Tour BuildTour() =>
        TourBuilder.Create("profile")
            .Labels(DemoLabels.Instance)
            .Modal(s => s
                .Title("Welcome!")
                .Text("A short tour of the interface. Each page runs its own tour, and they hand off to each other."))
            .Coachmark("userNameBox", s => s
                .Placement(Side.Right)
                .Title("User name")
                .Text("A plain text field. Coachmarks work the same on any input control."))
            .Coachmark("languageCombo", s => s
                .Placement(Side.Right)
                .Title("Language")
                .Text("A drop-down selector — the spotlight follows its actual on-screen bounds."))
            .Coachmark("rememberMeCheck", s => s
                .Placement(Side.Right)
                .Title("Remember me")
                .Text("Checkboxes, toggles, radio buttons — any control can be a target."))
            .Coachmark(OpenMediaTargetId, s => s
                .Placement(Side.Right)
                .Title("Over to the Media page")
                .Text("Press this button — clicks reach the control through the spotlight. The Media page takes over with its own tour, and this one resumes right here afterwards.")
                .OnEnter(() => IsWaitingForMediaClick = true)
                .OnExit(() => IsWaitingForMediaClick = false))
            // Everything below this line is the part replayed after the Media tour finishes.
            .Coachmark("userNameBox", s => s
                .Placement(Side.Right)
                .Title("Back on Profile")
                .Text("The Media page is done, so we're back on this page's own tour — one step past where it stopped."))
            .Modal(s => s
                .Title("That's it")
                .Text("The tour ended on the page it started from. Press “Start tour” to run it again."))
            .Build();
}
