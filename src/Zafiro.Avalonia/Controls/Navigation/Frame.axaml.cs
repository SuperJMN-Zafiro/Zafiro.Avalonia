using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using ReactiveUI;
using Zafiro.UI.Commands;
using Zafiro.UI.Navigation;

namespace Zafiro.Avalonia.Controls.Navigation
{
    public class Frame : TemplatedControl, IDisposable
    {
        public static readonly DirectProperty<Frame, INavigator> NavigatorProperty = 
            AvaloniaProperty.RegisterDirect<Frame, INavigator>(
                nameof(Navigator),
                o => o.Navigator,
                (o, v) => o.Navigator = v);

        private INavigator navigator;
        private readonly CompositeDisposable disposables = new();
        private object content;

        public static readonly DirectProperty<Frame, object> ContentProperty = 
            AvaloniaProperty.RegisterDirect<Frame, object>(
                nameof(Content), 
                o => o.Content, 
                (o, v) => o.Content = v);

        public object Content
        {
            get => content;
            private set
            {
                // Dispose previous content if it's disposable
                if (content is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                
                SetAndRaise(ContentProperty, ref content, value);
            }
        }

        public INavigator Navigator
        {
            get => navigator;
            set 
            {
                // Clean up previous subscriptions
                disposables.Clear();
                
                // Update the navigator reference
                var old = navigator;
                if (SetAndRaise(NavigatorProperty, ref navigator, value))
                {
                    // Unsubscribe from old navigator if needed
                    if (old != null && old is IDisposable disposableNavigator)
                    {
                        disposableNavigator.Dispose();
                    }
                    
                    // Subscribe to new navigator if it exists
                    if (value != null)
                    {
                        SetupNavigatorSubscriptions();
                    }
                }
            }
        }

        private void SetupNavigatorSubscriptions()
        {
            // Subscribe to Content observable
            navigator.Content
                .ObserveOn(RxApp.MainThreadScheduler)  // Ensure UI thread
                .Subscribe(newContent => Content = newContent)
                .DisposeWith(disposables);
            
            Back = navigator.Back;
        }

        public IEnhancedCommand? Back { get; private set; }

        public static readonly StyledProperty<bool> IsBackButtonVisibleProperty = 
            AvaloniaProperty.Register<Frame, bool>(
                nameof(IsBackButtonVisible), 
                defaultValue: true);

        public bool IsBackButtonVisible
        {
            get => GetValue(IsBackButtonVisibleProperty);
            set => SetValue(IsBackButtonVisibleProperty, value);
        }

        public static readonly StyledProperty<object> BackButtonContentProperty = 
            AvaloniaProperty.Register<Frame, object>(
                nameof(BackButtonContent));

        public object BackButtonContent
        {
            get => GetValue(BackButtonContentProperty);
            set => SetValue(BackButtonContentProperty, value);
        }

        protected override void OnUnloaded(RoutedEventArgs e)
        {
            // Dispose content and subscriptions when control is unloaded
            Dispose();
            base.OnUnloaded(e);
        }

        public void Dispose()
        {
            // Clean up all subscriptions
            disposables.Dispose();
            
            // Dispose content if applicable
            if (content is IDisposable disposable)
            {
                disposable.Dispose();
                content = null;
            }
        }
    }
}