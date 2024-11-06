using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Zafiro.DataAnalysis.Clustering;

namespace Zafiro.Avalonia.DataViz.Dendrograms
{
    public class DendrogramLinesControl : Control
    {
        public static readonly StyledProperty<ICluster?> RootClusterProperty =
            AvaloniaProperty.Register<DendrogramLinesControl, ICluster?>(nameof(RootCluster));

        public ICluster? RootCluster
        {
            get => GetValue(RootClusterProperty);
            set => SetValue(RootClusterProperty, value);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (RootCluster == null)
                return;

            // Definir un diccionario para almacenar las posiciones de las hojas
            var leafPositions = new Dictionary<ICluster, double>();
            var leafClusters = GetLeaves(RootCluster).ToList();
            double leafSpacing = Bounds.Width / (leafClusters.Count + 1);

            // Asignar posiciones X a las hojas
            for (int i = 0; i < leafClusters.Count; i++)
            {
                leafPositions[leafClusters[i]] = (i + 1) * leafSpacing;
            }

            // Calcular la altura máxima basada en MergeDistance
            double maxDistance = GetMaxMergeDistance(RootCluster);

            // Dibujar las líneas del dendrograma
            DrawClusterLines(context, RootCluster, leafPositions, Bounds.Height, maxDistance);
        }

        private void DrawClusterLines(DrawingContext context, ICluster cluster, Dictionary<ICluster, double> leafPositions, double controlHeight, double maxDistance)
        {
            if (cluster.Left != null && cluster.Right != null)
            {
                // Calcular posiciones
                double leftX = GetClusterX(cluster.Left, leafPositions);
                double rightX = GetClusterX(cluster.Right, leafPositions);
                double centerX = (leftX + rightX) / 2;

                double clusterY = controlHeight - (cluster.MergeDistance / maxDistance) * controlHeight;
                double leftY = controlHeight - (cluster.Left.MergeDistance / maxDistance) * controlHeight;
                double rightY = controlHeight - (cluster.Right.MergeDistance / maxDistance) * controlHeight;

                // Dibujar líneas horizontales y verticales
                var pen = new Pen(Brushes.Black, 1);

                // Línea horizontal superior
                context.DrawLine(pen, new Point(leftX, clusterY), new Point(rightX, clusterY));
                // Línea vertical izquierda
                context.DrawLine(pen, new Point(leftX, clusterY), new Point(leftX, leftY));
                // Línea vertical derecha
                context.DrawLine(pen, new Point(rightX, clusterY), new Point(rightX, rightY));

                // Dibujar recursivamente los subárboles
                DrawClusterLines(context, cluster.Left, leafPositions, controlHeight, maxDistance);
                DrawClusterLines(context, cluster.Right, leafPositions, controlHeight, maxDistance);
            }
        }

        private double GetClusterX(ICluster cluster, Dictionary<ICluster, double> leafPositions)
        {
            if (cluster.Left == null && cluster.Right == null)
            {
                // Es una hoja
                return leafPositions[cluster];
            }
            else
            {
                // Es un nodo interno
                double leftX = GetClusterX(cluster.Left!, leafPositions);
                double rightX = GetClusterX(cluster.Right!, leafPositions);
                return (leftX + rightX) / 2;
            }
        }

        private IEnumerable<ICluster> GetLeaves(ICluster cluster)
        {
            if (cluster.Left == null && cluster.Right == null)
            {
                yield return cluster;
            }
            else
            {
                if (cluster.Left != null)
                {
                    foreach (var leaf in GetLeaves(cluster.Left))
                    {
                        yield return leaf;
                    }
                }
                if (cluster.Right != null)
                {
                    foreach (var leaf in GetLeaves(cluster.Right))
                    {
                        yield return leaf;
                    }
                }
            }
        }

        private double GetMaxMergeDistance(ICluster cluster)
        {
            double maxDistance = cluster.MergeDistance;

            if (cluster.Left != null)
                maxDistance = System.Math.Max(maxDistance, GetMaxMergeDistance(cluster.Left));

            if (cluster.Right != null)
                maxDistance = System.Math.Max(maxDistance, GetMaxMergeDistance(cluster.Right));

            return maxDistance;
        }
    }
}

