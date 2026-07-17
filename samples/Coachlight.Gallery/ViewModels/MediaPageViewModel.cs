using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Coachlight.Avalonia.Building;
using Coachlight.Avalonia.Enums;
using Coachlight.Avalonia.Models;
using Coachlight.Avalonia.Targeting;    // Coachmark.GetId
using Coachlight.Gallery.Tours;

namespace Coachlight.Gallery.ViewModels;

/// <summary>The second page. Its tour is handed the baton by the Profile tour, and passes it on to Actions.</summary>
public partial class MediaPageViewModel : ObservableObject
{
    /// <summary>Coachmark id of the button that hands the tour over to the Actions page.</summary>
    public const string OpenActionsTargetId = "openActionsBtn";

    [ObservableProperty] private double _volume = 40;

    public ObservableCollection<string> RecentItems { get; } =
        new(Enumerable.Range(1, 20).Select(i => $"Project {i}"));

    /// <summary>True only while the tour sits on the step that asks the user to press "Open Actions".</summary>
    public bool IsWaitingForActionsClick { get; private set; }

    /// <summary>Raised when the user presses "Open Actions".</summary>
    public event Action? ActionsRequested;

    // Auto-scroll demo shown while the "recent items" coachmark step is active.
    private DispatcherTimer? _listDemoTimer;
    private double _listDemoDirection = 1;

    [RelayCommand]
    private void OpenActions() => ActionsRequested?.Invoke();

    /// <summary>
    /// Builds this page's tour. As on Profile, the step targeting <see cref="OpenActionsTargetId"/>
    /// is the hand-off point and the steps after it are replayed on the way back.
    /// </summary>
    /// <param name="anchor">The window, used by the list auto-scroll demo to find its own ScrollViewer.</param>
    public Tour BuildTour(Visual anchor) =>
        TourBuilder.Create("media")
            .Labels(DemoLabels.Instance)
            .Modal(s => s
                .Title("The Media page")
                .Text("A different page, a different view model — and a tour of its own, started by the Profile tour."))
            .Coachmark("volumeSlider", s => s
                .Placement(Side.Right)
                .Title("Volume")
                .Text("Sliders and other custom-shaped controls are highlighted just as precisely."))
            .Coachmark("recentItemsList", s => s
                .Placement(Side.Right)
                .Spotlight(padding: 4, radius: 12)
                .Title("Recent items")
                .Text("Even a scrollable list can be the target — watch it scroll by itself.")
                .OnEnter(() => StartListDemo(anchor))
                .OnExit(() => StopListDemo(anchor)))
            .Coachmark(OpenActionsTargetId, s => s
                .Placement(Side.Right)
                .Title("One level deeper")
                .Text("Press this button and the Actions page takes over. Hand-offs nest: Actions comes back here, and this page comes back to Profile.")
                .OnEnter(() => IsWaitingForActionsClick = true)
                .OnExit(() => IsWaitingForActionsClick = false))
            // Everything below this line is the part replayed after the Actions tour finishes.
            .Coachmark("volumeSlider", s => s
                .Placement(Side.Right)
                .Title("Back on Media")
                .Text("Actions is done, so this page's tour picks up where it stopped. Press “Got it” to return to Profile."))
            .Build();

    /// <summary>Finds a descendant control tagged with the given Coachmark.Id.</summary>
    private static T? FindById<T>(Visual root, string id) where T : Control =>
        root.GetVisualDescendants()
            .OfType<T>()
            .FirstOrDefault(c => Coachmark.GetId(c) == id);

    private static ScrollViewer? FindRecentItemsScrollViewer(Visual anchor) =>
        FindById<ListBox>(anchor, "recentItemsList")?
            .GetVisualDescendants()
            .OfType<ScrollViewer>()
            .FirstOrDefault();

    private void StartListDemo(Visual anchor)
    {
        _listDemoDirection = 1;
        _listDemoTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(30) };
        _listDemoTimer.Tick += (_, _) =>
        {
            if (FindRecentItemsScrollViewer(anchor) is not { } scrollViewer)
                return;

            var max = scrollViewer.Extent.Height - scrollViewer.Viewport.Height;
            if (max <= 0)
                return;

            var y = scrollViewer.Offset.Y + _listDemoDirection * 2.5;
            if (y >= max) { y = max; _listDemoDirection = -1; }
            else if (y <= 0) { y = 0; _listDemoDirection = 1; }

            scrollViewer.Offset = new Vector(scrollViewer.Offset.X, y);
        };
        _listDemoTimer.Start();
    }

    private void StopListDemo(Visual anchor)
    {
        _listDemoTimer?.Stop();
        _listDemoTimer = null;

        // Reset scroll position so the list looks the same next time the tour runs.
        if (FindRecentItemsScrollViewer(anchor) is { } scrollViewer)
            scrollViewer.Offset = new Vector(scrollViewer.Offset.X, 0);
    }
}
