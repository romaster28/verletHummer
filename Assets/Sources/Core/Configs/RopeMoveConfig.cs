using System;
using UnityEngine;

[Serializable]
public class RopeMoveConfig
{
    [SerializeField] private float _minDistanceYConnectForUpMove = 10;
    [SerializeField] private float _gravity = 9.8f;
    [Min(0)] [SerializeField] private float _damping = 0.1f;
    [Min(0)] [SerializeField] private float _initialSwingForce = 80f;

    public float MinDistanceYConnectForUpMove => _minDistanceYConnectForUpMove;
    public float Gravity => _gravity;
    public float Damping => _damping;
    public float InitialSwingForce => _initialSwingForce;
}