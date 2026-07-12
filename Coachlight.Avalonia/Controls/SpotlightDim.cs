using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering;

namespace Coachlight.Avalonia.Controls;

internal sealed class SpotlightDim : Control, ICustomHitTest
{
    private readonly SpotlightGeometryCache _cache = new();

    public static readonly StyledProperty<Rect?> HoleProperty =
        AvaloniaProperty.Register<SpotlightDim, Rect?>(nameof(Hole));

    public static readonly StyledProperty<double> CornerRadiusProperty =
        AvaloniaProperty.Register<SpotlightDim, double>(nameof(CornerRadius), 8d);

    public static readonly StyledProperty<IBrush?> FillProperty =
        AvaloniaProperty.Register<SpotlightDim, IBrush?>(nameof(Fill));
    
    static SpotlightDim()
    {
        AffectsRender<SpotlightDim>(HoleProperty, CornerRadiusProperty, FillProperty, BoundsProperty);
    }

    public Rect? Hole
    {
        get => GetValue(HoleProperty);
        set => SetValue(HoleProperty, value);
    }

    public double CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public IBrush? Fill
    {
        get => GetValue(FillProperty);
        set => SetValue(FillProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        var bounds = Bounds;
        if(bounds.Width <= 0 || bounds.Height <= 0)
            return;
        var geometry = _cache.Get(bounds.Width, bounds.Height, Hole, CornerRadius);
        context.DrawGeometry(Fill, null, geometry);
    }

    public bool HitTest(Point point)
    {
        var bounds = Bounds;
        if(bounds.Width <= 0 || bounds.Height <= 0)
            return false;
        var geometry = _cache.Get(bounds.Width, bounds.Height, Hole, CornerRadius);
        return geometry.FillContains(point);
    }
}