using Avalonia.Controls;
using Coachlight.Avalonia.Models;

namespace Coachlight.Avalonia.Building;

public sealed class TourBuilder
{
    private readonly string _id;
    private readonly List<TourStep> _steps = new List<TourStep>();
    private Action? _onCompleted;
    private Action? _onSkipped;

    private TourBuilder(string id)
    {
        _id = id;
    }

    public static TourBuilder Create(string id)
    {
        return new TourBuilder(id);
    }

    public TourBuilder Modal(Action<StepBuilder> configure)
    {
        return AddStep(new StepBuilder(null, null), configure);
    } 
    
    public TourBuilder Coachmark(string? targetId, Action<StepBuilder> configure)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(targetId);
        return AddStep(new StepBuilder(targetId, null), configure);
    } 
    
    public TourBuilder Coachmark(Func<Control?> target, Action<StepBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(target);
        return AddStep(new StepBuilder(null, target), configure);
    }

    public TourBuilder Step(TourStep step)
    {
        ArgumentNullException.ThrowIfNull(step);
        _steps.Add(step);
        return this;
    }
    
    public TourBuilder OnCompleted(Action handler) { _onCompleted = handler; return this; }
    public TourBuilder OnSkipped(Action handler) { _onSkipped = handler; return this; }

    public Tour Build()
    {
        if(!_steps.Any())
            throw new InvalidOperationException("Tour must have at least one step.");
        return new Tour(_id, _steps)
        {
            OnCompleted =  _onCompleted,
            OnSkipped =  _onSkipped
        };
    }

    private TourBuilder AddStep(StepBuilder builder, Action<StepBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        configure(builder);
        _steps.Add(builder.Build());
        return this;
    }
}