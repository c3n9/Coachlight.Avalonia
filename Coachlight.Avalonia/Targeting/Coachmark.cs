using Avalonia;
using Avalonia.Controls;

namespace Coachlight.Avalonia.Targeting;

public sealed class Coachmark
{
    private Coachmark()
    {
        
    }
    
    public static readonly AttachedProperty<string?> IdProperty =
        AvaloniaProperty.RegisterAttached<Coachmark, Control, string?>("Id");   
    
    public static void SetId(Control element, string? value) 
        => element.SetValue(IdProperty, value);
    
    public static string? GetId(Control element)
        => element.GetValue(IdProperty);
}