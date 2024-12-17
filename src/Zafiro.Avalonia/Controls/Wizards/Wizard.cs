using ReactiveUI.SourceGenerators;
using Zafiro.Avalonia.Commands;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Controls.Wizards;

public partial class Wizard : ReactiveObject, IWizard
{
    private readonly IList<Func<IValidatable?, IValidatable>> pageFactories;
    private readonly IList<IValidatable?> createdPages;

    [Reactive] private int currentIndex = -1;
    private IValidatable content;

    public IEnhancedCommand Back { get; }
    public IEnhancedCommand Next { get; }

    public IValidatable Content
    {
        get => content;
        set => this.RaiseAndSetIfChanged(ref content, value);
    }

    public IObservable<bool> IsLastPage { get; }

    public Wizard(List<Func<IValidatable?, IValidatable>> pages)
    {
        pageFactories = pages;
        // Al inicio, ninguna instancia está creada, así que iniciamos con null
        createdPages = pages.Select(_ => (IValidatable?)null).ToList();

        // Para saber si hay siguiente página: CurrentIndex < pages.Count - 1
        var hasNext = this.WhenAnyValue(x => x.CurrentIndex)
            .Select(i => i < pages.Count - 1);

        // Validez del contenido actual
        var isValid = this.WhenAnyValue(x => x.Content)
            .WhereNotNull()
            .Select(x => x.IsValid)
            .Switch()
            .StartWith(false);

        // Podemos ir al siguiente si existe un paso más y si el actual es válido
        var canGoNext = hasNext.CombineLatest(isValid, (h, v) => h && v);
        
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
            .Select(i => i > 0);

        var backCommand = ReactiveCommand.Create(() =>
        {
            createdPages[CurrentIndex] = null;
            
            CurrentIndex--;
            // La instancia ya existe, así que la reutilizamos
            Content = createdPages[CurrentIndex]!;
        }, canGoBack);

        Back = EnhancedCommand.Create(backCommand);
        Next = EnhancedCommand.Create(nextCommand);

        // Forzamos la carga de la primera página
        nextCommand.Execute().Subscribe();
    }
}
