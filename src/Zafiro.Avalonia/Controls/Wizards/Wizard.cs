using System.Reactive;
using ReactiveUI.SourceGenerators;
using Zafiro.Avalonia.Commands;
using Zafiro.Avalonia.Controls.Wizards.Builder;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Controls.Wizards;

public partial class Wizard : ReactiveObject, IWizard
{
    private readonly IList<IStep?> createdPages;
    private readonly IList<Func<IStep?, IStep>> pageFactories;
    private IStep content;

    [Reactive] private int currentIndex = -1;

    public Wizard(List<Func<IStep?, IStep>> pages)
    {
        pageFactories = pages;
        createdPages = pages.Select(_ => (IStep?)null).ToList();

        var hasNext = this.WhenAnyValue(x => x.CurrentIndex)
            .Select(i => i < pages.Count - 1);

        IsValid = this.WhenAnyValue(x => x.Content)
            .WhereNotNull()
            .Select(x => x.IsValid)
            .Switch()
            .StartWith(false);

        IsBusy = this.WhenAnyValue(x => x.Content)
            .WhereNotNull()
            .Select(x => x.IsBusy)
            .Switch()
            .StartWith(false);

        // Podemos ir al siguiente si existe un paso más y si el actual es válido
        var canGoNext = hasNext.CombineLatest(IsValid, (h, v) => h && v);

        IsLastPage = hasNext.Not();

        var nextCommand = ReactiveCommand.Create(() =>
        {
            // Avanzamos el índice
            CurrentIndex++;

            if (createdPages[CurrentIndex] == null)
            {
                // Obtenemos la página anterior (si existe)
                var previousPage = CurrentIndex > 0 ? createdPages[CurrentIndex - 1] : null;
                // Creamos la nueva página pasando la anterior
                createdPages[CurrentIndex] = pageFactories[CurrentIndex](previousPage);
            }

            Content = createdPages[CurrentIndex]!;
        }, canGoNext);

        // Podemos volver atrás si currentIndex > 0
        var canGoBack = this.WhenAnyValue(x => x.CurrentIndex)
            .Select(i => i > 0).CombineLatest(IsBusy, (hasPrev, b) => hasPrev && !b);

        var backCommand = ReactiveCommand.Create(() =>
        {
            createdPages[CurrentIndex] = null;

            CurrentIndex--;
            // La instancia ya existe, así que la reutilizamos
            Content = createdPages[CurrentIndex]!;
        }, canGoBack);

        Back = EnhancedCommand.Create(backCommand);
        Next = EnhancedCommand.Create(nextCommand);

        SetupAutoAdvance(nextCommand);

        // Forzamos la carga de la primera página
        nextCommand.Execute().Subscribe();
    }

    private IDisposable SetupAutoAdvance(ReactiveCommand<Unit, Unit> nextCommand)
    {
        return IsValid.Trues().Where(_ => Content.AutoAdvance).ToSignal().InvokeCommand(nextCommand);
    }

    public IEnhancedCommand Back { get; }
    public IEnhancedCommand Next { get; }

    public IStep Content
    {
        get => content;
        set => this.RaiseAndSetIfChanged(ref content, value);
    }

    public IObservable<bool> IsLastPage { get; }
    public IObservable<bool> IsValid { get; }
    public IObservable<bool> IsBusy { get; }
}