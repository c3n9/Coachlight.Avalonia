using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace Coachlight.Avalonia.Controls;

public class CoachmarkCard : TemplatedControl
{
    public static readonly StyledProperty<object?> TitleProperty = 
        AvaloniaProperty.Register<CoachmarkCard, object?>(nameof(Title));

    public object? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<CoachmarkCard, object?>(nameof(Content));

    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }
    
    public static readonly StyledProperty<string?> ProgressTextProperty = 
        AvaloniaProperty.Register<CoachmarkCard, string?>(nameof(ProgressText));

    public string? ProgressText
    {
        get => GetValue(ProgressTextProperty);
        set => SetValue(ProgressTextProperty, value);
    }
    
    public static readonly StyledProperty<bool> IsFirstProperty =
        AvaloniaProperty.Register<CoachmarkCard, bool>(nameof(IsFirst), false);

    public bool IsFirst
    {
        get => GetValue(IsFirstProperty);
        set => SetValue(IsFirstProperty, value);
    }
    
    public static readonly StyledProperty<bool> IsLastProperty =
        AvaloniaProperty.Register<CoachmarkCard, bool>(nameof(IsLast), false);

    public bool IsLast
    {
        get => GetValue(IsLastProperty);
        set => SetValue(IsLastProperty, value);
    }
    
    public static readonly StyledProperty<string?> NextTextProperty =
        AvaloniaProperty.Register<CoachmarkCard, string?>(nameof(NextText));

    public string? NextText
    {
        get => GetValue(NextTextProperty);
        set => SetValue(NextTextProperty, value);
    }

    public static readonly StyledProperty<ICommand?> NextCommandProperty =
        AvaloniaProperty.Register<CoachmarkCard, ICommand?>(nameof(NextCommand));

    public ICommand? NextCommand
    {
        get => GetValue(NextCommandProperty);
        set => SetValue(NextCommandProperty, value);
    }
    
    public static readonly StyledProperty<ICommand?> PreviousCommandProperty =
        AvaloniaProperty.Register<CoachmarkCard, ICommand?>(nameof(PreviousCommand));

    public ICommand? PreviousCommand
    {
        get => GetValue(PreviousCommandProperty);
        set => SetValue(PreviousCommandProperty, value);
    }

    public static readonly StyledProperty<ICommand?> SkipCommandProperty =
        AvaloniaProperty.Register<CoachmarkCard, ICommand?>(nameof(SkipCommand));

    public ICommand? SkipCommand
    {
        get => GetValue(SkipCommandProperty);
        set => SetValue(SkipCommandProperty, value);
    }
}