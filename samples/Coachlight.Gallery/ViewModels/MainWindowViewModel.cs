using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Coachlight.Avalonia;              // StartTour extension
using Coachlight.Avalonia.Building;
using Coachlight.Avalonia.Enums;
using Coachlight.Avalonia.Models;       // TourLabels
using Coachlight.Avalonia.Targeting;    // Coachmark.GetId

namespace Coachlight.Gallery.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    // Backing state for the demo form controls the tour points at.
    [ObservableProperty] private string _userName = string.Empty;
    [ObservableProperty] private string _selectedLanguage = "English";
    [ObservableProperty] private bool _rememberMe;
    [ObservableProperty] private double _volume = 40;

    public ObservableCollection<string> Languages { get; } = new() { "English", "Russian", "German", "Spanish" };

    public ObservableCollection<string> RecentItems { get; } =
        new(Enumerable.Range(1, 20).Select(i => $"Project {i}"));

    // Auto-scroll demo shown while the "recent items" coachmark step is active.
    private DispatcherTimer? _listDemoTimer;
    private double _listDemoDirection = 1;

    /// <summary>
    /// Builds a demo tour and starts it over the window. The anchor (window) comes in as a
    /// CommandParameter, so the view model holds no direct reference to the view.
    /// </summary>
    [RelayCommand]
    private void StartTour(Visual? anchor)
    {
        if (anchor is null)
            return;

        var tour = TourBuilder.Create("demo")
            // Custom button captions (localization / wording). Omit to use the English defaults.
            .Labels(new TourLabels
            {
                Skip = "Skip tour",
                Back = "Previous",
                Next = "Continue",
                Done = "Got it",
            })
            .Modal(s => s
                .Title("Welcome!")
                .Text("A short tour of the interface, covering several different control types. Click “Continue”."))
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
            .Coachmark("volumeSlider", s => s
                .Placement(Side.Top)
                .Title("Volume")
                .Text("Sliders and other custom-shaped controls are highlighted just as precisely."))
            .Coachmark("recentItemsList", s => s
                .Placement(Side.Right)
                .Spotlight(padding: 4, radius: 12)
                .Title("Recent items")
                .Text("Even a scrollable list can be the target — watch it scroll by itself.")
                .OnEnter(() => StartListDemo(anchor))
                .OnExit(() => StopListDemo(anchor)))
            .Coachmark("btnConnect", s => s
                .Placement(Side.Right)
                .Title("Connect")
                .Text("This button connects to the robot."))
            .Coachmark("btnSettings", s => s
                .Placement(Side.Bottom)
                .Title("Settings")
                .Text("And here are the application settings."))
            .Build();

        anchor.StartTour(tour);
    }

    /// <summary>Finds a descendant control tagged with the given Coachmark.Id.</summary>
    private static T? FindById<T>(Visual root, string id) where T : Control =>
        root.GetVisualDescendants()
            .OfType<T>()
            .FirstOrDefault(c => Coachmark.GetId(c) == id);

    private void StartListDemo(Visual anchor)
    {
        var scrollViewer = FindById<ListBox>(anchor, "recentItemsList")?
            .GetVisualDescendants()
            .OfType<ScrollViewer>()
            .FirstOrDefault();
        if (scrollViewer is null)
            return;

        _listDemoDirection = 1;
        _listDemoTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(30) };
        _listDemoTimer.Tick += (_, _) =>
        {
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
        var scrollViewer = FindById<ListBox>(anchor, "recentItemsList")?
            .GetVisualDescendants()
            .OfType<ScrollViewer>()
            .FirstOrDefault();
        if (scrollViewer is not null)
            scrollViewer.Offset = new Vector(scrollViewer.Offset.X, 0);
    }
}
