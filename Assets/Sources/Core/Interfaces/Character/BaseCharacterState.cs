using System;

public abstract class BaseCharacterState
{
    public abstract void Enter();
    public abstract void Exit();
    public abstract void FixedTick();
}