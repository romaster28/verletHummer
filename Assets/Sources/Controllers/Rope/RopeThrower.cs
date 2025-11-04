using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class RopeThrower
{
    private readonly IRopeService _ropeService;
    private readonly ICameraService _cameraService;
    private readonly RopeConfig _config;
    private readonly CoreConfig _core;
    private readonly CharacterHead _character;

    private readonly HashSet<Rope> _broken = new();
    
    private const float CheckBreakDistanceInterval = .3f;
    
    public RopeThrower(IRopeService ropeService, ICameraService cameraService, RopeConfig config, CharacterHead character, CoreConfig coreConfig)
    {
        _ropeService = ropeService ?? throw new ArgumentNullException(nameof(ropeService));
        _cameraService = cameraService ?? throw new ArgumentNullException(nameof(cameraService));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _character = character ?? throw new ArgumentNullException(nameof(character));
        _core = coreConfig ?? throw new ArgumentNullException(nameof(coreConfig));

        Observable.Interval(TimeSpan.FromSeconds(CheckBreakDistanceInterval)).Subscribe(OnCheckBreakDistanceInterval);
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
        
        if (_ropeService.SpawnedCount > _core.MaxActiveRopes)
            _ropeService.DeSpawn(_ropeService.GetSpawned().First());
        
        if (!successConnected)
            _ropeService.DeSpawn(_ropeService.GetSpawned().Last());
    }

    private void OnCheckBreakDistanceInterval(long tick)
    {
        foreach (var spawned in _ropeService.GetSpawned())
        {
            float distanceToHead = Vector3.Distance(_character.Position.Value, spawned.Segments.Last());

            if (distanceToHead >= _config.ThrowConfig.BreakRopeDistance)
                _broken.Add(spawned);
        }

        foreach (var broke in _broken) 
            _ropeService.DeSpawn(broke);
        
        _broken.Clear();
    }
}