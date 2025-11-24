using System;
using UnityEngine;
using Zenject;

public class CharacterController : IHandler<StartTick>, IHandler<FixedTick>
{
    private readonly ICharacterStateMachine _stateMachine;

    public CharacterController(ICharacterStateMachine stateMachine)
    {
        _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));
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
}