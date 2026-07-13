namespace Coachlight.Avalonia.Models;

/// <summary>
/// Navigation button captions for a tour, for localization. Set via
/// <see cref="Building.TourBuilder.Labels"/>; omit to use <see cref="Defaults"/> (English).
/// </summary>
public sealed class TourLabels
{
    /// <summary>Caption of the "skip tour" button. Defaults to "Skip".</summary>
    public string Skip { get; init; } = "Skip";

    /// <summary>Caption of the "next step" button. Defaults to "Next".</summary>
    public string Next { get; init; } = "Next";

    /// <summary>Caption of the "previous step" button. Defaults to "Back".</summary>
    public string Back { get; init; } = "Back";

    /// <summary>Caption of the primary button on the last step (replaces <see cref="Next"/>). Defaults to "Done".</summary>
    public string Done { get; init; } = "Done";

    /// <summary>Shared instance of the default (English) captions.</summary>
    public static TourLabels Defaults { get; } = new TourLabels();
}
