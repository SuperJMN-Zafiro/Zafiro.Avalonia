using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls.Primitives;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Zafiro.Avalonia.Controls.Wizard
{
    public class WizardControl : TemplatedControl
    {
        public WizardControl()
        {
            this.WhenAnyValue(x => x.Pages)
                .WhereNotNull()
                .Select(enumerable => new Wizard(enumerable.ToList()))
                .BindTo(this, x => x.Wizard);
        }

        [Reactive]
        public Wizard? Wizard { get; private set; }

        public static readonly StyledProperty<IEnumerable<IWizardPage>> PagesProperty = AvaloniaProperty.Register<WizardControl, IEnumerable<IWizardPage>>(
            nameof(Pages));

        public IEnumerable<IWizardPage> Pages
        {
            get => GetValue(PagesProperty);
            set => SetValue(PagesProperty, value);
        }
    }
}