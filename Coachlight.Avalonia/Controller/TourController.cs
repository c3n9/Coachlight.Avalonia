using Avalonia.Controls;
using Coachlight.Avalonia.Models;

namespace Coachlight.Avalonia.Controller;

public sealed class TourController
{
    private readonly ITargetResolver? _resolver;
    private Tour? _tour;
    private int _index = -1;
    private TourStep? _activeStep;

    public TourController(ITargetResolver? resolver = null)
    {
        _resolver = resolver;
    }

    public bool IsActive => _tour is not null;
    public Tour? CurrentTour => _tour;
    public int CurrentIndex => _index;
    public int StepCount => _tour?.Steps.Count ?? 0;

    public TourStep? CurrentStep =>
        _tour is not null && _index >= 0 && _index < _tour.Steps.Count ? _tour?.Steps[_index] : null;

    public bool IsFirst => !HasShowableInRange(0, _index - 1);
    public bool IsLast => !HasShowableInRange(_index + 1, StepCount - 1);
    public Control? CurrentTarget => CurrentStep is { } step ? ResolveTarget(step) : null;
    
    public event EventHandler<TourStep?>? CurrentStepChanged;
    public event EventHandler? Ended;

    public void Start(Tour tour)
    {
        ArgumentNullException.ThrowIfNull(tour);
        if (IsActive) Stop();
        _tour = tour;
        _index = -1;
        _activeStep = null;
        ShowStepFrom(0, +1);
    }

    public void Next()
    {
        if (IsActive) ShowStepFrom(_index + 1, +1);
    }

    public void Previous()
    {
        if (IsActive) ShowStepFrom(_index - 1, -1);
    }

    public void Skip()
    {
        if (!IsActive) return;
        var t = _tour;
        EndInternal();
        t?.OnSkipped?.Invoke();
        Ended?.Invoke(this, EventArgs.Empty);
    }

    public void Stop()
    {
        if (!IsActive) return;
        EndInternal();
        Ended?.Invoke(this, EventArgs.Empty);
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
        try
        {
            _activeStep?.OnExit?.Invoke();
        }
        catch (Exception ex)
        {
            // TODO: StepError
        }

        _index = index;
        _activeStep = CurrentStep;
        try
        {
            _activeStep?.OnEnter?.Invoke();
        }
        catch (Exception ex)
        {
            // TODO: StepError
        }

        CurrentStepChanged?.Invoke(this, CurrentStep);
    }

    private void Complete()
    {
        var t = _tour;
        EndInternal();
        t?.OnCompleted?.Invoke();
        Ended?.Invoke(this, EventArgs.Empty);
    }

    private void EndInternal()
    {
        try
        {
            _activeStep?.OnExit?.Invoke();
        }
        catch (Exception ex)
        {
            // TODO: StepError
        }

        _tour = null;
        _index = -1;
        _activeStep = null;
        CurrentStepChanged?.Invoke(this, null);
    }

    private bool ShouldShow(TourStep step)
    {
        var target = ResolveTarget(step);
        if (target is null) return true;
        return target.IsVisible && target.IsEffectivelyVisible;
    }

    private Control? ResolveTarget(TourStep step)
    {
        if (step.TargetProvider is not null) return step.TargetProvider();
        if (step.TargetId is not null) return _resolver?.ResolveById(step.TargetId);
        return null;
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