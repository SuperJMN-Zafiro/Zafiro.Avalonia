namespace TestApp.Samples.ControlsNew.Navigation;

public class TargetViewModel(string parameter, IOtherDependency otherDependency)
{
    public string Parameter { get; } = parameter;
    public IOtherDependency OtherDependency { get; } = otherDependency;
}

public interface IOtherDependency;