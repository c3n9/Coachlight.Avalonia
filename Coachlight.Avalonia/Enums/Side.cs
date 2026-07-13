namespace Coachlight.Avalonia.Enums;

/// <summary>Preferred side to place a coachmark's card on, relative to its target.</summary>
public enum Side
{
    /// <summary>Pick whichever side fits (tries Bottom, Top, Right, Left in order).</summary>
    Auto,

    /// <summary>Above the target.</summary>
    Top,

    /// <summary>Below the target.</summary>
    Bottom,

    /// <summary>To the left of the target.</summary>
    Left,

    /// <summary>To the right of the target.</summary>
    Right
}
