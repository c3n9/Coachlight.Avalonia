using Avalonia.Controls;
using Coachlight.Avalonia.Enums;
using Coachlight.Avalonia.Models;

namespace Coachlight.Avalonia.Building;

/// <summary>
/// Fluent builder for a single tour step. Instances are created by <see cref="TourBuilder"/>
/// (via <see cref="TourBuilder.Modal"/> / <see cref="TourBuilder.Coachmark(string?, System.Action{StepBuilder})"/>),
/// which already supplies the target — this builder only configures content and behavior.
/// </summary>
public sealed class StepBuilder
{
    private readonly string? _targetId;
    private readonly Func<Control?>? _targetProvider;
    private readonly IReadOnlyList<string>? _targetIds;
    private readonly Func<IEnumerable<Control?>>? _targetsProvider;

    private object? _title;
    private object? _content;
    private Side _placement = Side.Auto;
    private double _padding = 8;
    private double _radius = 8;
    private bool _allowInteraction = true;
    private bool _skipIfMissing = true;
    private int _anchorIndex = -1;
    private Action? _onEnter;
    private Action? _onExit;

    internal StepBuilder(string? targetId, Func<Control?>? targetProvider)
    {
        _targetId = targetId;
        _targetProvider = targetProvider;
    }

    internal StepBuilder(IReadOnlyList<string>? targetIds, Func<IEnumerable<Control?>>? targetsProvider)
    {
        _targetIds = targetIds;
        _targetsProvider = targetsProvider;
    }

    /// <summary>Sets the step title. Accepts a string or any object rendered via a <c>DataTemplate</c>.</summary>
    public StepBuilder Title(object title) { _title = title; return this; }

    /// <summary>Alias for <see cref="Content"/>.</summary>
    public StepBuilder Text(object text) => Content(text);

    /// <summary>Sets the step body. Accepts a string or any object rendered via a <c>DataTemplate</c>.</summary>
    public StepBuilder Content(object content) { _content = content; return this; }

    /// <summary>Sets the preferred side the card is placed on relative to the target (auto-flips if it doesn't fit).</summary>
    public StepBuilder Placement(Side side) { _placement = side; return this; }

    /// <summary>Sets the spotlight hole's padding around the target and its corner radius (in pixels).</summary>
    public StepBuilder Spotlight(double padding, double radius) { _padding = padding; _radius = radius; return this; }

    /// <summary>
    /// Sets whether the spotlight hole is click-through: when <paramref name="allow"/> is <c>true</c>
    /// (the default) the user can click the highlighted control directly; when <c>false</c> clicks
    /// over the hole are captured by the overlay so the target can only be looked at.
    /// </summary>
    public StepBuilder Interactive(bool allow = true) { _allowInteraction = allow; return this; }

    /// <summary>
    /// Sets whether this step is skipped when its target can't be resolved. When <paramref name="skip"/>
    /// is <c>true</c> (the default) the tour moves on to the next showable step; when <c>false</c> the
    /// step is still shown, as a centered card. No effect on modal steps.
    /// </summary>
    public StepBuilder SkipIfMissing(bool skip = true) { _skipIfMissing = skip; return this; }

    /// <summary>For a multi-target step, anchors the card to the target at <paramref name="index"/> instead of the union of all holes.</summary>
    public StepBuilder Anchor(int index) { _anchorIndex = index; return this; }

    /// <summary>Sets a callback invoked when this step becomes active (for example, to start a live demo or open a panel).</summary>
    public StepBuilder OnEnter(Action action) { _onEnter = action; return this; }

    /// <summary>Sets a callback invoked when this step is left (for example, to stop a live demo or restore a panel's state).</summary>
    public StepBuilder OnExit(Action action) { _onExit = action; return this; }

    internal TourStep Build() => new TourStep()
    {
        TargetId = _targetId,
        TargetProvider = _targetProvider,
        TargetIds = _targetIds,
        TargetsProvider = _targetsProvider,
        AnchorIndex = _anchorIndex,
        Title = _title,
        Content = _content,
        Placement = _placement,
        SpotlightPadding = _padding,
        SpotlightRadius = _radius,
        AllowInteraction = _allowInteraction,
        SkipIfMissing = _skipIfMissing,
        OnEnter = _onEnter,
        OnExit = _onExit,
    };
}
