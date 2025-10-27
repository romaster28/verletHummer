using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterStatesCachedPoolProvider
{
    private readonly ICharacterState[] _pool;

    public CharacterStatesCachedPoolProvider(IEnumerable<ICharacterState> pool)
    {
        if (pool == null)
            throw new ArgumentNullException(nameof(pool));
        
        _pool = pool.ToArray();
        
        Debug.Log($"Const {_pool.Length}, {pool.Count()}");
    }

    public T GetState<T>() where T : ICharacterState
    {
        if (!TryFind<T>(out var result))
            throw new ArgumentException($"Cant find state of {typeof(T)} type");

        return result;
    }

    private bool TryFind<T>(out T result) where T : ICharacterState
    {
        Debug.Log(_pool.Length);
        
        result = (T)_pool.First(x => x is T);
        return result != null;
    }
}