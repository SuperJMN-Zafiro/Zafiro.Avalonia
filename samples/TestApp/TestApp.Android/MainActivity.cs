﻿using Android.App;
using Android.Content.PM;
using Avalonia.Android;

namespace TestApp.Android;

[Activity(Label = "TestApp.Android", Theme = "@style/MyTheme.NoActionBar", Icon = "@drawable/icon", LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
}
