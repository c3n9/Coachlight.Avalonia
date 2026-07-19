using Avalonia.Controls;
using Coachlight.Avalonia.Enums;

namespace Coachlight.Avalonia.Models;

/// <summary>
/// One step of a tour: what to highlight and what to show in the card. Immutable; built via
/// <see cref="Building.StepBuilder"/> (through <see cref="Building.TourBuilder"/>).
/// </summary>
public sealed class TourStep
{
    /// <summary>Id of the target control, resolved via <see cref="Controller.ITargetResolver"/>. Mutually exclusive with <see cref="TargetProvider"/> in practice; a provider takes priority if both are set.</summary>
    public string? TargetId { get; init; }

    /// <summary>Resolves the target control directly, called when the step is shown. Takes priority over <see cref="TargetId"/> if both are set.</summary>
    public Func<Control?>? TargetProvider { get; init; }

    /// <summary>Ids of several controls to spotlight at once (each gets its own hole). Resolved via <see cref="Controller.ITargetResolver"/>.</summary>
    public IReadOnlyList<string>? TargetIds { get; init; }

    /// <summary>Resolves several target controls directly, called when the step is shown. Nulls are ignored.</summary>
    public Func<IEnumerable<Control?>>? TargetsProvider { get; init; }

    /// <summary>Index of the resolved target the card anchors to. <c>-1</c> (default) anchors the card to the union of all holes.</summary>
    public int AnchorIndex { get; init; } = -1;

    /// <summary>The step title. Accepts a string or any object rendered via a <c>DataTemplate</c>.</summary>
    public object? Title { get; init; }

    /// <summary>The step body. Accepts a string or any object rendered via a <c>DataTemplate</c>.</summary>
    public object? Content { get; init; }

    /// <summary>Preferred side to place the card on relative to the target.</summary>
    public Side Placement { get; init; }

    /// <summary>Padding (px) between the target's bounds and the edge of the spotlight hole.</summary>
    public double SpotlightPadding { get; init; } = 8;

    /// <summary>Corner radius (px) of the spotlight hole.</summary>
    public double SpotlightRadius { get; init; } = 8;

    /// <summary>
    /// Whether the step is skipped when its target can't be resolved (id not in the visual tree,
    /// provider returned null). When <c>true</c> (the default) the tour moves on to the next
    /// showable step; when <c>false</c> the step is still shown, as a centered card. Has no effect
    /// on modal steps, which have no target.
    /// </summary>
    public bool SkipIfMissing { get; init; } = true;

    /// <summary>
    /// Whether the spotlight hole lets pointer input through to the target beneath it. When
    /// <c>true</c> (the default) the hole is "transparent" — the user can click the highlighted
    /// control directly. When <c>false</c> the dim captures clicks over the hole too, so the
    /// target can only be looked at, not interacted with.
    /// </summary>
    public bool AllowInteraction { get; init; } = true;

    /// <summary>Invoked when this step becomes active. Exceptions are swallowed so a broken demo cannot break tour navigation.</summary>
    public Action? OnEnter { get; init; }

    /// <summary>Invoked when this step is left. Exceptions are swallowed so a broken demo cannot break tour navigation.</summary>
    public Action? OnExit { get; init; }

    /// <summary>Whether this step has no target (neither <see cref="TargetId"/> nor <see cref="TargetProvider"/> set) and is shown as a centered modal card.</summary>
    public bool IsModal =>
        TargetId is null && TargetProvider is null &&
        (TargetIds is null || TargetIds.Count == 0) && TargetsProvider is null;
}
