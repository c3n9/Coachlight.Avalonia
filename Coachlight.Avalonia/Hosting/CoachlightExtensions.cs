using Avalonia;
using Avalonia.VisualTree;
using Coachlight.Avalonia.Controller;
using Coachlight.Avalonia.Controls;
using Coachlight.Avalonia.Models;
using Coachlight.Avalonia.Targeting;

namespace Coachlight.Avalonia;

public static class CoachlightExtensions
{
    public static void StartTour(this Visual anchor, Tour tour)
    {
        ArgumentNullException.ThrowIfNull(anchor);
        ArgumentNullException.ThrowIfNull(tour);
        var root = anchor.GetVisualRoot() as Visual ?? anchor;
        var resolver = new VisualTreeTargetResolver(root);
        var controller = new TourController(resolver);
        var overlay = new CoachmarkOverlay(controller);
        overlay.Attach(anchor);
        controller.Start(tour);
    }
}