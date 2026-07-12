using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace Coachlight.Avalonia.Controls;

/// <summary>
/// A tooltip-style card that presents a single tour step: title, body content,
/// progress and navigation. Its appearance is defined by a <c>ControlTheme</c>
/// and can be fully overridden by consumers.
/// </summary>
public class CoachmarkCard : TemplatedControl
{
    /// <summary>Defines the <see cref="Title"/> property.</summary>
    public static readonly StyledProperty<object?> TitleProperty =
        AvaloniaProperty.Register<CoachmarkCard, object?>(nameof(Title));

    /// <summary>Gets or sets the step title. Rendered via a presenter, so it accepts a string or any content.</summary>
    public object? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>Defines the <see cref="Content"/> property.</summary>
    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<CoachmarkCard, object?>(nameof(Content));

    /// <summary>Gets or sets the step body. Rendered via a presenter, so it accepts a string or any content.</summary>
    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>Defines the <see cref="ProgressText"/> property.</summary>
    public static readonly StyledProperty<string?> ProgressTextProperty =
        AvaloniaProperty.Register<CoachmarkCard, string?>(nameof(ProgressText));

    /// <summary>Gets or sets the progress label, for example "2 / 5".</summary>
    public string? ProgressText
    {
        get => GetValue(ProgressTextProperty);
        set => SetValue(ProgressTextProperty, value);
    }

    /// <summary>Defines the <see cref="IsFirst"/> property.</summary>
    public static readonly StyledProperty<bool> IsFirstProperty =
        AvaloniaProperty.Register<CoachmarkCard, bool>(nameof(IsFirst), false);

    /// <summary>Gets or sets whether this is the first shown step. Used to hide the "Back" button.</summary>
    public bool IsFirst
    {
        get => GetValue(IsFirstProperty);
        set => SetValue(IsFirstProperty, value);
    }

    /// <summary>Defines the <see cref="IsLast"/> property.</summary>
    public static readonly StyledProperty<bool> IsLastProperty =
        AvaloniaProperty.Register<CoachmarkCard, bool>(nameof(IsLast), false);

    /// <summary>Gets or sets whether this is the last shown step. Used to switch the primary button to "Done".</summary>
    public bool IsLast
    {
        get => GetValue(IsLastProperty);
        set => SetValue(IsLastProperty, value);
    }

    /// <summary>Defines the <see cref="NextText"/> property.</summary>
    public static readonly StyledProperty<string?> NextTextProperty =
        AvaloniaProperty.Register<CoachmarkCard, string?>(nameof(NextText), "Next");

    /// <summary>Gets or sets the caption of the primary (next/done) button.</summary>
    public string? NextText
    {
        get => GetValue(NextTextProperty);
        set => SetValue(NextTextProperty, value);
    }

    /// <summary>Defines the <see cref="NextCommand"/> property.</summary>
    public static readonly StyledProperty<ICommand?> NextCommandProperty =
        AvaloniaProperty.Register<CoachmarkCard, ICommand?>(nameof(NextCommand));

    /// <summary>Gets or sets the command invoked to advance to the next step.</summary>
    public ICommand? NextCommand
    {
        get => GetValue(NextCommandProperty);
        set => SetValue(NextCommandProperty, value);
    }

    /// <summary>Defines the <see cref="PreviousCommand"/> property.</summary>
    public static readonly StyledProperty<ICommand?> PreviousCommandProperty =
        AvaloniaProperty.Register<CoachmarkCard, ICommand?>(nameof(PreviousCommand));

    /// <summary>Gets or sets the command invoked to go back to the previous step.</summary>
    public ICommand? PreviousCommand
    {
        get => GetValue(PreviousCommandProperty);
        set => SetValue(PreviousCommandProperty, value);
    }

    /// <summary>Defines the <see cref="SkipCommand"/> property.</summary>
    public static readonly StyledProperty<ICommand?> SkipCommandProperty =
        AvaloniaProperty.Register<CoachmarkCard, ICommand?>(nameof(SkipCommand));

    /// <summary>Gets or sets the command invoked to skip the tour.</summary>
    public ICommand? SkipCommand
    {
        get => GetValue(SkipCommandProperty);
        set => SetValue(SkipCommandProperty, value);
    }
}