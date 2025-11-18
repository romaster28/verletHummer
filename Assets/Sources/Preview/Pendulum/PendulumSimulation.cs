using UniRx;
using UnityEngine;

public class PendulumSimulation
{
    private readonly PendulumConfig _config;
    private readonly ReactiveProperty<Vector3> _pivot;
    private readonly ReactiveProperty<Vector3> _bob;
    private readonly ReactiveProperty<Vector3> _swingAxis;
    private readonly float _length;

    private float _theta;
    private float _omega;

    public PendulumSimulation(PendulumConfig config, float initialSwingImpulse = 0f)
    {
        _config = config;

        _pivot = new ReactiveProperty<Vector3>(config.Pivot);
        _bob = new ReactiveProperty<Vector3>(config.BobStart);
        _length = Vector3.Distance(config.BobStart, config.Pivot);

        Vector3 toBob = config.BobStart - config.Pivot;
        Vector3 horizontal = new Vector3(toBob.x, 0, toBob.z).normalized;
        if (horizontal.sqrMagnitude < 0.01f) horizontal = Vector3.forward;

        _swingAxis = new ReactiveProperty<Vector3>(horizontal);

        CalculateThetaFromCurrentPosition();
        _omega = 0f;
        
        if (Mathf.Abs(initialSwingImpulse) > 0.001f) 
            AddSwingForce(initialSwingImpulse);
    }

    public IReadOnlyReactiveProperty<Vector3> Bob => _bob;
    public ReactiveProperty<Vector3> SwingAxis => _swingAxis;

    private bool _isInitialized;

    public void Simulate()
    {
        EnsureInitialized();

        float alpha = -(_config.Gravity / _length) * Mathf.Sin(_theta)
                      - _config.AirDamping * _omega;

        _omega += alpha * Time.fixedDeltaTime;
        _theta += _omega * Time.fixedDeltaTime;
        _theta = Mathf.Clamp(_theta, -Mathf.PI * 0.99f, Mathf.PI * 0.99f);

        Vector3 axis = _swingAxis.Value.normalized;
        Vector3 planeNormal = Vector3.Cross(axis, Vector3.up).normalized;
        Vector3 ropeDirection = Quaternion.AngleAxis(_theta * Mathf.Rad2Deg, planeNormal) * Vector3.down;

        _bob.Value = _pivot.Value + ropeDirection * _length;
    }

    private void EnsureInitialized()
    {
        if (_isInitialized) return;
        CalculateThetaFromCurrentPosition();
        _isInitialized = true;
    }

    private void CalculateThetaFromCurrentPosition()
    {
        Vector3 toBob = _bob.Value - _pivot.Value;
        if (toBob.sqrMagnitude < 0.0001f)
        {
            _theta = 0f;
            return;
        }

        Vector3 dir = toBob.normalized;
        Vector3 axis = _swingAxis.Value.normalized;
        Vector3 planeNormal = Vector3.Cross(axis, Vector3.up).normalized;

        Vector3 downInPlane = Vector3.ProjectOnPlane(Vector3.down, planeNormal);
        if (downInPlane.sqrMagnitude < 0.01f) { _theta = 0f; return; }
        downInPlane.Normalize();

        Vector3 bobInPlane = Vector3.ProjectOnPlane(dir, planeNormal);
        if (bobInPlane.sqrMagnitude < 0.01f) { _theta = 0f; return; }
        bobInPlane.Normalize();

        _theta = Vector3.SignedAngle(downInPlane, bobInPlane, planeNormal) * Mathf.Deg2Rad;
    }
    
    public void AddSwingForce(float force)
    {
        float angularImpulse = force * Time.fixedDeltaTime / _length;
        _omega += angularImpulse;
    }
    
    public void SetSwingVelocity(float angularVelocityRadPerSec)
    {
        _omega = angularVelocityRadPerSec;
    }
}

public struct PendulumConfig
{
    public float Gravity { get; }
    public float AirDamping { get; }
    public Vector3 Pivot { get; }
    public Vector3 BobStart { get; }

    public PendulumConfig(Vector3 bobStart, Vector3 pivot, float gravity = 9.8f, float airDamping = 0f)
    {
        Gravity = gravity;
        AirDamping = airDamping;
        Pivot = pivot;
        BobStart = bobStart;
    }
}