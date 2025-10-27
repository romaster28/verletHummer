using UnityEngine;
using Zenject;

public class GameLoop : MonoBehaviour
{
    [Inject] private readonly IHandler<StartTick> _startHandler;
    [Inject] private readonly IHandler<Tick> _tickHandler;
    [Inject] private readonly IHandler<FixedTick> _fixedHandler;
        
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
}