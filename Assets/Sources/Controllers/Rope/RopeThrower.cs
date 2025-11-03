using System;
using UnityEngine;

public class RopeThrower
{
    private readonly IRopeService _ropeService;
    private readonly ICameraService _cameraService;
    private readonly RopeConfig _config;
    private readonly Character _character;

    public RopeThrower(IRopeService ropeService, ICameraService cameraService, RopeConfig config, Character character)
    {
        _ropeService = ropeService ?? throw new ArgumentNullException(nameof(ropeService));
        _cameraService = cameraService ?? throw new ArgumentNullException(nameof(cameraService));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _character = character ?? throw new ArgumentNullException(nameof(character));
    }

    public void ThrowToCrosshair()
    {
        Vector3 endPoint = _cameraService.GetPosition() + _cameraService.GetForward() * _config.ThrowConfig.MaxConnectDistance;
        
        bool successConnected = Physics.Raycast(_cameraService.GetPosition(), _cameraService.GetForward(), out var hitInfo, _config.ThrowConfig.MaxConnectDistance, _config.ConnectLayer);

        if (successConnected)
        {
            endPoint = hitInfo.point;
            Debug.Log($"Connected rope");
        }
        
        _ropeService.Spawn(_character.Position.Value, endPoint, successConnected);
    }
}