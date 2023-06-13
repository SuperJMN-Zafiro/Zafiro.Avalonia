namespace TestApp.ViewModels;

public class Section
{
    public string Name { get; }
    public object Content { get; }

    public Section(string name, object content)
    {
        Name = name;
        Content = content;
    }
}