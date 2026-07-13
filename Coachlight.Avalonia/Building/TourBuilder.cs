using Avalonia.Controls;
using Coachlight.Avalonia.Models;

namespace Coachlight.Avalonia.Building;

/// <summary>Fluent builder for a <see cref="Tour"/>. Start with <see cref="Create"/>.</summary>
public sealed class TourBuilder
{
    private readonly string _id;
    private readonly List<TourStep> _steps = new List<TourStep>();
    private Action? _onCompleted;
    private Action? _onSkipped;
    private TourLabels _labels = TourLabels.Defaults;

    private TourBuilder(string id)
    {
        _id = id;
    }

    /// <summary>Sets custom navigation button captions (Skip/Back/Next/Done). Defaults to English captions if not called.</summary>
    public TourBuilder Labels(TourLabels labels)
    {
        _labels = labels ?? TourLabels.Defaults;
        return this;
    }

    /// <summary>Starts building a tour identified by <paramref name="id"/> (used as the persistence key).</summary>
    public static TourBuilder Create(string id)
    {
        return new TourBuilder(id);
    }

    /// <summary>Adds a modal step: a centered card with no spotlighted target.</summary>
    public TourBuilder Modal(Action<StepBuilder> configure)
    {
        return AddStep(new StepBuilder(null, null), configure);
    }

    /// <summary>Adds a coachmark step targeting the control tagged with <c>Coachmark.Id="targetId"</c> in the visual tree.</summary>
    public TourBuilder Coachmark(string? targetId, Action<StepBuilder> configure)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(targetId);
        return AddStep(new StepBuilder(targetId, null), configure);
    }

    /// <summary>Adds a coachmark step targeting whatever control <paramref name="target"/> returns when the step is shown.</summary>
    public TourBuilder Coachmark(Func<Control?> target, Action<StepBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(target);
        return AddStep(new StepBuilder(null, target), configure);
    }

    /// <summary>Escape hatch: adds a fully pre-built <see cref="TourStep"/> as-is.</summary>
    public TourBuilder Step(TourStep step)
    {
        ArgumentNullException.ThrowIfNull(step);
        _steps.Add(step);
        return this;
    }

    /// <summary>Sets a callback invoked once when the tour is completed (the user reaches the end).</summary>
    public TourBuilder OnCompleted(Action handler) { _onCompleted = handler; return this; }

    /// <summary>Sets a callback invoked once when the tour is skipped by the user.</summary>
    public TourBuilder OnSkipped(Action handler) { _onSkipped = handler; return this; }

    /// <summary>Builds the immutable <see cref="Tour"/>. Throws if no steps were added.</summary>
    public Tour Build()
    {
        if(!_steps.Any())
            throw new InvalidOperationException("Tour must have at least one step.");
        return new Tour(_id, _steps)
        {
            OnCompleted =  _onCompleted,
            OnSkipped =  _onSkipped,
            Labels = _labels
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
