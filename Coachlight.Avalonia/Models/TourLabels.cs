namespace Coachlight.Avalonia.Models;

public sealed class TourLabels
{
    public string Skip { get; init; } = "Skip";
    public string Next { get; init; } = "Next";
    public string Back { get; init; } = "Back";
    public string Done { get; init; } = "Done";

    public static TourLabels Defaults { get; } = new TourLabels();
}