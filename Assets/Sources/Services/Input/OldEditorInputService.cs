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
        var jumpProperty = new ReactiveProperty<bool>(false);
        var lookProperty = new ReactiveProperty<Vector2>(Vector2.zero);
        var throwRopeProperty = new ReactiveProperty<bool>(false);
        
        _handlers = new IInputHandler[]
        {
            new MoveInput(moveProperty),
            new JumpInput(jumpProperty),
            new LookInput(lookProperty),
            new ThrowRopeInput(throwRopeProperty)
        };

        Observable.EveryUpdate().Subscribe(_ =>
        {
            moveProperty.Value = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            lookProperty.Value = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            jumpProperty.Value = Input.GetKeyDown(KeyCode.Space);
            throwRopeProperty.Value = Input.GetKeyDown(KeyCode.LeftAlt);
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