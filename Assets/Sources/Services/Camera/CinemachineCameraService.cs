using System;
using Unity.Cinemachine;
using UnityEngine;
using Object = UnityEngine.Object;

public interface ICameraService
{
    void StartFollow(Transform target);
    void StartFollow(TransformModel target);
    Vector3 GetPosition();
    Vector3 GetForward();
}

public class CinemachineCameraService : ICameraService
{
    private const string CameraPath = "Level/Camera/TPA";
    
    private readonly IAssetProvider _assetProvider;

    private CinemachineCamera _camera;
    private Transform _followTarget;

    public CinemachineCameraService(IAssetProvider assetProvider)
    {
        _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
    }

    public void StartFollow(Transform target)
    {
        TryCreate();
        _camera.Target.TrackingTarget = target;
    }

    public void StartFollow(TransformModel model)
    {
        if (_followTarget == null)
            _followTarget = new GameObject("FollowTarget").transform;
        
        model.PositionUpdated += pos => _followTarget.position = pos;
        model.RotationUpdated += rot => _followTarget.rotation = rot;
        StartFollow(_followTarget);
    }

    public Vector3 GetPosition()
    {
        return _camera.transform.position;
    }

    public Vector3 GetForward()
    {
        return _camera.transform.forward;        
    }

    private void TryCreate()
    {
        if (_camera != null)
            return;
        
        var prefab = _assetProvider.LoadAsset<CinemachineCamera>(CameraPath);
        _camera = Object.Instantiate(prefab);
    }
}