using Avalonia.Controls;
using Coachlight.Avalonia.Enums;


namespace Coachlight.Avalonia.Models;

/// <summary>
/// One step of the tour: what to highlight and what to show in the card.
/// </summary>
public sealed class TourStep
{
    // Two ways for target control
    public string? TargetId { get; init; }
    public Func<Control?>? TargetProvider { get; init; }
    
    // Card content
    public object? Title {get; init; }
    public object? Content {get; init; }
    
    // Placement
    public Side Placement { get; init; }
    public double SpotlightPadding { get; init; } = 8;
    public double SpotlightRadius { get; init; } = 8;
    
    // Lifecycle hooks
    public Action? OnEnter { get; init; }
    public Action? OnExit { get; init; }
    
    // If there is no goal, we show it in the center.
    public bool IsModal => TargetId is not null && TargetProvider is not null;
}