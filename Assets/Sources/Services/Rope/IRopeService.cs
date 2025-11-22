using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IRopeService
{
    int SpawnedCount { get; }
    Rope Spawn(Vector3 start, Vector3 direction, bool connected);
    void Disconnect(Rope rope);
    void DeSpawn(Rope rope);
    IEnumerable<Rope> GetSpawned();
    bool IsActive(Rope rope);
}

public class RopeService : IRopeService
{
    private readonly SignalBus _signalBus;
    private readonly List<Vector3> _cacheSegments = new();
    private readonly List<Rope> _spawned = new();
    private readonly RopeConfig _config;

    private Rope _first;
    
    public RopeService(SignalBus signalBus, RopeConfig config)
    {
        _signalBus = signalBus ?? throw new ArgumentNullException(nameof(signalBus));
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public int SpawnedCount => _spawned.Count;

    public Rope Spawn(Vector3 start, Vector3 end, bool connected)
    {
        for (int i = 0; i < _config.ToRopeConfig().Segments; i++) 
            _cacheSegments.Add(i == 0 ? start : Vector3.zero);

        var result = new Rope(_cacheSegments, end);
        _spawned.Add(result);
        _signalBus.Fire(new RopeSpawned(result, start, end, connected));
        _cacheSegments.Clear();
        return result;
    }

    public void Disconnect(Rope rope)
    {
        if (!_spawned.Contains(rope))
            throw new InvalidOperationException($"Cant find rope {rope}");
        
        _signalBus.Fire(new RopeDisconnected(rope));
    }

    public void DeSpawn(Rope rope)
    {
        if (rope == null)
            throw new ArgumentNullException(nameof(rope));
        
        if (!_spawned.Contains(rope))
            throw new InvalidOperationException($"Cant find rope {rope}");
        
        _signalBus.Fire(new RopeDeSpawned(rope));
        _spawned.Remove(rope);
    }

    public bool IsActive(Rope rope)
    {
        if (rope == null)
            return false;

        return _spawned.Contains(rope);
    }
    
    public IEnumerable<Rope> GetSpawned() => _spawned;
}