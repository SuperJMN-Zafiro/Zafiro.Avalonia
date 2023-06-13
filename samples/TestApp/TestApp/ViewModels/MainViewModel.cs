using System.Collections.Generic;

namespace TestApp.ViewModels;

public class MainViewModel : ViewModelBase
{
    public IEnumerable<Section> Sections { get; } = new List<Section>()
    {
        new Section("Hi"),
        new Section("How"),
        new Section("Are"),
        new Section("You"),
    };
}

public class Section
{
    public string Name { get; }

    public Section(string name)
    {
        Name = name;
    }
}
