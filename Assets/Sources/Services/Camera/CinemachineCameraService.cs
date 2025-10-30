using System;
using Unity.Cinemachine;
using UnityEngine;
using Object = UnityEngine.Object;

public interface ICameraService
{
    void StartFollow(Transform target);
}

public class CinemachineCameraService : ICameraService
{
    private const string CameraPath = "Level/Camera/TPA";
    
    private readonly IAssetProvider _assetProvider;

    private CinemachineCamera _camera;

    public CinemachineCameraService(IAssetProvider assetProvider)
    {
        _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
    }

    public void StartFollow(Transform target)
    {
        TryCreate();
        _camera.Target.TrackingTarget = target;
    }

    private void TryCreate()
    {
        if (_camera != null)
            return;
        
        var prefab = _assetProvider.LoadAsset<CinemachineCamera>(CameraPath);
        _camera = Object.Instantiate(prefab);
    }
}