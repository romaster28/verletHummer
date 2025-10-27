using System;

public class CompositeHandler<T> : IHandler<T>
{
    private readonly IHandler<T>[] _handlers;

    public CompositeHandler(IHandler<T>[] handlers)
    {
        _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
    }

    public void Handle(T eventData)
    {
        foreach (var handler in _handlers) 
            handler.Handle(eventData);
    }
}