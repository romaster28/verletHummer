using System;

public class RopeMoveState : ICharacterState
{
    private readonly Character _character;

    public RopeMoveState(Character character)
    {
        _character = character ?? throw new ArgumentNullException(nameof(character));
    }

    public void Enter()
    {
        
    }

    public void Exit()
    {
        
    }

    public void FixedTick()
    {
        
    }
}