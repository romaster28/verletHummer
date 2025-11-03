using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Rope
{
    private readonly ReactiveCollection<Vector3> _segments;
    
    public Rope(IEnumerable<Vector3> points)
    {
        _segments = points.ToReactiveCollection();
    }

    public Vector3 this[int index]
    {
        get => _segments[index];
        set => _segments[index] = value;
    }
    
    public ReactiveCollection<Vector3> Segments => _segments;
}