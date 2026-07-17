using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Coachlight.Avalonia.Controller;   // TourEndReason
using Coachlight.Avalonia.Models;
using Coachlight.Gallery.Tours;

namespace Coachlight.Gallery.ViewModels;

/// <summary>
/// The shell: owns the pages and the navigation between them, and wires the per-page tours
/// together. Each page view model builds its own tour and knows nothing about the others; the
/// shell is the only place that decides when one tour hands off to the next.
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    private const int ProfilePage = 0;
    private const int MediaPage = 1;
    private const int ActionsPage = 2;

    [ObservableProperty] private int _selectedPageIndex;

    private TourHost? _host;
    private Tour? _profileTour;
    private Tour? _mediaTour;

    public ProfilePageViewModel Profile { get; } = new();
    public MediaPageViewModel Media { get; } = new();
    public ActionsPageViewModel Actions { get; } = new();

    public MainWindowViewModel()
    {
        Profile.MediaRequested += OnMediaRequested;
        Media.ActionsRequested += OnActionsRequested;
    }

    /// <summary>
    /// Starts the Profile tour over the window. The anchor comes in as a CommandParameter, so
    /// the view models hold no direct reference to the view.
    /// </summary>
    [RelayCommand]
    private void StartTour(Visual? anchor)
    {
        if (anchor is null)
            return;

        var host = _host = new TourHost(anchor);
        SelectedPageIndex = ProfilePage;
        _profileTour = Profile.BuildTour();
        host.Run(_profileTour);
    }

    // Pressing "Open Media" always navigates. If the Profile tour is waiting on exactly that
    // button, the press also hands the tour over to the Media page.
    private void OnMediaRequested()
    {
        SelectedPageIndex = MediaPage;

        if (_host is not { IsRunning: true } host || !Profile.IsWaitingForMediaClick)
            return;

        _mediaTour = Media.BuildTour(host.Anchor);
        host.HandOff(_mediaTour, OnMediaTourEnded);
    }

    // The same hand-off one level deeper: Actions returns to the Media tour, which in turn
    // still owes its own return to Profile.
    private void OnActionsRequested()
    {
        SelectedPageIndex = ActionsPage;

        if (_host is not { IsRunning: true } host || !Media.IsWaitingForActionsClick || _mediaTour is not { } parent)
            return;

        var resumeAt = TourHost.IndexOf(parent, MediaPageViewModel.OpenActionsTargetId) + 1;

        host.HandOff(Actions.BuildTour(), reason =>
        {
            SelectedPageIndex = MediaPage;
            if (reason == TourEndReason.Completed)
                host.Run(parent, resumeAt, OnMediaTourEnded);
        });
    }

    // The Media tour is the Profile tour's sub-tour, so its end — whether it ended directly or
    // after Actions handed control back to it — is what returns us to Profile.
    private void OnMediaTourEnded(TourEndReason reason)
    {
        SelectedPageIndex = ProfilePage;

        // Skipping is the user leaving the whole demo, so only a finished sub-tour resumes the
        // tour that started it.
        if (reason != TourEndReason.Completed || _host is not { } host || _profileTour is not { } parent)
            return;

        host.Run(parent, TourHost.IndexOf(parent, ProfilePageViewModel.OpenMediaTargetId) + 1);
    }
}
