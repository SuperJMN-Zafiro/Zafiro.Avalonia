namespace Zafiro.Avalonia.Misc;

public class Enum
{
    public static string[] GetNames(Type type) => System.Enum.GetNames(type);
}