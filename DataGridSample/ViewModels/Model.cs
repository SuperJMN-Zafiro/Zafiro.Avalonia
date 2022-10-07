using System.Collections.Generic;

namespace DataGridSample.ViewModels;

public class Model
{
    public Model(string name, IEnumerable<Model> children)
    {
        Name = name;
        Children = children;
    }

    public string Name { get; }
    public IEnumerable<Model> Children { get; }
}