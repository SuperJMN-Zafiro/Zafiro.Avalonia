using System.Collections;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace Zafiro.Avalonia.Extensions
{
    public class ItemIndex : MarkupExtension
    {
        public IValueConverter? Converter { get; set; }
        public object? ConverterParameter { get; set; }
        public bool StartFromOne { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new MultiBinding
            {
                Converter = new ContainerIdConverter()
                {
                    Converter = this.Converter,
                    ConverterParameter = this.ConverterParameter,
                    StartFromOne = this.StartFromOne,
                },
                Bindings =
                [
                    // 0: ItemsControl
                    new Binding
                    {
                        RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
                        {
                            AncestorType = typeof(ItemsControl)
                        }
                    },
                    // 1: Item itself
                    new Binding(),
                    // 2: ItemsSource.Count that forces reevaluation
                    new Binding
                    {
                        Path = "ItemsSource.Count",
                        RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
                        {
                            AncestorType = typeof(ItemsControl)
                        }
                    }
                ]
            };
        }
    }

    public class ContainerIdConverter : IMultiValueConverter
    {
        public IValueConverter? Converter { get; set; }
        public object? ConverterParameter { get; set; }
        public bool StartFromOne { get; set; }

        public object? Convert(
            IList<object?> values,
            Type targetType,
            object? parameter,
            CultureInfo culture)
        {
            if (values.Count < 2
                || values[0] is not ItemsControl itemsControl
                || values[1] is null)
            {
                return AvaloniaProperty.UnsetValue;
            }

            // Obtain the collection and look up the current index
            if (itemsControl.Items is IList list)
            {
                var index = list.IndexOf(values[1]!);
                if (StartFromOne)
                {
                    index++;
                }

                return index >= 0
                    ? Converter?.Convert(index, targetType, ConverterParameter, culture) ?? index
                    : AvaloniaProperty.UnsetValue;
            }

            return AvaloniaProperty.UnsetValue;
        }
    }
}