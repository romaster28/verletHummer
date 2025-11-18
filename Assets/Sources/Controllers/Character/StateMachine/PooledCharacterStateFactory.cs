using System;
using System.Linq;
using Zenject;

public class PooledCharacterStateFactory : IFactory<Type, BaseCharacterState>
{
    private readonly BaseCharacterState[] _pool;

    public PooledCharacterStateFactory(DiContainer container)
    {
        _pool = new BaseCharacterState[]
        {
            container.Instantiate<GroundMoveState>(),
            container.Instantiate<RopeSwingState>()
        };
    }

    public BaseCharacterState Create(Type type)
    {
        return _pool.FirstOrDefault(state => state.GetType() == type);
    }
}

public class CharacterStateFactory : PlaceholderFactory<Type, BaseCharacterState>
{
}