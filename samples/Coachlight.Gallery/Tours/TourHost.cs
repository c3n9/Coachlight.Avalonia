using System;
using Avalonia;
using Avalonia.Threading;
using Coachlight.Avalonia;              // StartTour extension
using Coachlight.Avalonia.Controller;
using Coachlight.Avalonia.Models;

namespace Coachlight.Gallery.Tours;

/// <summary>
/// Runs one tour at a time over a window and hands off between the per-page tours: it ends the
/// tour in progress, starts another page's tour, and lets the caller resume the first one at the
/// step it stopped on.
/// </summary>
public sealed class TourHost
{
    private TourController? _current;

    public TourHost(Visual anchor) => Anchor = anchor ?? throw new ArgumentNullException(nameof(anchor));

    /// <summary>The control the overlay is attached to. Page view models need it to look into their own visual tree.</summary>
    public Visual Anchor { get; }

    /// <summary>Whether a tour is currently on screen.</summary>
    public bool IsRunning => _current?.IsActive == true;

    /// <summary>
    /// Starts <paramref name="tour"/> at <paramref name="startIndex"/>. <paramref name="onEnd"/>
    /// runs when the user finishes or skips it — not when it is handed off to another tour.
    /// </summary>
    public void Run(Tour tour, int startIndex = 0, Action<TourEndReason>? onEnd = null)
    {
        ArgumentNullException.ThrowIfNull(tour);

        _current = Anchor.StartTour(tour, startIndex);
        _current.Ended += (_, reason) =>
        {
            // Stopped means this host swapped the tour out, which is nobody else's business.
            if (reason == TourEndReason.Stopped || onEnd is null)
                return;

            // onEnd usually starts the next tour, which attaches another overlay. Post it so
            // that happens on its own dispatcher turn rather than re-entrantly, while the
            // controller that just ended is still raising this event.
            Dispatcher.UIThread.Post(() => onEnd(reason));
        };
    }

    /// <summary>Ends the tour in progress and starts <paramref name="next"/> in its place.</summary>
    public void HandOff(Tour next, Action<TourEndReason> onNextEnd)
    {
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(onNextEnd);

        _current?.Stop();
        Run(next, onEnd: onNextEnd);
    }

    /// <summary>Index of the step targeting <paramref name="targetId"/>, or -1.</summary>
    public static int IndexOf(Tour tour, string targetId)
    {
        ArgumentNullException.ThrowIfNull(tour);
        for (var i = 0; i < tour.Steps.Count; i++)
            if (tour.Steps[i].TargetId == targetId)
                return i;
        return -1;
    }
}
