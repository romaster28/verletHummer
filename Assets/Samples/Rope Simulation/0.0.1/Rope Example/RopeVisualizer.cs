using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class RopeVisualizer : MonoBehaviour
{
    [SerializeField] private int _segments;
    [SerializeField] private float _segmentLength;
    [SerializeField] private float _damping;
    [SerializeField] private int _numOfConstraintRuns;
    [SerializeField] private Vector3 _gravity;
    
    [SerializeField] private LineRenderer _renderer;

    [SerializeField] private Transform _start;
    [SerializeField] private Transform _end;
    
    private RopeSimulation _simulation;

    private void FixedUpdate()
    {
        if (_start.gameObject.activeSelf)
            _simulation.BlockSegment(0, _start.position);
        else
            _simulation.ReleaseSegment(0);
        
        if (_end.gameObject.activeSelf)
            _simulation.BlockSegment(_segments - 1, _end.position);
        else
            _simulation.ReleaseSegment(_segments - 1);
        
        _simulation.Simulate();
    }

    private void Update()
    {
        _renderer.positionCount = _segments;
        
        int index = 0;
        
        foreach (var segment in _simulation.GetSegments())
        {
            _renderer.SetPosition(index, segment);
            index++;
        }
    }

    private void Awake()
    {
        UpdateConfig();
    }

    private void OnValidate()
    {
        UpdateConfig();
    }

    private void UpdateConfig()
    {
        var config = new RopeConfig(_segments, _segmentLength, _damping, _numOfConstraintRuns, _gravity);
        _simulation = new RopeSimulation(config, _start.position);
    }
}