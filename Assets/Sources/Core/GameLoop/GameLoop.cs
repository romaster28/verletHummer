using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameLoop : MonoBehaviour
{
    [Inject] private readonly IHandler<StartTick> _startHandler;
    [Inject] private readonly IHandler<Tick> _tickHandler;
    [Inject] private readonly IHandler<FixedTick> _fixedHandler;
    [Inject] private readonly IHandler<DestroyTick> _destroyHandler;
        
    private void Start()
    {
        _startHandler.Handle(new StartTick());
    }

    private void Update()
    {
        _tickHandler.Handle(new Tick());
    }

    private void FixedUpdate()
    {
        _fixedHandler.Handle(new FixedTick());
    }

    private void OnDestroy()
    {
        _destroyHandler.Handle(new DestroyTick());
    }
}

public class StartHandler : CompositeHandler<StartTick> { public StartHandler(IEnumerable<IHandler<StartTick>> handlers) : base(handlers) { } }
public class TickHandler : CompositeHandler<Tick> { public TickHandler(IEnumerable<IHandler<Tick>> handlers) : base(handlers) { } }
public class FixedTickHandler : CompositeHandler<FixedTick> { public FixedTickHandler(IEnumerable<IHandler<FixedTick>> handlers) : base(handlers) { } }
public class DestroyTickHandler : CompositeHandler<DestroyTick> { public DestroyTickHandler(IEnumerable<IHandler<DestroyTick>> handlers) : base(handlers) { } }