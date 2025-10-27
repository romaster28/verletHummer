using System;
using UnityEngine;

[RequireComponent(typeof(RopeLineRendererVisualizer))]
public class TwoPointsRopePreview : MonoBehaviour
{
    [SerializeField] private RopeConfigSerializable _config;

    [SerializeField] private Transform _first;
    [SerializeField] private Transform _second;
    
    private RopeSimulation _rope;
    private RopeLineRendererVisualizer _visualizer;

    private void Awake()
    {
        _rope = new RopeSimulation(_config, _first.position);
        _visualizer = GetComponent<RopeLineRendererVisualizer>();
        _visualizer.SetRope(_rope);
    }

    private void Update()
    {
        _rope.BlockSegment(0, _first.position);
        _rope.BlockSegment(_rope.SegmentsCount - 1, _second.position);
    }
}