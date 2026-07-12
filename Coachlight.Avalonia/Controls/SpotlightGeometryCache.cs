using Avalonia;
using Avalonia.Media;

namespace Coachlight.Avalonia.Controls;

internal sealed class SpotlightGeometryCache
{
    private double _width = double.NaN;
    private double _height = double.NaN;
    private double _radius = double.NaN;

    private Rect? _hole;
    private Geometry? _cached;

    public Geometry Get(double width, double height, Rect? hole, double radius)
    {
        if (_cached is not null && Near(_width, width) && Near(_height, height) && Near(_radius, radius) && HoleEquals(_hole, hole))
        {
            return _cached;           
        }

        _width = width;
        _height = height;
        _radius = radius;
        _hole = hole;
        _cached = Build(width, height, hole, radius);
        return _cached;
    }

    public void Reset()
    {
        _width = _height = _radius = double.NaN;
        _hole = null;
        _cached = null;
    }

    private static Geometry Build(double width, double height, Rect? hole, double radius)
    {
        var geo = new StreamGeometry();
        using var ctx = geo.Open();
        ctx.SetFillRule(FillRule.EvenOdd);
        
        ctx.BeginFigure(new Point(0, 0), isFilled: true);
        ctx.LineTo(new Point(width, 0));
        ctx.LineTo(new Point(width, height));
        ctx.LineTo(new Point(0, height));
        ctx.EndFigure(isClosed: true);
        
        if (hole is Rect r && r.Width > 0 && r.Height > 0)
        {
            var rr = Math.Min(radius, Math.Min(r.Width, r.Height) / 2);
            AddRoundedRect(ctx, r, rr);
        }

        return geo;
    }

    private static void AddRoundedRect(StreamGeometryContext ctx, Rect r, double rr)
    {
        double left = r.X, top = r.Y, right = r.Right, bottom = r.Bottom;
        var size = new Size(rr, rr);

        ctx.BeginFigure(new Point(left + rr, top), isFilled: true);
        ctx.LineTo(new Point(right - rr, top));
        ctx.ArcTo(new Point(right, top + rr), size, 0, false, SweepDirection.Clockwise);
        ctx.LineTo(new Point(right, bottom - rr));
        ctx.ArcTo(new Point(right - rr, bottom), size, 0, false, SweepDirection.Clockwise);
        ctx.LineTo(new Point(left + rr, bottom));
        ctx.ArcTo(new Point(left, bottom - rr), size, 0, false, SweepDirection.Clockwise);
        ctx.LineTo(new Point(left, top + rr));
        ctx.ArcTo(new Point(left + rr, top), size, 0, false, SweepDirection.Clockwise);
        ctx.EndFigure(isClosed: true);
    }
    
    private static bool Near(double a, double b) => Math.Abs(a - b) < 0.5;
    
    private static bool HoleEquals(Rect? a, Rect? b)
    {
        if (a is null && b is null)
            return true;
        
        if (a is Rect ra && b is Rect rb)                
            return Near(ra.X, rb.X) && Near(ra.Y, rb.Y) && Near(ra.Width, rb.Width) && Near(ra.Height, rb.Height);

        return false;     
    }
}