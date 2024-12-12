using System.Collections.Generic;
using ReactiveUI;

namespace TestApp.Samples.ControlsNew.SlimDataGrid;

public class SlimDataGridViewModel : ReactiveObject
{
    public SlimDataGridViewModel()
    {
        People = GetPeople();
    }

    public IList<Person> People { get; }

    public IList<Person> GetPeople()
    {
        return
        [
            new() { Name = "Pepe", Surname = "Marchena" },
            new() { Name = "Carlos", Surname = "Santana" },
            new() { Name = "Lola", Surname = "Flores" },
            new() { Name = "Rafael", Surname = "Amargo" },
            new() { Name = "Juanito", Surname = "Valderrama" }
        ];
    }
}

public class Person
{
    public string Name { get; set; }
    public string Surname { get; set; }
}