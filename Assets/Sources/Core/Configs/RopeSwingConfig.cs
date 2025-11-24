using System;
using UnityEngine;

[Serializable]
public class RopeSwingConfig
{
    [SerializeField] private float _minDistanceYConnectForUpMove = 10;
    [SerializeField] private float _gravity = 9.8f;
    [Min(0)] [SerializeField] private float _damping = 0.1f;
    [Min(0)] [SerializeField] private float _initialSwingForce = 80f;
    [Range(0, 100)] [SerializeField] private int _forceCropRopeDistancePercentage = 10; 

    public float MinDistanceYConnectForUpMove => _minDistanceYConnectForUpMove;
    public float Gravity => _gravity;
    public float Damping => _damping;
    public float InitialSwingForce => _initialSwingForce;
    public int ForceCropRopeDistancePercentage => _forceCropRopeDistancePercentage;
}