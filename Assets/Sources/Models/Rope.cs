using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Rope
{
    private readonly ReactiveCollection<Vector3> _segments;
    
    public Rope(IEnumerable<Vector3> points, Vector3 target)
    {
        _segments = points.ToReactiveCollection();
        Target = target;
    }

    public Vector3 this[int index]
    {
        get => _segments[index];
        set => _segments[index] = value;
    }
    
    public Vector3 Target { get; }
    
    public ReactiveCollection<Vector3> Segments => _segments;
}