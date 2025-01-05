using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using Zafiro.DataAnalysis.Graphs;
using INode = Zafiro.FileSystem.Core.INode;

namespace Zafiro.Avalonia.Controls.Diagrams.Enhanced;

public class Edges : MarkupExtension
{
    // La lista de nodos a la que haremos referencia
    public required IEnumerable<INode> List { get; set; }

    // Recogemos los <avalonia:Edge .../> como hijitos
    // Marcamos [Content] para que XAML coloque las EdgeItem dentro.
    [Content]
    public List<EdgeItem> Items { get; } = new List<EdgeItem>();

    // Este método se invoca en tiempo de XAML para crear la "salida"
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var result = new List<IEdge<INode>>();

        if (List == null || !List.Any() || Items.Count == 0)
            return result; // lista vacía si no hay datos

        // Para cada "EdgeItem", buscamos en la List el nodo con el Name que coincida
        foreach (var edgeItem in Items)
        {
            var fromNode = List.FirstOrDefault(n => n.Name == edgeItem.From);
            var toNode   = List.FirstOrDefault(n => n.Name == edgeItem.To);

            if (fromNode != null && toNode != null)
            {
                result.Add(new MyEdge(fromNode, toNode));
            }
        }
        return result;
    }

    // Implementación interna de la arista
    private class MyEdge : IEdge<INode>
    {
        public INode From { get; }
        public INode To { get; }

        public MyEdge(INode from, INode to)
        {
            From = from;
            To   = to;
        }
    }
}