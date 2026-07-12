using Avalonia;
using Avalonia.VisualTree;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Coachlight.Avalonia.Common;
using Coachlight.Avalonia.Controller;
using Coachlight.Avalonia.Enums;
using Coachlight.Avalonia.Models;

namespace Coachlight.Avalonia.Controls;

internal sealed class CoachmarkOverlay : Canvas
{
    private readonly TourController _controller;
    private readonly SpotlightDim _dim = new SpotlightDim();
    private readonly CoachmarkCard _card = new CoachmarkCard();
    private OverlayLayer? _layer;
    
    
    public CoachmarkOverlay(TourController controller)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));

        _dim.Bind(SpotlightDim.FillProperty, this.GetResourceObservable("CoachlightDimBrush"));
        Children.Add(_dim);
        Children.Add(_card);

        _card.NextCommand = new RelayCommand(_controller.Next);
        _card.PreviousCommand = new RelayCommand(_controller.Previous);
        _card.SkipCommand = new RelayCommand(_controller.Skip);

        _dim.PointerPressed += (sender, args) => controller.Next();
    }

    public void Attach(Visual anchor)
    {
        _layer = OverlayLayer.GetOverlayLayer(anchor);
        if (_layer is null) return;

        _layer.Children.Add(this);
        _controller.CurrentStepChanged += OnCurrentStepChanged;
        _controller.Ended += OnEnded;
    }

    private void Detach()
    {
        _controller.CurrentStepChanged -= OnCurrentStepChanged;
        _controller.Ended -= OnEnded;
        if (_layer is not null)
        {
            _layer.Children.Remove(this);
            _layer = null;
        }
    }

    private void OnEnded(object? sender, EventArgs e) => Detach();

    private void OnCurrentStepChanged(object? sender, TourStep? step)
    {
        if (step is null) return;
        SyncCard(step);
        RefreshVisual();
    }

    private void SyncCard(TourStep step)
    {
        _card.Title = step.Title;
        _card.Content = step.Content;
        _card.ProgressText = $"{_controller.CurrentIndex + 1} / {_controller.StepCount}";
        _card.IsFirst = _controller.IsFirst;
        _card.IsLast = _controller.IsLast;
        _card.NextText = _controller.IsLast ? "Done" : "Next";
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == BoundsProperty)
            RefreshVisual();
    }

    private void RefreshVisual()
    {
        var width = Bounds.Width;
        var height = Bounds.Height;
        if(width <= 0 || height <= 0) return;

        _dim.Width = width;
        _dim.Height = height;
        
        var step = _controller.CurrentStep;
        if (step is null) return;

        var rect = GetTargetRect(_controller.CurrentTarget);
        if (rect is { } r)
        {
            var padding = step.SpotlightPadding;
            _dim.Hole = new Rect(r.X - padding, r.Y - padding, r.Width + padding * 2, r.Height + padding * 2);
            _dim.CornerRadius = step.SpotlightRadius;
            PlaceCard(_dim.Hole.Value, step.Placement);
        }
        else
        {
            _dim.Hole = null;
            CenterCard();
        }
    }
    
    private Rect? GetTargetRect(Control? target)
    {
        if (target is null || !target.IsVisible || !target.IsEffectivelyVisible) return null;
        if (!target.IsArrangeValid || target.GetVisualRoot() is null) return null;

        var b = target.Bounds;
        if (b.Width <= 0 || b.Height <= 0) return null;

        if (target.TransformToVisual(this) is not { } m) return null;  
        var rect = new Rect(b.Size).TransformToAABB(m);               
        return rect is { Width: > 0, Height: > 0 } ? rect : null;
    }

    private void PlaceCard(Rect hole, Side side)
    {
        _card.Measure(new Size(Bounds.Width, Bounds.Height));
        var cs = _card.DesiredSize;
        const double gap = 12;
        
        double x = hole.Center.X - cs.Width / 2;
        double y = hole.Bottom + gap;
        
        var maxX = Math.Max(8, Bounds.Width - cs.Width - 8);
        var maxY = Math.Max(8, Bounds.Height - cs.Height - 8);

        x = Math.Clamp(x, 8, maxX);
        y = Math.Clamp(y, 8, maxY);
        
        Canvas.SetLeft(_card, x);
        Canvas.SetTop(_card, y);
    }
    
    private void CenterCard()
    {
        _card.Measure(new Size(Bounds.Width, Bounds.Height));
        var cs = _card.DesiredSize;
        Canvas.SetLeft(_card, (Bounds.Width - cs.Width) / 2);
        Canvas.SetTop(_card, (Bounds.Height - cs.Height) / 2);
    }
}