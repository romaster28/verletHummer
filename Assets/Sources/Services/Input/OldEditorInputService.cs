using System;
using System.Linq;
using UniRx;
using UnityEngine;

public class OldEditorInputService : IInputService
{
    private readonly IInputHandler[] _handlers;

    public OldEditorInputService()
    {
        var moveProperty = new ReactiveProperty<Vector2>(Vector2.zero);
        
        _handlers = new IInputHandler[]
        {
            new MoveInput(moveProperty)
        };

        Observable.EveryUpdate().Subscribe(_ =>
        {
            moveProperty.Value = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        });
    }

    public T GetHandler<T>() where T : IInputHandler
    {
        T handler = (T)_handlers.First(x => x is T);
        
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));
        
        return handler;
    }
}