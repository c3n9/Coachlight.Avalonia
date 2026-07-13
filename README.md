
# Coachlight.Avalonia [![NuGet](https://img.shields.io/nuget/v/Coachlight.Avalonia.svg)](https://www.nuget.org/packages/Coachlight.Avalonia/)

Interactive onboarding / product tours for [Avalonia](https://avaloniaui.net/) apps — spotlight
coachmarks, modal steps, and step-by-step navigation. Zero runtime dependencies beyond Avalonia
itself, MVVM-friendly, and fully re-themeable.

<p align="center">
    <img width="1012" height="540" alt="Frame 47983" src="https://github.com/user-attachments/assets/d7aa87ad-5f36-44b1-b569-a44c0cfb2ce4" alt="Coachlight.Avalonia banner" />
</p>


- **Spotlight coachmarks** — dims the screen and cuts a rounded hole around the target control.
  The target stays fully interactive (clicks pass through the hole) — you can even drive a live
  demo while the step is shown (see `OnEnter`/`OnExit`).
- **Any control can be a target** — buttons, text boxes, combo boxes, checkboxes, sliders, whole
  list boxes — targeting works off the control's actual on-screen bounds, not just buttons.
- **Auto-flip placement** — the card tries your preferred side first and falls back to whichever
  side actually fits the window.
- **Zero dependencies** — no MVVM Toolkit, no ReactiveUI required to use the library itself.
- **MVVM-friendly** — `StartTour` takes a `Visual` anchor (e.g. via `CommandParameter`), so a
  view model never needs a direct reference to a view.
- **Fully re-themeable** — the card is a `TemplatedControl` with a default `ControlTheme`; every
  color is a `DynamicResource` you can override, and the whole template can be replaced.
- **Opt-in persistence** — show a tour once per user with a pluggable `IProgressStore`; the
  library never touches disk unless you explicitly ask it to.

https://github.com/user-attachments/assets/264ae1e3-30cd-45f3-9faf-117156bb496a

## Install

```bash
dotnet add package Coachlight.Avalonia
```

## Quickstart

**1. Reference the default theme** in `App.axaml`:

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceInclude Source="avares://Coachlight.Avalonia/Themes/Coachlight.axaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```

**2. Tag the controls you want to spotlight** with `Coachmark.Id`:

```xml
<Window xmlns:cl="clr-namespace:Coachlight.Avalonia.Targeting;assembly=Coachlight.Avalonia">
    <Button cl:Coachmark.Id="btnConnect" Content="Connect"/>
</Window>
```

**3. Build a tour and start it:**

```csharp
using Coachlight.Avalonia;                // StartTour extension
using Coachlight.Avalonia.Building;
using Coachlight.Avalonia.Enums;

var tour = TourBuilder.Create("demo")
    .Modal(s => s
        .Title("Welcome!")
        .Text("A short tour of the interface."))
    .Coachmark("btnConnect", s => s
        .Placement(Side.Right)
        .Title("Connect")
        .Text("This button connects to the server."))
    .Build();

anchor.StartTour(tour); // anchor: any Visual already attached to the window
```

That's it — a dimmed overlay with a spotlighted hole and a navigation card appears over the
window. See `samples/Coachlight.Gallery` for a full MVVM example covering `TextBox`, `ComboBox`,
`CheckBox`, `Slider`, `ListBox`, custom button captions, and a live auto-scroll demo.

## Targeting

Two ways to point a step at a control:

```csharp
// By id (attached property, works from anywhere — recommended)
.Coachmark("btnConnect", s => s.Title(...))

// By direct reference, resolved lazily when the step is shown
.Coachmark(() => myControl, s => s.Title(...))
```

A step with no target (`.Modal(...)`) is shown as a centered card with no spotlight.

## Localizing button captions

```csharp
TourBuilder.Create("demo")
    .Labels(new TourLabels
    {
        Skip = "Überspringen",
        Back = "Zurück",
        Next = "Nächste",
        Done = "Erledigt",
    })
    ...
```

## Live demos on a step (`OnEnter` / `OnExit`)

```csharp
.Coachmark("recentItemsList", s => s
    .Title("Recent items")
    .Text("Watch it scroll by itself.")
    .OnEnter(() => StartAutoScroll(anchor))
    .OnExit(() => StopAutoScroll(anchor)))
```

`OnEnter` runs when the step becomes active, `OnExit` when the user navigates away (or the tour
ends) — exceptions thrown from either are swallowed so a broken demo can't break navigation.

## Showing a tour only once

`StartTour(anchor, tour)` shows the tour every time it's called. To show it once per user, pass
an `IProgressStore` — the bundled `JsonProgressStore` persists to a JSON file under the OS
application-data folder:

```csharp
static readonly IProgressStore Store = new JsonProgressStore();

anchor.StartTour(tour, Store);              // shown once; no-op afterwards
anchor.StartTour(tour, Store, force: true);  // always shown (e.g. a "?" help button)
```

Implement `IProgressStore` yourself to persist elsewhere (a database, cloud settings, etc.).

## Customizing the look

Every color used by the default theme is a `DynamicResource` — override the keys in your own
resources to restyle without touching the template:

```xml
<SolidColorBrush x:Key="CoachlightAccentBrush" Color="#FF16A34A"/>
<SolidColorBrush x:Key="CoachlightCardBackgroundBrush" Color="#FF202020"/>
```

Keys: `CoachlightDimBrush`, `CoachlightCardBackgroundBrush`, `CoachlightCardBorderBrush`,
`CoachlightForegroundBrush`, `CoachlightSecondaryForegroundBrush`, `CoachlightAccentBrush`,
`CoachlightAccentForegroundBrush`. Light/dark variants are provided via `ThemeDictionaries` and
switch automatically with `ActualThemeVariant`.

For structural changes, provide your own `ControlTheme` keyed `{x:Type controls:CoachmarkCard}`
(`xmlns:controls="clr-namespace:Coachlight.Avalonia.Controls;assembly=Coachlight.Avalonia"`) —
`CoachmarkCard` exposes `Title`, `Content`, `ProgressText`, `IsFirst`, `IsLast`, `SkipText`,
`BackText`, `NextText`, `SkipCommand`, `PreviousCommand`, `NextCommand` for the template to bind.

## License

MIT — see [LICENSE](LICENSE).
