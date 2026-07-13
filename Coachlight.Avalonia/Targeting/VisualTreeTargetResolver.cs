using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using Coachlight.Avalonia.Controller;

namespace Coachlight.Avalonia.Targeting;

/// <summary>Resolves targets by searching the visual tree for a control tagged with <see cref="Coachmark.IdProperty"/>.</summary>
internal sealed class VisualTreeTargetResolver : ITargetResolver
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