using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Rendering;

namespace Coachlight.Avalonia.Controls;

internal sealed class SpotlightDim : Control, ICustomHitTest
{
    private readonly SpotlightGeometryCache _cache = new();

    public static readonly StyledProperty<IReadOnlyList<Rect>?> HolesProperty =
        AvaloniaProperty.Register<SpotlightDim, IReadOnlyList<Rect>?>(nameof(Holes));

    public static readonly StyledProperty<double> CornerRadiusProperty =
        AvaloniaProperty.Register<SpotlightDim, double>(nameof(CornerRadius), 8d);

    public static readonly StyledProperty<IBrush?> FillProperty =
        AvaloniaProperty.Register<SpotlightDim, IBrush?>(nameof(Fill));

    static SpotlightDim()
    {
        AffectsRender<SpotlightDim>(HolesProperty, CornerRadiusProperty, FillProperty, BoundsProperty);
    }

    public IReadOnlyList<Rect>? Holes
    {
        get => GetValue(HolesProperty);
        set => SetValue(HolesProperty, value);
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
        var geometry = _cache.Get(bounds.Width, bounds.Height, Holes, CornerRadius);
        context.DrawGeometry(Fill, null, geometry);
    }

    public bool HitTest(Point point)
    {
        var bounds = Bounds;
        if(bounds.Width <= 0 || bounds.Height <= 0)
            return false;
        var geometry = _cache.Get(bounds.Width, bounds.Height, Holes, CornerRadius);
        return geometry.FillContains(point);
    }
}
