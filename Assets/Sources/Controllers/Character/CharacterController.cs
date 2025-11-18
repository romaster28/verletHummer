using System;
using UnityEngine;
using Zenject;

public class CharacterController : IHandler<StartTick>, IHandler<FixedTick>
{
    private readonly ICharacterStateMachine _stateMachine;
    private readonly RopeMoveConfig _ropeMove;
    private readonly CharacterFacade _characterFacade;

    public CharacterController(ICharacterStateMachine stateMachine, SignalBus signalBus, RopeMoveConfig ropeMove, CharacterFacade characterFacade)
    {
        _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
        _ropeMove = ropeMove ?? throw new ArgumentNullException(nameof(ropeMove));
        _characterFacade = characterFacade ?? throw new ArgumentNullException(nameof(characterFacade));
        signalBus.Subscribe<RopeSpawned>(OnRopeSpawned);
    }

    public void Handle(StartTick eventData)
    {
        _stateMachine.SetState<GroundMoveState>();
    }

    public void Handle(FixedTick eventData)
    {
        if (!_stateMachine.TryGetState(out var state))
            return;
        
        state.FixedTick();
    }

    private void OnRopeSpawned(RopeSpawned signal)
    {
        if (!signal.Connected)
            return;
        
        float differenceConnectY = signal.End.y - signal.Start.y;
        
        if (differenceConnectY >= _ropeMove.MinDistanceYConnectForUpMove)
            _stateMachine.SetState<RopeSwingState>();
    }
}