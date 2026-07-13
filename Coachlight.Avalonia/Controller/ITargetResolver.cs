using Avalonia.Controls;

namespace Coachlight.Avalonia.Controller;

/// <summary>
/// Resolves a <see cref="Models.TourStep.TargetId"/> to the actual <see cref="Control"/> it
/// refers to. The default implementation searches the visual tree for a control tagged with
/// <see cref="Targeting.Coachmark.IdProperty"/>; supply your own to resolve targets differently
/// (e.g. by automation id, or across a different tree).
/// </summary>
public interface ITargetResolver
{
    /// <summary>Returns the control tagged with <paramref name="id"/>, or <c>null</c> if none is found.</summary>
    Control? ResolveById(string id);
}
