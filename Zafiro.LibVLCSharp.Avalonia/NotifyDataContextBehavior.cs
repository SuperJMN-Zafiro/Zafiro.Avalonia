using System.Diagnostics;
using Avalonia;
using Avalonia.Xaml.Interactivity;
using ReactiveUI;

namespace Zafiro.Avalonia.LibVLCSharp
{
    public class NotifyDataContextBehavior : Behavior<VideoView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.GetObservable(StyledElement.DataContextProperty).Subscribe(Update);
        }

        private void Update(object? dataContext)
        {
            if (dataContext is not null)
            {
                MessageBus.Current.SendMessage(new MediaPlayerCreated(AssociatedObject, dataContext));
            }
            else
            {
                Debug.WriteLine("DataContext is null in ViewViewAdvanced");
            }
        }
    }
}
