# Fluid.Avalonia

<p align="center">
  <a href="https://www.nuget.org/packages/Fluid.Avalonia"><img src="https://img.shields.io/nuget/v/Fluid.Avalonia.svg?label=NuGet&color=blue" alt="NuGet version" /></a>
  <a href="https://www.nuget.org/packages/Fluid.Avalonia"><img src="https://img.shields.io/nuget/dt/Fluid.Avalonia.svg?label=Downloads&color=blue" alt="NuGet downloads" /></a>
  <a href="https://github.com/Alpaq92/Fluid.Avalonia/actions/workflows/ci.yml"><img src="https://img.shields.io/github/actions/workflow/status/Alpaq92/Fluid.Avalonia/ci.yml?branch=main&label=CI" alt="CI" /></a>
  <a href="https://github.com/Alpaq92/Fluid.Avalonia/actions/workflows/release.yml"><img src="https://img.shields.io/github/actions/workflow/status/Alpaq92/Fluid.Avalonia/release.yml?branch=main&label=Release" alt="Release" /></a>
  <a href="https://alpaq92.github.io/Fluid.Avalonia/"><img src="https://img.shields.io/badge/demo-live-success" alt="Live demo" /></a>
  <a href="LICENSE"><img src="https://img.shields.io/badge/License-MIT-blue.svg" alt="License: MIT" /></a>
</p>

