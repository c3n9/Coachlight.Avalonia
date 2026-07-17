using Coachlight.Avalonia.Models;

namespace Coachlight.Gallery.Tours;

/// <summary>Navigation button captions shared by every page's tour (localization / wording).</summary>
internal static class DemoLabels
{
    public static readonly TourLabels Instance = new TourLabels
    {
        Skip = "Skip tour",
        Back = "Previous",
        Next = "Continue",
        Done = "Got it",
    };
}
