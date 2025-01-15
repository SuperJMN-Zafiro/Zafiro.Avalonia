namespace Zafiro.Avalonia.Controls.SuperCanvas;

public class NonOverlappingLayoutManager : AvaloniaObject, ILayoutManager
{
    public void ProcessLayout(List<ControlPosition> positions)
    {
        const int maxIterations = 100;
        var iteration = 0;
        bool hasOverlap;

        do
        {
            hasOverlap = false;
            iteration++;

            for (int i = 0; i < positions.Count; i++)
            {
                for (int j = i + 1; j < positions.Count; j++)
                {
                    var pos1 = positions[i];
                    var pos2 = positions[j];

                    if (IsOverlapping(pos1, pos2))
                    {
                        hasOverlap = true;
                            
                        double dx = (pos2.Left + pos2.Width / 2) - (pos1.Left + pos1.Width / 2);
                        double dy = (pos2.Top + pos2.Height / 2) - (pos1.Top + pos1.Height / 2);
                            
                        double distance = Math.Sqrt(dx * dx + dy * dy);
                        if (distance < 0.0001)
                        {
                            dx = 1;
                            dy = 0;
                        }
                        else
                        {
                            dx /= distance;
                            dy /= distance;
                        }

                        double overlapX = (pos1.Width + pos2.Width) / 2 - 
                                          Math.Abs((pos1.Left + pos1.Width / 2) - (pos2.Left + pos2.Width / 2));
                        double overlapY = (pos1.Height + pos2.Height) / 2 - 
                                          Math.Abs((pos1.Top + pos1.Height / 2) - (pos2.Top + pos2.Height / 2));
                        double overlap = Math.Min(overlapX, overlapY);

                        double moveDistance = overlap / 2 + 1;
                        pos1.Left -= dx * moveDistance;
                        pos1.Top -= dy * moveDistance;
                        pos2.Left += dx * moveDistance;
                        pos2.Top += dy * moveDistance;
                    }
                }
            }
        }
        while (hasOverlap && iteration < maxIterations);
    }

    private bool IsOverlapping(ControlPosition pos1, ControlPosition pos2)
    {
        if (ReferenceEquals(pos1.Control, pos2.Control))
            return false;

        return !(pos1.Left + pos1.Width <= pos2.Left ||
                 pos2.Left + pos2.Width <= pos1.Left ||
                 pos1.Top + pos1.Height <= pos2.Top ||
                 pos2.Top + pos2.Height <= pos1.Top);
    }
}