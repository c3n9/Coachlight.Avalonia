using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Coachlight.Avalonia.Controller;

namespace Coachlight.Avalonia.Targeting;

public class VisualTreeTargetResolver : ITargetResolver
{
    private readonly Visual _root;

    public VisualTreeTargetResolver(Visual root)
    {
        _root = root ?? throw new ArgumentNullException(nameof(root));
    }

    public Control? ResolveById(string id) =>
        _root.GetVisualDescendants()
            .OfType<Control>()
            .FirstOrDefault(c => Coachmark.GetId(c) == id);
}