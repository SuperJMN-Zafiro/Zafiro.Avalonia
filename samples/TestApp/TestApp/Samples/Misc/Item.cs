namespace TestApp.Samples.Misc;

public class Item
{
    public Item(int id)
    {
        Id = id;
    }

    public int Id { get; }

    public override string ToString() => $"Item {Id}";
}