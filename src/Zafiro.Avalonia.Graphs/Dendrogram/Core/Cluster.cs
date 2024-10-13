using System;
using System.Collections.Generic;
using System.Linq;

namespace Zafiro.Avalonia.DataViz.Dendrogram.Core;

public class Cluster
{
    public object? Content { get; }
    public IReadOnlyList<object> AllElements { get; }
    public Cluster? Left { get; }
    public Cluster? Right { get; }
    public Cluster? Parent { get; private set; } // Nuevo campo para el padre
    public double FusionDistance { get; }

    // Constructor para clústeres iniciales (hojas)
    public Cluster(object content)
    {
        Content = content;
        Left = null;
        Right = null;
        FusionDistance = 0.0;
        Parent = null;  // Las hojas no tienen padre
        AllElements = [content];
    }

    public IEnumerable<Cluster?> Items => [Left, Right];

    // Constructor para clústeres fusionados
    public Cluster(Cluster left, Cluster right, double fusionDistance)
    {
        if (left == null || right == null)
            throw new ArgumentNullException("Left and Right clusters cannot be null.");

        AllElements = new List<object>(left.AllElements.Concat(right.AllElements)).AsReadOnly();
        Left = left;
        Right = right;
        FusionDistance = fusionDistance;
        Parent = null;  // El padre se establecerá en los hijos después de la fusión

        // Establecer este clúster como padre de los hijos
        left.SetParent(this);
        right.SetParent(this);
    }

    // Método privado para establecer el padre
    private void SetParent(Cluster parent)
    {
        if (Parent != null)
            throw new InvalidOperationException("Parent is already set.");
        Parent = parent;
    }
}
