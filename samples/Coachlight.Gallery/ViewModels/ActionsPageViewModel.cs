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
    [ObservableProperty] private string _status = "Idle";

    [RelayCommand]
    private void Connect() => Status = "Connected";

    public Tour BuildTour() =>
        TourBuilder.Create("actions")
            .Labels(DemoLabels.Instance)
            .Modal(s => s
                .Title("The Actions page")
                .Text("The third view model, with the third tour. Finish it and the Media tour resumes on its own."))
            .Coachmark("btnConnect", s => s
                .Placement(Side.Left)
                .Title("Connect")
                .Text("This button connects to the robot — up in the top-right corner."))
            .Coachmark("btnSettings", s => s
                .Placement(Side.Left)
                .Title("Settings")
                .Text("And the application settings — down in the bottom-right corner."))
            .Coachmark(new[] { "btnConnect", "btnSettings" }, s => s
                .Placement(Side.Left)
                .Title("Both buttons")
                .Text("One coachmark can spotlight several controls at once, even in different corners — each gets its own hole, and the card sits beside the whole group."))
            .Build();
}
