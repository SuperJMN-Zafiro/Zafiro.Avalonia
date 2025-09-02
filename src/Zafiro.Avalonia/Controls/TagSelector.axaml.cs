using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using CSharpFunctionalExtensions;
using ReactiveUI;

namespace Zafiro.Avalonia.Controls;

public class TagSelector : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<string>?> ItemsSourceProperty =
        AvaloniaProperty.Register<TagSelector, IEnumerable<string>?>(nameof(ItemsSource));

    public static readonly StyledProperty<int> MaxTagsProperty =
        AvaloniaProperty.Register<TagSelector, int>(nameof(MaxTags), 1);

    public static readonly StyledProperty<IDataTemplate?> TagTemplateProperty =
        AvaloniaProperty.Register<TagSelector, IDataTemplate?>(nameof(TagTemplate));

    public static readonly StyledProperty<StringComparison> StringComparisonProperty =
        AvaloniaProperty.Register<TagSelector, StringComparison>(nameof(StringComparison), StringComparison.OrdinalIgnoreCase);

    public IEnumerable<string>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public int MaxTags
    {
        get => GetValue(MaxTagsProperty);
        set => SetValue(MaxTagsProperty, Math.Max(1, value));
    }

    public IDataTemplate? TagTemplate
    {
        get => GetValue(TagTemplateProperty);
        set => SetValue(TagTemplateProperty, value);
    }

    public StringComparison StringComparison
    {
        get => GetValue(StringComparisonProperty);
        set => SetValue(StringComparisonProperty, value);
    }

    public ObservableCollection<string> SelectedTags { get; } = new();

    readonly ObservableCollection<string> suggestions = new();
    readonly ObservableCollection<object> elements = new();

    public ReactiveCommand<string, Unit> RemoveTagCommand { get; }

    TextBox? inputBox;
    ListBox? listBox;
    ListBox? suggestionsList;
    global::Avalonia.Controls.Primitives.Popup? popup;

    public TagSelector()
    {
        RemoveTagCommand = ReactiveCommand.Create<string>(RemoveTag);
        TagTemplateProperty.Changed.AddClassHandler<TagSelector>((x, _) => x.UpdateItemTemplate());
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        listBox = e.NameScope.Find<ListBox>("PART_SelectedItems");
        suggestionsList = e.NameScope.Find<ListBox>("PART_Suggestions");
        popup = e.NameScope.Find<global::Avalonia.Controls.Primitives.Popup>("PART_Popup");

        inputBox = new TextBox();
        inputBox.GetObservable(TextBox.TextProperty).Subscribe(_ => UpdateSuggestions());
        inputBox.KeyDown += InputBoxOnKeyDown;
        elements.Add(inputBox);

        if (listBox != null)
        {
            listBox.ItemsSource = elements;
            listBox.KeyDown += ListBoxOnKeyDown;
            listBox.SelectionChanged += ListBoxOnSelectionChanged;
            UpdateItemTemplate();
        }

        if (suggestionsList != null)
        {
            suggestionsList.ItemsSource = suggestions;
            suggestionsList.DoubleTapped += (_, _) => AddSelectedSuggestion();
            suggestionsList.KeyDown += SuggestionsListOnKeyDown;
        }

        if (popup != null)
        {
            popup.PlacementTarget = inputBox;
        }
    }

    void UpdateItemTemplate()
    {
        if (listBox == null)
        {
            return;
        }

        listBox.ItemTemplate = new FuncDataTemplate<object>((item, _) => item switch
        {
            string s => TagTemplate?.Build(s) ?? new TextBlock { Text = s },
            Control c => c,
            _ => new TextBlock { Text = item?.ToString() ?? string.Empty }
        });
    }

    void SuggestionsListOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            AddSelectedSuggestion();
            e.Handled = true;
        }
    }

    void ListBoxOnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (listBox?.SelectedItem is TextBox)
        {
            listBox.SelectedItem = null;
        }
    }

    void ListBoxOnKeyDown(object? sender, KeyEventArgs e)
    {
        if ((e.Key == Key.Delete || e.Key == Key.Back) && listBox?.SelectedItem is string s)
        {
            RemoveTag(s);
            e.Handled = true;
        }
    }

    void InputBoxOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Tab)
        {
            var first = suggestions.FirstOrDefault();
            AddTag(first);
            e.Handled = true;
        }
        else if ((e.Key == Key.Back || e.Key == Key.Delete) && string.IsNullOrEmpty(inputBox?.Text))
        {
            RemoveLastTag();
            e.Handled = true;
        }
    }

    void AddSelectedSuggestion()
    {
        if (suggestionsList?.SelectedItem is string s)
        {
            AddTag(s);
        }
    }

    void UpdateSuggestions()
    {
        if (inputBox == null)
        {
            return;
        }

        suggestions.Clear();
        var text = inputBox.Text ?? string.Empty;
        var source = ItemsSource ?? Enumerable.Empty<string>();
        foreach (var tag in source)
        {
            if (tag.Contains(text, StringComparison) && !SelectedTags.Contains(tag, GetComparer()))
            {
                suggestions.Add(tag);
            }
        }
        if (popup != null)
        {
            popup.IsOpen = suggestions.Any();
        }
    }

    void AddTag(string? tag)
    {
        Result.Success(tag ?? string.Empty)
            .Ensure(t => !string.IsNullOrWhiteSpace(t), "Empty")
            .Ensure(t => !SelectedTags.Contains(t, GetComparer()), "Duplicate")
            .Ensure(_ => SelectedTags.Count < MaxTags, "MaxReached")
            .Tap(t =>
            {
                SelectedTags.Add(t);
                elements.Insert(elements.Count - 1, t);
            })
            .Tap(() =>
            {
                if (inputBox != null)
                {
                    inputBox.Text = string.Empty;
                    inputBox.Focus();
                }
                UpdateSuggestions();
            });
    }

    void RemoveTag(string tag)
    {
        SelectedTags.Remove(tag);
        elements.Remove(tag);
        UpdateSuggestions();
    }

    void RemoveLastTag()
    {
        if (SelectedTags.Count > 0)
        {
            SelectedTags.RemoveAt(SelectedTags.Count - 1);
            elements.RemoveAt(elements.Count - 2);
            UpdateSuggestions();
        }
    }

    StringComparer GetComparer() => StringComparison switch
    {
        StringComparison.CurrentCulture => StringComparer.CurrentCulture,
        StringComparison.CurrentCultureIgnoreCase => StringComparer.CurrentCultureIgnoreCase,
        StringComparison.InvariantCulture => StringComparer.InvariantCulture,
        StringComparison.InvariantCultureIgnoreCase => StringComparer.InvariantCultureIgnoreCase,
        StringComparison.Ordinal => StringComparer.Ordinal,
        StringComparison.OrdinalIgnoreCase => StringComparer.OrdinalIgnoreCase,
        _ => StringComparer.Ordinal
    };
}

