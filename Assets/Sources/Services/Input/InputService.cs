using System;
using UniRx;

public interface IInputService
{
    T GetHandler<T>() where T : IInputHandler;
}

public interface IInputHandler
{
}

public abstract class InputHandler<T> : IInputHandler
{
    private readonly ReactiveProperty<T> _property;

    protected InputHandler(ReactiveProperty<T> property)
    {
        _property = property ?? throw new ArgumentNullException(nameof(property));
    }

    public IReadOnlyReactiveProperty<T> Property => _property;
}