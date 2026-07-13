namespace Coachlight.Avalonia.Models;

/// <summary>An immutable tour: an ordered list of steps plus its identifier and end callbacks.</summary>
public sealed class Tour
{
    /// <summary>Unique key for this tour, used as the persistence key by <see cref="Persistence.IProgressStore"/>.</summary>
    public string Id { get; }

    /// <summary>The steps to show, in order.</summary>
    public IReadOnlyList<TourStep> Steps { get; }

    /// <summary>Invoked once when the user reaches the end of the tour.</summary>
    public Action? OnCompleted { get; init; }

    /// <summary>Invoked once when the user skips the tour.</summary>
    public Action? OnSkipped { get; init; }

    /// <summary>Navigation button captions. Defaults to English captions (<see cref="TourLabels.Defaults"/>).</summary>
    public TourLabels Labels { get; init; } = TourLabels.Defaults;

    /// <summary>Creates a tour. <paramref name="steps"/> is defensively copied.</summary>
    public Tour(string id, IReadOnlyList<TourStep> steps)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));
        }

        ArgumentNullException.ThrowIfNull(steps);
        if (steps.Any(s => s is null))
            throw new ArgumentException("Steps cannot contain null.", nameof(steps));

        Id = id;
        Steps = steps.ToList().AsReadOnly();
    }
}
