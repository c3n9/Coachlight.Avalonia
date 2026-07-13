using Avalonia;
using Avalonia.VisualTree;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
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
    private IDisposable? _layerBoundsSub;
    private readonly DispatcherTimer _refreshTimer;
    private DateTime _warmupUntil;
    private Rect? _prevSpot;
    private bool _hasPrevSpot;

    public CoachmarkOverlay(TourController controller)
    {
        Focusable = true;
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));

        _refreshTimer = new DispatcherTimer(DispatcherPriority.Render) { Interval = TimeSpan.FromMilliseconds(16) };
        _refreshTimer.Tick += (_, _) => Recompute();
        
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
        Dispatcher.UIThread.Post(() => Focus(), DispatcherPriority.Input);

        Width = _layer.Bounds.Width;
        Height = _layer.Bounds.Height;
        _layerBoundsSub = _layer.GetObservable(BoundsProperty).Subscribe(new BoundsObserver(rect =>
        {
            Width = rect.Width;
            Height = rect.Height;
            RequestRefresh();
        }));

        _controller.CurrentStepChanged += OnCurrentStepChanged;
        _controller.Ended += OnEnded;
    }
    
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Source is Button && (e.Key is Key.Enter or Key.Space))
            return;

        switch (e.Key)
        {
            case Key.Escape:
                _controller.Skip();
                e.Handled = true;
                break;
            case Key.Enter:
            case Key.Right:
            case Key.Down:
                _controller.Next();
                e.Handled = true;
                break;
            case Key.Left:
            case Key.Up:
                _controller.Previous();
                e.Handled = true;
                break;
        }
    }

    private void Detach()
    {
        _refreshTimer.Stop();
        _controller.CurrentStepChanged -= OnCurrentStepChanged;
        _controller.Ended -= OnEnded;
        _layerBoundsSub?.Dispose();
        _layerBoundsSub = null;
        if (_layer is not null)
        {
            _layer.Children.Remove(this);
            _layer = null;
        }
    }

    private void OnEnded(object? sender, TourEndReason reason) => Detach();

    private void OnCurrentStepChanged(object? sender, TourStep? step)
    {
        if (step is null) return;
        SyncCard(step);
        _warmupUntil = DateTime.UtcNow + TimeSpan.FromMilliseconds(800); 
        _hasPrevSpot = false;                                            
        RequestRefresh();
    }

    private void SyncCard(TourStep step)
    {
        _card.Title = step.Title;
        _card.Content = step.Content;
        _card.ProgressText = $"{_controller.CurrentIndex + 1} / {_controller.StepCount}";
        _card.IsFirst = _controller.IsFirst;
        _card.IsLast = _controller.IsLast;
        
        var labels = _controller.CurrentTour?.Labels ?? TourLabels.Defaults;

        _card.BackText = labels.Back;
        _card.SkipText = labels.Skip;
        _card.NextText = _controller.IsLast ? 
            labels.Done : labels.Next;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == BoundsProperty)
            RequestRefresh();
    }
    
    private void RequestRefresh()
    {
        if (!_refreshTimer.IsEnabled)
            _refreshTimer.Start();
    }

    private void Recompute()
    {
        var width = Bounds.Width;
        var height = Bounds.Height;
        if (width <= 0 || height <= 0) return;

        _dim.Width = width;
        _dim.Height = height;

        var step = _controller.CurrentStep;
        if (step is null) { _refreshTimer.Stop(); return; }

        Rect? spot = null;
        if (!step.IsModal)
        {
            var rect = GetTargetRect(_controller.CurrentTarget);
            if (rect is { } r)
            {
                var p = step.SpotlightPadding;
                spot = new Rect(r.X - p, r.Y - p, r.Width + p * 2, r.Height + p * 2);
            }
        }

        if (spot is { } sr)
        {
            _dim.Hole = sr;
            _dim.CornerRadius = step.SpotlightRadius;
            PlaceCard(sr, step.Placement);
        }
        else if (step.IsModal)
        {
            _dim.Hole = null;
            CenterCard();
        }
        else
        {
            _dim.Hole = null;
            if (DateTime.UtcNow < _warmupUntil)
                return;             
            CenterCard();         
        }
        
        var stable = _hasPrevSpot && SpotEquals(spot, _prevSpot);
        _prevSpot = spot;
        _hasPrevSpot = true;
        if (stable && DateTime.UtcNow >= _warmupUntil)
            _refreshTimer.Stop();
    }
    
    private static bool SpotEquals(Rect? a, Rect? b)
    {
        if (a is null && b is null) return true;
        if (a is { } ra && b is { } rb)
            return Math.Abs(ra.X - rb.X) < 0.5 && Math.Abs(ra.Y - rb.Y) < 0.5
                                               && Math.Abs(ra.Width - rb.Width) < 0.5 && Math.Abs(ra.Height - rb.Height) < 0.5;
        return false;
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
        const double margin = 8;
        double w = Bounds.Width, h = Bounds.Height;

        Side[] order = side switch
        {
            Side.Top => new[] { Side.Top, Side.Bottom, Side.Right, Side.Left },
            Side.Left => new[] { Side.Left, Side.Right, Side.Bottom, Side.Top },
            Side.Right => new[] { Side.Right, Side.Left, Side.Bottom, Side.Top },
            _ => new[] { Side.Bottom, Side.Top, Side.Right, Side.Left }, // Bottom / Auto
        };

        foreach (var s in order)
        {
            double x, y;
            bool fits;
            switch (s)
            {
                case Side.Top:
                    x = hole.Center.X - cs.Width / 2;
                    y = hole.Y - gap - cs.Height;
                    fits = y >= margin;
                    break;
                case Side.Right:
                    x = hole.Right + gap;
                    y = hole.Center.Y - cs.Height / 2;
                    fits = x + cs.Width <= w - margin;
                    break;
                case Side.Left:
                    x = hole.X - gap - cs.Width;
                    y = hole.Center.Y - cs.Height / 2;
                    fits = x >= margin;
                    break;
                default: // Bottom
                    x = hole.Center.X - cs.Width / 2;
                    y = hole.Bottom + gap;
                    fits = y + cs.Height <= h - margin;
                    break;
            }

            if (fits)
            {
                SetCard(Math.Clamp(x, margin, Math.Max(margin, w - cs.Width - margin)),
                    Math.Clamp(y, margin, Math.Max(margin, h - cs.Height - margin)));
                return;
            }
        }

        double fx = hole.Center.X - cs.Width / 2;
        double fy = hole.Bottom + gap;
        SetCard(Math.Clamp(fx, margin, Math.Max(margin, w - cs.Width - margin)),
            Math.Clamp(fy, margin, Math.Max(margin, h - cs.Height - margin)));
    }

    private void SetCard(double x, double y)
    {
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

    private sealed class BoundsObserver : IObserver<Rect>
    {
        private readonly Action<Rect> _onNext;
        public BoundsObserver(Action<Rect> onNext) => _onNext = onNext;

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(Rect value) => _onNext(value);
    }
}