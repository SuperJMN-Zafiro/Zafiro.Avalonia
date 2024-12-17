namespace Zafiro.Avalonia.Dialogs.SizingAlgorithms;

public static class DialogSizeCalculator
{
    public static int CalculateDialogWidth(string message)
    {
        // Definir algunos parámetros base
        int charWidth = 7; // Ancho aproximado de un carácter
        int padding = 20; // Espacio adicional para márgenes y relleno

        // Calcular el número total de caracteres en el mensaje
        int totalChars = message.Length;

        // Determinar el máximo número de caracteres por línea basado en la longitud del texto
        int maxCharsPerLine;
        if (totalChars <= 50)
        {
            maxCharsPerLine = 20;
        }
        else if (totalChars <= 100)
        {
            maxCharsPerLine = 30;
        }
        else if (totalChars <= 200)
        {
            maxCharsPerLine = 40;
        }
        else
        {
            maxCharsPerLine = 50;
        }

        // Dividir el mensaje en líneas considerando el ancho máximo
        var lines = message
            .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Aggregate((currentLine: "", allLines: Enumerable.Empty<string>()), (acc, word) =>
            {
                var newLine = string.IsNullOrEmpty(acc.currentLine) ? word : $"{acc.currentLine} {word}";

                if (newLine.Length <= maxCharsPerLine)
                {
                    acc.currentLine = newLine;
                }
                else
                {
                    acc.allLines = acc.allLines.Append(acc.currentLine);
                    acc.currentLine = word;
                }

                return acc;
            }, acc => acc.allLines.Append(acc.currentLine));

        // Encontrar la línea más larga
        int maxLineLength = lines.Max(line => line.Length);

        // Calcular el ancho ideal basado en la línea más larga
        int width = (maxLineLength * charWidth) + padding;

        return width;
    }
}