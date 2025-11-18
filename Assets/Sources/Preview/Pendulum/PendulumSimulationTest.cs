using System;
using UniRx;
using UnityEngine;

public class PendulumSimulationTest : MonoBehaviour
{
    [SerializeField] private Transform _pivot;
    [SerializeField] private Transform _bob;
    [SerializeField] private Vector3 _setSwingAxis;
    [SerializeField] private float _force = 15;

    private PendulumSimulation _simulation;

    [ContextMenu("AddForce")]
    private void AddForce()
    {
        _simulation.AddSwingForce(_force);
    }
    
    private void Awake()
    {
        var config = new PendulumConfig(_bob.position, _pivot.position);
        _simulation = new PendulumSimulation(config, initialSwingImpulse: _force);
        _simulation.Bob.Subscribe(OnNext);
    }

    private void OnNext(Vector3 obj)
    {
        _bob.position = obj;
    }

    private void Update()
    {
        _simulation.SwingAxis.Value = _setSwingAxis;
    }

    private void FixedUpdate()
    {
        _simulation.Simulate();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(_bob.position, _pivot.position);
    }
    
    private void OnValidate()
    {
        if (Application.isPlaying)
            return;

        _setSwingAxis = (_bob.position - _pivot.position).normalized;
        _setSwingAxis.y = 0;
    }
}