A **Fluent 2-inspired** theme for [Avalonia](https://avaloniaui.net/) **12** (.NET 8) with its own identity. It layers authentic WinUI tokens and metrics over Avalonia's `FluentTheme`, and picks up the system accent color natively on Windows, macOS and Linux.

Your app's windows get a translucent backdrop too: the native one where it exists — Mica on Windows 11, a vibrancy blend on macOS, a KWin blur on KDE — or a software-rendered **acrylic** alternative, from the companion **[Fluid.Avalonia.Acrylic](https://www.nuget.org/packages/Fluid.Avalonia.Acrylic)** package, anywhere it doesn't.

![Fluid.Avalonia — the demo's Accents page, split diagonally between the light and dark themes](https://raw.githubusercontent.com/Alpaq92/Fluid.Avalonia/main/screenshot.png)

## Live demo

Try it right now in your browser — the demo runs on Avalonia's **WebAssembly** head, deployed to **GitHub Pages**:

> **Live demo →** **<https://alpaq92.github.io/Fluid.Avalonia/>** *(redeployed from the WASM head on every push to `main`)*

## Get started

Install from NuGet:

```
dotnet add package Fluid.Avalonia
```

It ships as a single `Styles` object you drop into your app:

```
<Application xmlns:fluid="clr-namespace:Fluid.Avalonia;assembly=Fluid.Avalonia">
  <Application.Styles>
    <fluid:FluidTheme />
  </Application.Styles>
</Application>
```

The accent adapts to the OS automatically, but you can also drive it from code — `AccentService` exposes the built-in preset palette plus a small apply / reset API:

```
using System.Linq;
using Fluid.Avalonia;

// the 20 built-in Open Color presets, each with a Name + Color
foreach (var p in AccentService.Preset)
    Console.WriteLine($"{p.Name}: {p.Color}");

// apply one as the app accent…
var teal = AccentService.Preset.First(p => p.Name == "Teal");
AccentService.SetAccent(teal.Color);

// …or hand back to the live OS accent. Overloads can throw PlatformNotSupportedException
// on an unsupported platform, or take your own fallback color.
AccentService.UseSystemAccent();

// observe accent changes — fires after every (re)publish, including live OS accent
// changes, with CurrentAccent already holding the fresh color.
AccentService.AccentChanged += (_, _) => Console.WriteLine(AccentService.CurrentAccent);
```

The repository also contains **Fluid.Avalonia.Demo**, a demo app that mirrors the structure of Microsoft's **WinUI 3 Gallery** (data-driven navigation, per-item pages, a Settings page) so you can compare the result side by side.

> **Deep dive:** the architecture and how the resource layering works, the demo-app internals, and the full coverage / roadmap matrix all live in **[OVERVIEW.md](OVERVIEW.md)**.

## How the demo is built

One shared library holds the whole gallery. Two thin heads launch it:

| Project | Role |
| --- | --- |
| `Fluid.Avalonia.Demo` | The gallery itself — App, Views, Controls, pages, assets. |
| `Fluid.Avalonia.Demo.Desktop` | Desktop entry point. Adds the Mica window and custom title bar. |
| `Fluid.Avalonia.Demo.Browser` | WebAssembly entry point. Hosts the gallery as the single top-level. |

Both heads show the same shell, because the window's contents live in a shared `MainView`. The Windows-only pieces — Mica, the `WM_SETICON` taskbar fix, the registry accent read — are guarded, so they simply don't run in the browser.

Every push to `main` publishes the Browser head to GitHub Pages via `.github/workflows/pages.yml`. That needs one bit of setup, once: **Settings → Pages → Source = "GitHub Actions"**.

To build the browser head yourself:

```
dotnet workload install wasm-tools
dotnet publish Fluid.Avalonia.Demo.Browser -c Release
```

---

## What it is?

- **A WinUI 3 look.** Fluent 2 color tokens, the WinUI type ramp, 4 px / 8 px corner radii, the "lit-edge" control border and drop-shadow elevation. Text and icons come from bundled fonts — **DejaVu Sans** and **Codicons** — so they render identically on desktop and in the browser.
- **A live accent, on every OS.** Read natively from the host: the seven-shade Windows `AccentPalette`, the macOS `AppleAccentColor`, and the GNOME, KDE or Cinnamon accent on Linux. It reaches every accented control and updates the moment the user changes it. Apps can override it instead — 20 **Open Color** presets, or any color at all through `AccentService`.
- **Themed for the whole control set,** newest included. Avalonia 12.1's `TableView` reads exactly like our `ListBox` and `DataGrid`, down to a shared header-hover fill. The demo's **Collections** page puts it beside the DataGrid.
- **Self-contained.** One `net8.0` library, no third-party theme dependencies, layered on Avalonia's own `FluentTheme`. The platform-specific pieces — accent readers, Mica, the dark title bar — are guarded, so they degrade gracefully wherever they don't apply.

## Why?

It began as a quest to align Avalonia with WinUI 3, and grew past the visual tokens as controls were added along the way. [Romzetron.Avalonia](https://github.com/Romzetron/Romzetron.Avalonia) was the reference for solution structure and semantic-brush styling, with three deliberate departures:

1. **No baked-in accent.** Adapt to whatever the user set in their OS. Presets and manual selection are options, never the default.
2. **Avalonia 12 and .NET 8.**
3. **Token-driven theming only.** No per-control color overrides, the way Fluent 2 actually works.

Hand-porting some 70 control templates would mean re-fighting every Avalonia template change. Layering on `FluentTheme` and overriding only the token layer keeps the theme small, faithful, and cheap to carry forward.

## Custom controls

The demo isn't only re-themed stock controls. These were built for this project, and have no direct equivalent in vanilla Avalonia:

**Time and date**

- **`RadialTimePicker`** — two concentric rings, hour inside and minute outside, each a Fluent slider bent into a circle. Switches to the Material 24-hour layout via `Is24Hour`.
- **`RadialClock`** — the dial above, on its own. Drop it straight onto a page for an inline picker.
- **`DateTimePicker`** — a segmented date and time field opening a flyout of looping spinner columns. `DateTimeSpinners` is those columns, reusable on their own.
- **`AnalogDateTimePicker`** — the same field, but its dropdown pairs a `Calendar` with the radial dial instead of spinner wheels.

**Input and selection**

- **`BinarySelector`** — a two-value segmented switch where an accent pill slides between the options. Returns your object, not a bool, so `LeftValue="AM" RightValue="PM"` just works.
- **`RadialSlider`** — a circular slider on the same ring geometry as the clock dial.
- **`VisualRate`** — a row of clickable glyphs for a 0..N rating. Click the topmost lit one again to clear it.
- **`SignaturePad`** — freehand signing with a velocity-driven variable-width pen, so ink reads calligraphic rather than flat. Ported from android-signaturepad.
- **`FluidColorPicker`** — a swatch and hex button opening tabbed color editors. Its body, `FluidColorEditor`, can be hosted in any flyout of your own.
- **`LabeledTextInput`** — a caption paired with a text field, matched to the Fluent `ColorView` inputs.

**Layout and status**

- **`BreadcrumbBar`** — a chevron-joined trail that collapses WinUI-style behind a `…` chip when it runs out of room. Give it a `ChildrenSelector` and every chevron becomes a directory dropdown.
- **`InfoBar`** — a severity banner: icon, title, message, optional action, dismiss button.
- **`GroupBox`** — a titled card, the classic WPF control Avalonia lacks.
- **`ProgressCircle`** — a determinate radial progress ring.
- **`BusyArea`** — wrap any content and toggle `IsBusy` to dim it behind a scrim and a progress indicator.
- **`ContentDialog`** — a modal over a dimmed surface, DialogHost.Avalonia re-themed to Fluent 2. An in-window overlay, so it works in the browser too.

Plus the shell itself: the reusable **`FluidWindow`**, a system tray menu, and the thin Fluent scrollbar.

**[CUSTOM.md](CUSTOM.md)** describes each one in full, and the demo's **Custom** page renders that same document live.

## Building & running

Needs the **.NET 10 SDK**, pinned in `global.json`. The library and desktop demo still target `net8.0`; only the WebAssembly head needs SDK 10.

```
dotnet build
dotnet run --project Fluid.Avalonia.Demo.Desktop
```

**Hot reload** is wired into the Desktop head but off by default, because the package is commercial and fails the build without a licence key. Opt in per run — details in [OVERVIEW.md](OVERVIEW.md#hot-reload-opt-in):

```
dotnet watch run -p:FluidAvaloniaHotReload=true -p:AvaloniaUILicenseKey=<key>
```

Two behaviours worth knowing when you run it:

- **The accent** comes from the OS on Windows, macOS and Linux, and falls back to Avalonia's platform accent elsewhere. A preset or a picked color overrides it at any time.
- **The backdrop** follows `FluidWindow.TransparencyEnabled`: Mica on Windows, vibrancy on macOS, and a real blur on KDE. Elsewhere the window turns solid. Windows seeds it from the OS "Transparency effects" setting; **Settings → Window** in the demo overrides that. The dark title-bar frame stays Windows-only.

## Inspirations

- **[Fluent 2 Design System](https://fluent2.microsoft.design/)** — type ramp, color tokens, elevation and materials guidance.
- **[microsoft-ui-xaml](https://github.com/microsoft/microsoft-ui-xaml)** (MIT) — the canonical WinUI 3 control theme resources (`src/controls/dev/CommonStyles/*_themeresources.xaml`) and `Common_themeresources` color values were ported from here.
- **[WinUI 3 Gallery](https://github.com/microsoft/WinUI-Gallery)** (MIT) — the structural reference for the demo app (data-driven catalog, NavigationView shell, ItemPage, ControlExample).
- **[Romzetron.Avalonia](https://github.com/Romzetron/Romzetron.Avalonia)** — solution structure and the file-per-control / semantic-brush styling architecture.
- **[FluentAvalonia](https://github.com/amwx/FluentAvalonia)** — cross-checked our approach.
- **[SukiUI](https://github.com/kikipoulet/SukiUI)** — reference for the custom window / title-bar technique, the thin overlay scrollbar look, and the Playground page concept.
- **[Semi.Avalonia](https://github.com/irihitech/Semi.Avalonia)** — reference for the roomy circular-day Calendar styling.
- **[Markdown.Avalonia](https://github.com/whistyun/Markdown.Avalonia)** — renders the Home page.
- **[Material Design Icons](https://pictogrammers.com/library/mdi/)** (Apache-2.0) — the *code-array* glyph used in the app icon.
- **[Open Color](https://yeun.github.io/open-color/)** (MIT) — the colors behind the 20 built-in accent presets.
- **[WPF-UI](https://github.com/lepoco/wpfui)** (MIT) — the demo's `BreadcrumbBar` and `InfoBar` are reimplemented after its controls of the same name, and its `NotifyIcon` inspired the Fluent-themed system-tray menu.
- **[Dirkster99/bm](https://github.com/Dirkster99/bm)** (MIT) — the breadcrumb's per-crumb chevron-dropdown navigation.
- **[DialogHost.Avalonia](https://github.com/AvaloniaUtils/DialogHost.Avalonia)** (MIT) — the Fluent-themed `ContentDialog` host (an in-window overlay, so it works on the browser head too).
- **[Avalonia.Samples](https://github.com/AvaloniaUI/Avalonia.Samples)** (MIT) — the `VisualRate` is generalised from its `RatingControlSample` port.
- **[android-signaturepad](https://github.com/warting/android-signaturepad)** (MIT; based on [gcacace/android-signaturepad](https://github.com/gcacace/android-signaturepad), Apache-2.0) — the `SignaturePad`'s velocity-driven variable-width Bézier ink algorithm is ported from here.
- **[Avalonia](https://github.com/AvaloniaUI/Avalonia)** — the `FluentTheme` we build on.

## Credits

The full list of third-party projects this solution bundles, depends on, or references — each with what it's used for and its license — lives in **[CREDITS.md](CREDITS.md)**.

## License

[MIT](LICENSE).

The Fluent 2 design-token *values* and WinUI resource *structures* are ported from the MIT-licensed [microsoft-ui-xaml](https://github.com/microsoft/microsoft-ui-xaml) and [WinUI 3 Gallery](https://github.com/microsoft/WinUI-Gallery) projects, © Microsoft Corporation. The built-in accent presets come from the MIT-licensed [Open Color](https://yeun.github.io/open-color/).

**[CREDITS.md](CREDITS.md)** has the full attribution list.
