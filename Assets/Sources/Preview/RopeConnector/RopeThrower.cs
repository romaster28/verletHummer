using UnityEngine;

public class RopeThrower : MonoBehaviour
{
    [SerializeField] private RopeLineRendererVisualizer _visualize;
    [SerializeField] private RopeConfigSerializable _config;
    [SerializeField] private Transform _anchor;
    [SerializeField] private Vector3 _testDirection;
    [SerializeField] private float _throwSpeed = 5;
    [SerializeField] private Transform _camera;
    [SerializeField] private float _cameraStoppingDistance = 15;
    [SerializeField] private LayerMask _connectLayer;
    [SerializeField] private float _connectDistanceTargetCheck = 1;
    
    private RopeSimulation _ropeSimulation;
    private Vector3 _targetThrowPoint;
    private Vector3 _throwerPoint;
    private Vector3 _throwDirection;
    private bool _throwing;
    private bool _successConnected;

    public void Throw(Vector3 from, Vector3 direction)
    {
        direction = direction.normalized;
        _ropeSimulation = new RopeSimulation(_config, from);
        _throwDirection = direction;
        _throwerPoint = from;
        _visualize.SetRope(_ropeSimulation);
        _throwing = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) && !_throwing)
        {
            _targetThrowPoint = _camera.position + _camera.forward * _cameraStoppingDistance;
            _successConnected = Physics.Raycast(_camera.position, _camera.forward, out var hitInfo,
                _cameraStoppingDistance, _connectLayer);
            
            if (_successConnected)
            {
                _targetThrowPoint = hitInfo.point;
                Debug.Log($"Connecting");
            }
            
            Vector3 direction = (_targetThrowPoint - _anchor.position).normalized;
            _targetThrowPoint += direction * (_connectDistanceTargetCheck * 2);
            Throw(_anchor.position, direction);
        }
        
        if (_ropeSimulation == null)
            return;
        
        _ropeSimulation.BlockSegment(0, _anchor.position);
        
        if (!_throwing)
            return;

        _throwerPoint += _throwDirection * (_throwSpeed * Time.deltaTime);
        _ropeSimulation.BlockSegment(_ropeSimulation.SegmentsCount - 1, _throwerPoint);
        
        if (Vector3.Distance(_throwerPoint, _targetThrowPoint) <= _connectDistanceTargetCheck)
        {
            _throwing = false;
            
            if (!_successConnected)
                _ropeSimulation.ReleaseSegment(_ropeSimulation.SegmentsCount - 1);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 cameraDistancePoint = _camera.position + _camera.forward * _cameraStoppingDistance;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(cameraDistancePoint, .3f);
    }
}