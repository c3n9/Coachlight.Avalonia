namespace Coachlight.Avalonia.Persistence;

/// <summary>
/// Stores which tours a user has already completed or skipped, so <c>StartTour</c> can show a
/// tour only once. Passing a store to <c>StartTour</c> is entirely opt-in — the library never
/// touches disk (or any other storage) unless you supply one.
/// </summary>
public interface IProgressStore
{
    /// <summary>Whether the tour with the given id has already been completed or skipped.</summary>
    bool IsCompleted(string tourId);

    /// <summary>Marks the tour as completed/skipped.</summary>
    void MarkCompleted(string tourId);

    /// <summary>Clears the completed/skipped state, so the tour will show again.</summary>
    void Reset(string tourId);
}
