using Avalonia.Controls;

namespace Coachlight.Avalonia.Controller;

public interface ITargetResolver
{
    Control? ResolveById(string id);
}