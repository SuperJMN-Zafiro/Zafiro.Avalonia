using Zafiro.Avalonia.Commands;

namespace Zafiro.Avalonia.Dialogs;

public interface IOption<T, Q> : IOption
{
    IEnhancedCommand<T, Q> TypedCommand { get; }
}

public interface IOption
{
    string Title { get; }

    IEnhancedCommand Command { get; }

    bool IsDefault { get; }

    bool IsCancel { get; }
    public IObservable<bool> IsVisible { get; }

    public OptionRole Role { get; }
}

public enum OptionRole
{
    /// <summary>
    /// Acción principal (ej. “OK”, “Guardar”, “Aceptar”…)
    /// </summary>
    Primary,

    /// <summary>
    /// Acción secundaria, complementaria a la principal (ej. “Opciones avanzadas”, “Más info”…)
    /// </summary>
    Secondary,

    /// <summary>
    /// Acción de cancelar o cerrar (ej. “Cancelar”, “Cerrar”...)
    /// </summary>
    Cancel,

    /// <summary>
    /// Acción destructiva (ej. “Eliminar”, “Borrar”)
    /// </summary>
    Destructive,

    /// <summary>
    /// Acción informativa, ayuda, o simplemente no tan central 
    /// (ej. “Ayuda”, “Leer más”, “Ver documentación”)
    /// </summary>
    Info
}
