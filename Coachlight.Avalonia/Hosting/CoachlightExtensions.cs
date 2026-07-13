using Avalonia;
using Avalonia.VisualTree;
using Coachlight.Avalonia.Controller;
using Coachlight.Avalonia.Controls;
using Coachlight.Avalonia.Models;
using Coachlight.Avalonia.Persistence;
using Coachlight.Avalonia.Targeting;

namespace Coachlight.Avalonia;

public static class CoachlightExtensions
{
    public static void StartTour(this Visual anchor, Tour tour)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        ArgumentNullException.ThrowIfNull(tour);
        Run(anchor, tour, store: null);
    }
    
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
        var root = anchor.GetVisualRoot() as Visual ?? anchor;
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