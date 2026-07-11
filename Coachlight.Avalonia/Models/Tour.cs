namespace Coachlight.Avalonia.Models;

/// <summary>
/// The sequence of steps and its identifier.
/// </summary>
public class Tour
{
    public string Id { get; }
    public IReadOnlyList<TourStep> Steps { get; }
    public Action? OnCompleted { get; init; }
    public Action? OnSkipped { get; init; }

    public Tour(string id, IReadOnlyList<TourStep> steps)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException($"'{nameof(id)}' cannot be null or whitespace.", nameof(id));
        }

        if (steps is null || steps.Any(s => s is null))
        {
            throw new ArgumentNullException($"'{nameof(steps)}' cannot be null.", nameof(steps));
        }
        
        Id = id;
        Steps = steps.ToList().AsReadOnly();
    }
}