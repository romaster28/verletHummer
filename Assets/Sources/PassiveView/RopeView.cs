using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RopeView : MonoBehaviour
{
    [SerializeField] private LineRenderer _renderer;
    
    public void UpdateSegments(IEnumerable<Vector3> segments)
    {
        if (segments.Count() > _renderer.positionCount)
            _renderer.positionCount = segments.Count();
        
        _renderer.SetPositions(segments.ToArray());
    }

    public void UpdateSegment(int index, Vector3 segment)
    {
        if (index >= _renderer.positionCount)
            _renderer.positionCount = index + 1;
        
        _renderer.SetPosition(index, segment);
    }
}