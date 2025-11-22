using System;
using System.Collections.Generic;
using System.Linq;

public class CompositeHandler<T> : IHandler<T>
{
    private readonly IHandler<T>[] _handlers;

    public CompositeHandler(IEnumerable<IHandler<T>> handlers)
    {
        if (handlers == null)
            throw new ArgumentNullException(nameof(handlers));
        
        _handlers = handlers.ToArray();
    }

    public void Handle(T eventData)
    {
        foreach (var handler in _handlers) 
            handler.Handle(eventData);
    }
}