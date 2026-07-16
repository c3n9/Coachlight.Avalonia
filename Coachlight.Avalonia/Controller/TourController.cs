using Avalonia.Controls;
using Coachlight.Avalonia.Models;

namespace Coachlight.Avalonia.Controller;

/// <summary>
/// Drives a single <see cref="Tour"/>: tracks the current step, skips steps whose target is
/// hidden, resolves targets through an <see cref="ITargetResolver"/>, and raises events the
/// UI layer (the overlay) reacts to. Contains no Avalonia rendering logic of its own.
/// </summary>
public sealed class TourController
{
    private readonly ITargetResolver? _resolver;
    private Tour? _tour;
    private int _index = -1;
    private TourStep? _activeStep;

    /// <summary>Creates a controller. Pass <c>null</c> to disable id-based targeting (only <see cref="TourStep.TargetProvider"/> steps will resolve).</summary>
    public TourController(ITargetResolver? resolver = null)
    {
        _resolver = resolver;
    }

    /// <summary>Whether a tour is currently running.</summary>
    public bool IsActive => _tour is not null;

    /// <summary>The tour currently running, or <c>null</c> if none.</summary>
    public Tour? CurrentTour => _tour;

    /// <summary>Index of the current step within <see cref="CurrentTour"/>.</summary>
    public int CurrentIndex => _index;

    /// <summary>Total number of steps in the current tour (0 if inactive).</summary>
    public int StepCount => _tour?.Steps.Count ?? 0;

    /// <summary>The step currently shown, or <c>null</c> if inactive.</summary>
    public TourStep? CurrentStep =>
        _tour is not null && _index >= 0 && _index < _tour.Steps.Count ? _tour?.Steps[_index] : null;

    /// <summary>Whether there is no showable step before the current one (hides the "Back" button).</summary>
    public bool IsFirst => !HasShowableInRange(0, _index - 1);

    /// <summary>Whether there is no showable step after the current one (switches "Next" to "Done").</summary>
    public bool IsLast => !HasShowableInRange(_index + 1, StepCount - 1);

    /// <summary>The resolved target control of <see cref="CurrentStep"/>, or <c>null</c> for a modal step or an unresolved target.</summary>
    public Control? CurrentTarget => CurrentTargets.Count > 0 ? CurrentTargets[0] : null;

    /// <summary>All resolved target controls of <see cref="CurrentStep"/> (one spotlight hole each). Empty for a modal step or when nothing resolves.</summary>
    public IReadOnlyList<Control> CurrentTargets =>
        CurrentStep is { } step ? ResolveTargets(step) : Array.Empty<Control>();

    /// <summary>Raised whenever the current step changes (including to <c>null</c> when the tour ends).</summary>
    public event EventHandler<TourStep?>? CurrentStepChanged;

    /// <summary>Raised once when the tour ends, with the reason it ended.</summary>
    public event EventHandler<TourEndReason>? Ended;

    /// <summary>Starts <paramref name="tour"/>, stopping any tour already in progress.</summary>
    public void Start(Tour tour)
    {
        ArgumentNullException.ThrowIfNull(tour);
        if (IsActive) Stop();
        _tour = tour;
        _index = -1;
        _activeStep = null;
        ShowStepFrom(0, +1);
    }

    /// <summary>Advances to the next showable step, or completes the tour if none remain.</summary>
    public void Next()
    {
        if (IsActive) ShowStepFrom(_index + 1, +1);
    }

    /// <summary>Goes back to the previous showable step. No-op if already at the first step.</summary>
    public void Previous()
    {
        if (IsActive) ShowStepFrom(_index - 1, -1);
    }

    /// <summary>Ends the tour early, invoking <see cref="Tour.OnSkipped"/> and raising <see cref="Ended"/> with <see cref="TourEndReason.Skipped"/>.</summary>
    public void Skip()
    {
        if (!IsActive) return;
        var t = _tour;
        EndInternal();
        t?.OnSkipped?.Invoke();
        Ended?.Invoke(this, TourEndReason.Skipped);
    }

    /// <summary>Ends the tour programmatically, without invoking <see cref="Tour.OnCompleted"/> or <see cref="Tour.OnSkipped"/>.</summary>
    public void Stop()
    {
        if (!IsActive) return;
        EndInternal();
        Ended?.Invoke(this, TourEndReason.Stopped);
    }

    // Internal methods

    private void ShowStepFrom(int start, int direction)
    {
        if (_tour is null) return;

        for (int i = start; i >= 0 && i < StepCount; i += direction)
        {
            if (ShouldShow(_tour.Steps[i]))
            {
                ApplyStep(i);
                return;
            }
        }

        if (direction > 0)
            Complete();
    }

    private void ApplyStep(int index)
    {
        // A step's OnEnter/OnExit hook is caller-supplied code (live demos, opening a panel,
        // etc.) and must never be allowed to break tour navigation if it throws.
        try { _activeStep?.OnExit?.Invoke(); }
        catch { /* step hooks must not break navigation */ }

        _index = index;
        _activeStep = CurrentStep;

        try { _activeStep?.OnEnter?.Invoke(); }
        catch { /* step hooks must not break navigation */ }

        CurrentStepChanged?.Invoke(this, CurrentStep);
    }

    private void Complete()
    {
        var t = _tour;
        EndInternal();
        t?.OnCompleted?.Invoke();
        Ended?.Invoke(this, TourEndReason.Completed);
    }

    private void EndInternal()
    {
        try { _activeStep?.OnExit?.Invoke(); }
        catch { /* step hooks must not break navigation */ }

        _tour = null;
        _index = -1;
        _activeStep = null;
        CurrentStepChanged?.Invoke(this, null);
    }

    private bool ShouldShow(TourStep step)
    {
        if (step.IsModal) return true;
        var targets = ResolveTargets(step);
        if (targets.Count == 0) return true;                    
        return targets.Any(t => t.IsVisible && t.IsEffectivelyVisible);
    }

    private IReadOnlyList<Control> ResolveTargets(TourStep step)
    {
        if (step.TargetsProvider is not null)
            return step.TargetsProvider().Where(c => c is not null).Select(c => c!).ToList();

        if (step.TargetIds is { Count: > 0 })
            return step.TargetIds
                .Select(id => _resolver?.ResolveById(id))
                .Where(c => c is not null).Select(c => c!).ToList();

        if (step.TargetProvider is not null)
            return step.TargetProvider() is { } c ? new[] { c } : Array.Empty<Control>();

        if (step.TargetId is not null)
            return _resolver?.ResolveById(step.TargetId) is { } c ? new[] { c } : Array.Empty<Control>();

        return Array.Empty<Control>();
    }

    private bool HasShowableInRange(int from, int to)
    {
        if (_tour is null) return false;
        for (var i = from; i <= to && i < StepCount; i++)
            if (i >= 0 && ShouldShow(_tour.Steps[i]))
                return true;
        return false;
    }
}
