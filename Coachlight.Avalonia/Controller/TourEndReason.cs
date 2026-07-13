namespace Coachlight.Avalonia.Controller;

/// <summary>Why a <see cref="TourController"/> raised its <see cref="TourController.Ended"/> event.</summary>
public enum TourEndReason
{
    /// <summary>The user navigated past the last showable step.</summary>
    Completed,

    /// <summary>The user explicitly skipped the tour.</summary>
    Skipped,

    /// <summary>The tour was stopped programmatically (e.g. <see cref="TourController.Stop"/>, or a new tour starting).</summary>
    Stopped
}
