using System;
using System.Collections.Generic;
using DR.RopeSimulation;
using UniRx;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

public class RopeController : IHandler<FixedTick>
{
    private readonly IAssetProvider _assetProvider;
    private readonly ICameraService _cameraService;
    private readonly RopeConfig _config;
    private readonly Character _character;

    private readonly Dictionary<Rope, RopeSimulation> _ropeSimulations = new();
    private readonly Dictionary<Rope, ValueTuple<Vector3, bool>> _throwing = new();

    private const float StopThrowDistanceToTarget = 2;

    public RopeController(IAssetProvider assetProvider, RopeConfig config, SignalBus signalBus, Character character,
        ICameraService cameraService)
    {
        _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _character = character ?? throw new ArgumentNullException(nameof(character));
        _cameraService = cameraService ?? throw new ArgumentNullException(nameof(cameraService));

        signalBus.Subscribe<RopeSpawned>(OnRopeSpawned);
    }

    private void OnRopeSpawned(RopeSpawned signal)
    {
        var view = _assetProvider.LoadAsset<RopeView>("Level/RopeView");
        var spawned = Object.Instantiate(view);

        signal.Rope.Segments.ObserveReplace().Subscribe(replaced =>
        {
            spawned.UpdateSegment(replaced.Index, replaced.NewValue);
        }).AddTo(view);

        var simulation = new RopeSimulation(_config.ToRopeConfig(), signal.Start);
        var targetThrowPoint = _cameraService.GetPosition() +
                               _cameraService.GetForward() * _config.ThrowConfig.MaxConnectDistance;

        _ropeSimulations.Add(signal.Rope, simulation);
        _throwing.Add(signal.Rope,
            new ValueTuple<Vector3, bool>(targetThrowPoint, signal.Connected));
    }

    public void Handle(FixedTick eventData)
    {
        foreach (var ropeSimulationPair in _ropeSimulations)
        {
            var rope = ropeSimulationPair.Key;
            var simulation = ropeSimulationPair.Value;

            simulation.Simulate();
            simulation.BlockSegment(0, _character.Position.Value);

            if (_throwing.TryGetValue(rope, out var throwData))
            {
                var currentPoint = simulation.GetSegment(simulation.SegmentsCount - 1);
                var direction = (throwData.Item1 - currentPoint).normalized;
                
                var newPoint = currentPoint + direction * (_config.ThrowConfig.ThrowSpeed * Time.fixedDeltaTime);
                simulation.BlockSegment(simulation.SegmentsCount - 1, newPoint);

                if (Vector3.Distance(newPoint, throwData.Item1) <= StopThrowDistanceToTarget)
                {
                    _throwing.Remove(rope);
                    
                    if (!throwData.Item2)
                        simulation.ReleaseSegment(simulation.SegmentsCount - 1);
                    else
                        simulation.BlockSegment(simulation.SegmentsCount - 1, throwData.Item1);
                }
            }

            for (int i = 0; i < simulation.SegmentsCount; i++)
                rope.Segments[i] = simulation.GetSegment(i);
        }
    }
}