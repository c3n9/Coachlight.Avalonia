using Avalonia;
using Avalonia.Controls;

namespace Coachlight.Avalonia.Targeting;

/// <summary>
/// Attached property used to tag a control as a tour target, e.g.
/// <c>&lt;Button coachlight:Coachmark.Id="btnConnect" /&gt;</c>. Reference the same id from
/// <see cref="Building.TourBuilder.Coachmark(string?, System.Action{Building.StepBuilder})"/>.
/// </summary>
public sealed class Coachmark
{
    private Coachmark()
    {
    }

    /// <summary>Defines the Coachmark.Id attached property.</summary>
    public static readonly AttachedProperty<string?> IdProperty =
        AvaloniaProperty.RegisterAttached<Coachmark, Control, string?>("Id");

    /// <summary>Sets the tour target id on <paramref name="element"/>.</summary>
    public static void SetId(Control element, string? value)
        => element.SetValue(IdProperty, value);

    /// <summary>Gets the tour target id set on <paramref name="element"/>, if any.</summary>
    public static string? GetId(Control element)
        => element.GetValue(IdProperty);
}
