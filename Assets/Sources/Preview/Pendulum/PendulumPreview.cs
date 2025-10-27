using System;
using UnityEngine;

public class PendulumPreview : MonoBehaviour
{
    [SerializeField] private Transform _pivot;
    [SerializeField] private Transform _bob;
    [SerializeField] private float _initialAngle = 40;
    [SerializeField] private float _lineLength = 10;
    [SerializeField] private float _gravityForce = 9.18f;
    [SerializeField] private float _damping = .5f;

    [SerializeField] private float _theta;
    [SerializeField] private float _omega;

    [SerializeField] private bool _emptyOmega;

    private void Start()
    {
        _theta = _initialAngle * Mathf.Deg2Rad;
        _omega = 0;
        
        UpdatePosition();
    }

    private void FixedUpdate()
    {
        float alpha = -(_gravityForce / _lineLength) * Mathf.Sin(_theta) - _damping * _omega;

        _omega += alpha * Time.fixedDeltaTime;

        if (_emptyOmega)
            _omega = 0;
        
        _theta += _omega * Time.fixedDeltaTime;
        
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        _bob.position = new Vector3(_pivot.position.x + _lineLength * Mathf.Sin(_theta), _pivot.position.y - _lineLength * Mathf.Cos(_theta), _pivot.position.z);
    }
}