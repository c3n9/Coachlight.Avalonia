using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Coachlight.Avalonia.Building;
using Coachlight.Avalonia.Enums;
using Coachlight.Avalonia.Models;
using Coachlight.Gallery.Tours;

namespace Coachlight.Gallery.ViewModels;

/// <summary>The last page in the chain. Its tour is started by the Media tour and returns to it when finished.</summary>
public partial class ActionsPageViewModel : ObservableObject
{
    // Where the "press me" button lands on each press. The last press hides it instead.
    private static readonly Thickness[] MoveStops =
    {
        new Thickness(0, 150, 0, 0),
        new Thickness(300, 210, 0, 0),
        new Thickness(60, 330, 0, 0),
        new Thickness(360, 90, 0, 0),
    };

    [ObservableProperty] private string _status = "Idle";
    [ObservableProperty] private Thickness _movePosition = MoveStops[0];
    [ObservableProperty] private bool _isMoveTargetVisible = true;
    [ObservableProperty] private bool _isVanishTargetVisible = true;

    private int _moveStop;

    private void ShowVanishTarget() => IsVanishTargetVisible = true;
    private void HideVanishTarget() => IsVanishTargetVisible = false;

    [RelayCommand]
    private void Connect() => Status = "Connected";

    /// <summary>Hops the button to its next spot, and hides it once it runs out of spots.</summary>
    [RelayCommand]
    private void Move()
    {
        _moveStop++;
        if (_moveStop < MoveStops.Length)
            MovePosition = MoveStops[_moveStop];
        else
            IsMoveTargetVisible = false;
    }

    /// <summary>Puts the button back, so the step behaves the same every time the tour runs.</summary>
    private void ResetMoveTarget()
    {
        _moveStop = 0;
        MovePosition = MoveStops[0];
        IsMoveTargetVisible = true;
    }

    public Tour BuildTour() =>
        TourBuilder.Create("actions")
            .Labels(DemoLabels.Instance)
            .Modal(s => s
                .Title("The Actions page")
                .Text("The third view model, with the third tour. Finish it and the Media tour resumes on its own."))
            .Coachmark("btnVanish", s => s
                .Placement(Side.Right)
                .Interactive(false)
                .OnEnter(ShowVanishTarget)
                .Title("Look, don't touch")
                .Text("Interactive(false) makes the overlay swallow clicks over the hole, so this button can't be pressed. Press Next — it's about to disappear."))
            .Modal(s => s
                .OnEnter(HideVanishTarget)
                .Title("...and it's gone")
                .Text("That button just set IsVisible=false. The very next step targets it — watch the tour skip straight past it."))
            .Coachmark("btnVanish", s => s
                //.SkipIfMissing(false)
                .Title("Kept, even though it's gone")
                .Text("Its target is hidden now. By default this step would be skipped, but SkipIfMissing(false) keeps it — shown as a centered card since there's nothing left to point at."))
            .Coachmark("btnConnect", s => s
                .Placement(Side.Left)
                .Title("Connect")
                .Text("This button connects to the robot — up in the top-right corner."))
            .Coachmark("btnSettings", s => s
                .Placement(Side.Left)
                .Title("Settings")
                .Text("And the application settings — down in the bottom-right corner."))
            .Coachmark("btnMove", s => s
                .Placement(Side.Right)
                .Title("The hole follows its target")
                .Text("Press the button — it hops somewhere else on every press and the spotlight keeps up. Press it once more than it has places to go and it disappears: with nothing left to point at, the card falls back to the centre.")
                .OnEnter(ResetMoveTarget))
            .Coachmark(new[] { "btnConnect", "btnSettings" }, s => s
                .Placement(Side.Left)
                .Title("Both buttons")
                .Text("One coachmark can spotlight several controls at once, even in different corners — each gets its own hole, and the card sits beside the whole group."))
            .Build();
}
