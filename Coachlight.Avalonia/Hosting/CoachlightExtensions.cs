using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Coachlight.Avalonia.Controller;
using Coachlight.Avalonia.Controls;
using Coachlight.Avalonia.Models;
using Coachlight.Avalonia.Persistence;
using Coachlight.Avalonia.Targeting;

namespace Coachlight.Avalonia;

/// <summary>Entry points for starting a <see cref="Tour"/> over a window.</summary>
public static class CoachlightExtensions
{
    /// <summary>
    /// Starts <paramref name="tour"/> over the window containing <paramref name="anchor"/>.
    /// Shows the tour every time it is called; use the overload with an
    /// <see cref="IProgressStore"/> for "show once" behavior.
    /// </summary>
    /// <param name="anchor">Any control already attached to the target window; used to locate the overlay layer and the visual tree to search for targets.</param>
    /// <param name="tour">The tour to run, typically built with <see cref="Building.TourBuilder"/>.</param>
    /// <param name="startIndex">Step to start at. Defaults to the beginning; pass a later step to resume an interrupted tour without losing its step numbering.</param>
    /// <returns>The controller driving the tour — use it to observe or end the tour it started.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is negative, or past the end of <paramref name="tour"/>.</exception>
    public static TourController StartTour(this Visual anchor, Tour tour, int startIndex = 0)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        ArgumentNullException.ThrowIfNull(tour);
        return Run(anchor, tour, store: null, startIndex);
    }

    /// <summary>
    /// Starts <paramref name="tour"/> with progress persistence: by default it only shows once
    /// per <see cref="Tour.Id"/> (tracked via <paramref name="store"/>); pass
    /// <paramref name="force"/> = <c>true</c> to always show it (e.g. from a "?" help button).
    /// </summary>
    /// <param name="anchor">Any control already attached to the target window.</param>
    /// <param name="tour">The tour to run.</param>
    /// <param name="store">Where "completed" state is persisted, e.g. a <see cref="JsonProgressStore"/>.</param>
    /// <param name="force">If <c>true</c>, shows the tour even if <paramref name="store"/> reports it as completed.</param>
    /// <param name="startIndex">Step to start at. Defaults to the beginning; pass a later step to resume an interrupted tour without losing its step numbering.</param>
    /// <returns>The controller driving the tour, or <c>null</c> if the tour was not shown because <paramref name="store"/> reports it as completed.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="startIndex"/> is negative, or past the end of <paramref name="tour"/>.</exception>
    public static TourController? StartTour(this Visual anchor, Tour tour, IProgressStore store, bool force = false, int startIndex = 0)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        ArgumentNullException.ThrowIfNull(tour);
        ArgumentNullException.ThrowIfNull(store);

        if (!force && store.IsCompleted(tour.Id))
            return null;

        return Run(anchor, tour, store, startIndex);
    }

    private static TourController Run(Visual anchor, Tour tour, IProgressStore? store, int startIndex)
    {
        var root = TopLevel.GetTopLevel(anchor) as Visual ?? anchor;
        var resolver = new VisualTreeTargetResolver(root);
        var controller = new TourController(resolver);

        if (store is not null)
        {
            controller.Ended += (_, reason) =>
            {
                if (reason is TourEndReason.Completed or TourEndReason.Skipped)
                    store.MarkCompleted(tour.Id);
            };
        }

        var overlay = new CoachmarkOverlay(controller);
        overlay.Attach(anchor);
        controller.Start(tour, startIndex);
        return controller;
    }
}
