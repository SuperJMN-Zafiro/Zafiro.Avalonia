using System.Reactive.Subjects;

namespace Zafiro.Avalonia.Controls.Navigation;

public class ObservableStack<T>
{
    private readonly BehaviorSubject<int> countSubject;
    private readonly BehaviorSubject<T> topSubject;
    private readonly Stack<T> stack;
    
    public ObservableStack()
    {
        stack = new Stack<T>();
        countSubject = new BehaviorSubject<int>(0);
        topSubject = new BehaviorSubject<T>(default);
    }

    public void Push(T value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));
            
        stack.Push(value);
        UpdateObservables(value);
    }

    public T Pop()
    {
        if (!stack.Any())
            throw new InvalidOperationException("The stack is empty");
            
        var item = stack.Pop();
        var newTop = stack.Count > 0 ? stack.Peek() : default;
        UpdateObservables(newTop);
        
        return item;
    }

    private void UpdateObservables(T topValue)
    {
        countSubject.OnNext(stack.Count);
        topSubject.OnNext(topValue);
    }

    // Combined Observable and current value properties
    public IBehaviorSubject<int> Count => countSubject.AsBehaviorSubject();
    public IBehaviorSubject<T> Top => topSubject.AsBehaviorSubject();
    
    public bool IsEmpty => Count.Value == 0;
    
    public IEnumerable<T> ToEnumerable() => stack.ToArray();
    
    public void Clear()
    {
        stack.Clear();
        UpdateObservables(default);
    }
}

public interface IBehaviorSubject<T> : IObservable<T>
{
    T Value { get; }
}