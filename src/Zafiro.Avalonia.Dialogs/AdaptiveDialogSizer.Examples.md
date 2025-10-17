# AdaptiveDialogSizer - Examples and Usage

## Overview

`AdaptiveDialogSizer` provides intelligent dialog sizing based on **real content measurement** rather than estimation. It measures the actual desired size of dialog content and applies proportional constraints based on available screen
space.

## Key Features

- **Real measurement**: Uses Avalonia's `Measure()` to get actual content size
- **Proportional sizing**: Respects available space (default: 80% of screen)
- **No guesswork**: No heuristics or estimations - just actual layout calculations
- **Flexible configuration**: Customizable ratios, min/max sizes, and fixed dimensions
- **Multiple dialog types**: Works with both Window-based and Adorner-based dialogs

## Basic Usage

### 1. Using Pre-built Adaptive Dialog Implementations

```csharp
// Desktop (Window-based) dialog with default configuration
var dialog = new AdaptiveDesktopDialog();

// With custom configuration
var config = new AdaptiveDialogSizer.SizingConfig
{
    MaxWidthRatio = 0.7,  // Max 70% of screen width
    MaxHeightRatio = 0.85, // Max 85% of screen height
    MinWidth = 400,
    MinHeight = 250
};
var customDialog = new AdaptiveDesktopDialog(config);

// Show dialog
await customDialog.Show(viewModel, "My Dialog", closeable => 
    new[] { Option.Ok(closeable), Option.Cancel(closeable) });
```

### 2. Using AdaptiveAdornerDialog (for Single-View apps)

```csharp
var adornerDialog = new AdaptiveAdornerDialog(
    () => AdornerLayer.GetAdornerLayer(mainView)!,
    new AdaptiveDialogSizer.SizingConfig
    {
        MaxWidthRatio = 0.9,
        MaxHeightRatio = 0.9
    });

await adornerDialog.Show(viewModel, "Title", closeable => options);
```

### 3. Manual Integration with Existing Dialogs

```csharp
// In your existing dialog implementation
var dialogContent = new DialogControl
{
    Content = viewModel,
    Options = options
};

window.Content = dialogContent;

// Apply adaptive sizing
var parentBounds = mainWindow.Bounds;
var result = AdaptiveDialogSizer.ApplyToWindow(
    window,
    dialogContent,
    parentBounds,
    new AdaptiveDialogSizer.SizingConfig
    {
        MaxWidthRatio = 0.75,
        MaxHeightRatio = 0.8
    });

// Result contains information about the sizing decision
Console.WriteLine($"Desired: {result.DesiredSize}");
Console.WriteLine($"Applied: {result.Width}x{result.Height}");
Console.WriteLine($"Constrained: {result.IsConstrained}");
```

## Configuration Options

### SizingConfig Properties

```csharp
public record SizingConfig
{
    // Maximum width as ratio of available space (0.0 - 1.0)
    public double MaxWidthRatio { get; init; } = 0.8;
    
    // Maximum height as ratio of available space (0.0 - 1.0)
    public double MaxHeightRatio { get; init; } = 0.8;
    
    // Minimum dialog dimensions
    public double MinWidth { get; init; } = 300;
    public double MinHeight { get; init; } = 200;
    
    // Fixed dimensions (overrides calculation)
    public double? FixedWidth { get; init; }
    public double? FixedHeight { get; init; }
}
```

## Common Scenarios

### Scenario 1: Small Dialog Content

```csharp
// Content desired size: 250x180
// Available space: 1920x1080
// MaxRatio: 0.8

// Result: 300x200 (enforces minimum size)
```

### Scenario 2: Large Dialog Content

```csharp
// Content desired size: 2000x1200
// Available space: 1920x1080
// MaxRatio: 0.8

// Result: 1536x864 (80% of available space)
```

### Scenario 3: Fixed Width, Adaptive Height

```csharp
var config = new AdaptiveDialogSizer.SizingConfig
{
    FixedWidth = 600,
    MaxHeightRatio = 0.7
};

// Width will always be 600px
// Height adapts to content (up to 70% of screen)
```

### Scenario 4: Different Ratios for Width/Height

```csharp
var config = new AdaptiveDialogSizer.SizingConfig
{
    MaxWidthRatio = 0.6,   // Narrower dialogs
    MaxHeightRatio = 0.9   // Can use more vertical space
};
```

## How It Works

1. **Measurement Phase**:
    - Content is measured with `Size.Infinity` to get its natural desired size
    - This is the actual size the content wants to be without constraints

2. **Constraint Calculation**:
    - Maximum allowed size = available space × ratio (e.g., 1920px × 0.8 = 1536px)
    - Minimum size from configuration

3. **Application**:
    - Final size = `Max(MinSize, Min(DesiredSize, MaxSize))`
    - Window/control is sized explicitly
    - Min/Max constraints prevent resizing

## Benefits Over SizeToContent

Traditional `SizeToContent`:

- Dialog can grow unbounded
- May exceed screen bounds
- No proportional sizing

AdaptiveDialogSizer:

- ✓ Respects screen boundaries
- ✓ Maintains proportions
- ✓ Based on real measurements
- ✓ Configurable constraints
- ✓ Predictable behavior

## Integration Examples

### Replace DesktopDialog

```diff
- public class MyDialog : IDialog
+ public class MyDialog : AdaptiveDesktopDialog
  {
-     public async Task<bool> Show(...)
-     {
-         var window = new Window
-         {
-             SizeToContent = SizeToContent.WidthAndHeight,
-             MaxWidth = 800,
-             MaxHeight = 800,
-             // ...
-         };
-         // ...
-     }
+     public MyDialog() : base(new AdaptiveDialogSizer.SizingConfig
+     {
+         MaxWidthRatio = 0.8,
+         MaxHeightRatio = 0.8
+     })
+     {
+     }
  }
```

### Update DialogService Factory

```diff
  public static IDialog Create()
  {
      return Application.Current.ApplicationLifetime switch
      {
-         ISingleViewApplicationLifetime singleView => new AdornerDialog(...),
+         ISingleViewApplicationLifetime singleView => new AdaptiveAdornerDialog(...),
-         _ => new StackedDialog()
+         _ => new AdaptiveDesktopDialog()
      };
  }
```

## Testing

```csharp
[Fact]
public void TestAdaptiveSizing()
{
    var content = new Border { Width = 1000, Height = 600 };
    var availableSize = new Size(1920, 1080);
    
    var result = AdaptiveDialogSizer.CalculateSize(
        content, 
        availableSize, 
        new AdaptiveDialogSizer.SizingConfig());
    
    Assert.Equal(1000, result.Width);  // Fits within 80%
    Assert.Equal(600, result.Height);
    Assert.False(result.IsConstrained);
}

[Fact]
public void TestContentExceedsMaxRatio()
{
    var content = new Border { Width = 2000, Height = 1200 };
    var availableSize = new Size(1920, 1080);
    
    var result = AdaptiveDialogSizer.CalculateSize(content, availableSize);
    
    Assert.Equal(1536, result.Width);  // 1920 * 0.8
    Assert.Equal(864, result.Height);  // 1080 * 0.8
    Assert.True(result.IsConstrained);
}
```
