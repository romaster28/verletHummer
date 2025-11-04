using System;
using UnityEngine;

[Serializable]
public class CoreConfig
{
    [SerializeField] private int _maxActiveRopes = 3;

    public int MaxActiveRopes => _maxActiveRopes;
}