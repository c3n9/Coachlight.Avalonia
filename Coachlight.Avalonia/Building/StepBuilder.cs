using Avalonia.Controls;
using Coachlight.Avalonia.Enums;
using Coachlight.Avalonia.Models;

namespace Coachlight.Avalonia.Building;

public  sealed class StepBuilder
{
    private readonly string? _targetId;
    private readonly Func<Control?>? _targetProvider;
    
    private object? _title;
    private object? _content;
    private Side _placement = Side.Auto;
    private double _padding = 8;
    private double _radius = 8;
    private Action? _onEnter;
    private Action? _onExit;

    internal StepBuilder(string? targetId, Func<Control?>? targetProvider)
    {
        _targetId = targetId;
        _targetProvider = targetProvider;
    }
    
    public StepBuilder Title(object title) { _title = title; return this; }
    public StepBuilder Text(object text) => Content(text); // alias
    public StepBuilder Content(object content) { _content = content; return this; }
    public StepBuilder Placement(Side side) { _placement = side; return this; }
    public StepBuilder Spotlight(double padding, double radius) { _padding = padding; _radius = radius; return this; }
    public StepBuilder OnEnter(Action action) { _onEnter = action; return this; }
    public StepBuilder OnExit(Action action) { _onExit = action; return this; }

    internal TourStep Build() => new TourStep()
    {
        TargetId = _targetId,
        TargetProvider = _targetProvider,
        Title = _title,
        Content = _content,
        Placement = _placement,
        SpotlightPadding = _padding,
        SpotlightRadius = _radius,
        OnEnter = _onEnter,
        OnExit = _onExit,
    };
}