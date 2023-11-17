namespace Zafiro.Avalonia.Controls.StringEditor;

public class EditableString : Editable<StringBox>
{
    public EditableString(string initialValue) : base(new StringBox(initialValue), () => new StringBox(initialValue), (a, b) => b.Text = a.Text )
    {
    }
}