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
    public static void StartTour(this Visual anchor, Tour tour)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        ArgumentNullException.ThrowIfNull(tour);
        Run(anchor, tour, store: null);
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
    public static void StartTour(this Visual anchor, Tour tour, IProgressStore store, bool force = false)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        ArgumentNullException.ThrowIfNull(tour);
        ArgumentNullException.ThrowIfNull(store);

        if (!force && store.IsCompleted(tour.Id))
            return;

        Run(anchor, tour, store);
    }

    private static void Run(Visual anchor, Tour tour, IProgressStore? store)
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
        controller.Start(tour);
    }
}
