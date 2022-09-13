# Zafiro.Avalonia

Contains abstractions that can be helpful for Avalonia applications

## ShowAttachedFlyoutWhenFocusedBehavior

A `Behavior` that will show the associated flyout as long as the control is focused.

https://user-images.githubusercontent.com/3109851/185638344-07833b53-e935-45fe-8c03-f782549428d1.mp4

Usage:

```
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="using:FlyoutSample.ViewModels"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:i="clr-namespace:Avalonia.Xaml.Interactivity;assembly=Avalonia.Xaml.Interactivity"
        xmlns:avalonia="clr-namespace:Zafiro.Avalonia;assembly=Zafiro.Avalonia"
        mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
        x:Class="FlyoutSample.Views.MainWindow"
        Icon="/Assets/avalonia-logo.ico"
        Focusable="True"
        Title="FlyoutSample">

    <Design.DataContext>
        <vm:MainWindowViewModel/>
    </Design.DataContext>

    <TextBox VerticalAlignment="Center" HorizontalAlignment="Center" Text="Click me">
        <FlyoutBase.AttachedFlyout>
            <Flyout>
                <StackPanel>
                    <TextBox>Hi</TextBox>
                    <Button Content="Some button" />
                </StackPanel>
            </Flyout>
        </FlyoutBase.AttachedFlyout>

        <i:Interaction.Behaviors>
            <avalonia:ShowAttachedFlyoutWhenFocusedBehavior />
        </i:Interaction.Behaviors>
    </TextBox>

</Window>
```

* If you want to programmatically hide the flyout, you need to move the focus away from the control :) There's no other way to do it!

# NumberBoxBehavior

A behavior to force a TextBox to accept numbers only.

# MultiSelector

A control that can automate multiple selection in lists:

- Given a collection, it will reflect the status of the individual items with 3-state.
- Clicking it will toggle the status of all the items in the given collection.

```xml
 <views:MultiSelector Items="{Binding Items}" />
```

https://user-images.githubusercontent.com/3109851/189864634-6638e86f-f40b-4349-8c64-71fec7de7d5f.mp4

