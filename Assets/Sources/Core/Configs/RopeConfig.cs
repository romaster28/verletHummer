using System;
using UnityEngine;

[Serializable]
public class RopeConfig
{
    [Range(1, 100)] [SerializeField] private float _accuracy = 50;
    [Min(0)] [SerializeField] private float _elastic = 25;
    [SerializeField] private float _damping = 0.6f;
    [SerializeField] private LayerMask _connectLayer;
    [SerializeField] private Vector3 _gravity = new(0, -9.84f, 0);
    [SerializeField] private RopeThrowConfig _throwConfig;
    [SerializeField] private int _segments = 15;
    
    public float Accuracy => _accuracy;
    public float Elastic => _elastic;
    public float Damping => _damping;
    public Vector3 Gravity => _gravity;
    public LayerMask ConnectLayer => _connectLayer;
    public RopeThrowConfig ThrowConfig => _throwConfig;
    public int Segments => _segments;
}

[Serializable]
public class RopeThrowConfig
{
    [SerializeField] private float _maxConnectDistance = 25;
    [SerializeField] private float _throwSpeed = 6;

    public float MaxConnectDistance => _maxConnectDistance;
    public float ThrowSpeed => _throwSpeed;
}

public static class RopeConfigExtensions
{
    public static DR.RopeSimulation.RopeConfig ToRopeConfig(this RopeConfig original)
    {
        float segmentLength = Mathf.Lerp(0.2f, 1f, original.Accuracy / 100f);
        int numOfConstraints = 10;
        return new DR.RopeSimulation.RopeConfig(original.Segments, segmentLength, original.Damping, numOfConstraints, original.Gravity);
    }
}