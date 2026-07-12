namespace Coachlight.Avalonia.Persistence;

public interface IProgressStore
{
    bool IsCompleted(string tourId);
    void MarkCompleted(string tourId);
    void Reset(string tourId);
}