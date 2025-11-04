using System;
using System.Collections.Generic;
using DR.RopeSimulation;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.Exceptions;
using Zenject;
using Object = UnityEngine.Object;

public class RopeController : IHandler<FixedTick>
{
    private readonly IAssetProvider _assetProvider;
    private readonly RopeConfig _config;
    private readonly CharacterHead _character;

    private readonly Dictionary<Rope, RopeSimulation> _ropeSimulations = new();
    private readonly Dictionary<Rope, ValueTuple<Vector3, bool>> _throwing = new();
    private readonly ReactiveCollection<Rope> _collapsing = new();
    private readonly Dictionary<Rope, RopeView> _viewMap = new();
    private readonly ObjectPool<RopeView> _viewPool;

    private readonly HashSet<Rope> _needToRemove = new();

    private const float StopThrowDistanceToTarget = 2;

    public RopeController(IAssetProvider assetProvider, RopeConfig config, SignalBus signalBus, CharacterHead character, CoreConfig coreConfig)
    {
        _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _character = character ?? throw new ArgumentNullException(nameof(character));
        _viewPool = new ObjectPool<RopeView>(Create, view => view.gameObject.SetActive(true),
            view => view.gameObject.SetActive(false), null, true, coreConfig.MaxActiveRopes + 1);

        signalBus.Subscribe<RopeSpawned>(OnRopeSpawned);
        signalBus.Subscribe<RopeDeSpawned>(OnRopeDeSpawned);
        _collapsing.ObserveRemove().Subscribe(OnEndCollapse);
    }

    private void OnRopeSpawned(RopeSpawned signal)
    {
        var view = _viewPool.Get();

        signal.Rope.Segments.ObserveReplace().Subscribe(replaced =>
        {
            view.UpdateSegment(replaced.Index, replaced.NewValue);
        }).AddTo(view);

        var simulation = new RopeSimulation(_config.ToRopeConfig(), signal.Start);
        var targetThrowPoint = signal.End;

        _ropeSimulations.Add(signal.Rope, simulation);
        _throwing.Add(signal.Rope, new ValueTuple<Vector3, bool>(targetThrowPoint, signal.Connected));
        _viewMap.Add(signal.Rope, view);
    }

    private void OnRopeDeSpawned(RopeDeSpawned signal)
    {
        if (!_viewMap.ContainsKey(signal.Rope))
            throw new OperationException($"Not founded view for rope: {signal.Rope}");

        _collapsing.Add(signal.Rope);
    }

    private void OnEndCollapse(CollectionRemoveEvent<Rope> item)
    {
        if (!_viewMap.ContainsKey(item.Value))
            throw new OperationException($"Not founded view for rope: {item.Value}");
        
        _needToRemove.Add(item.Value);
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
            else if (_collapsing.Contains(rope))
            {
                var currentPoint = simulation.GetSegment(simulation.SegmentsCount - 1);
                var direction = (simulation.GetSegment(0) - currentPoint).normalized;
                
                var newPoint = currentPoint + direction * (_config.ThrowConfig.ThrowSpeed * _config.ThrowConfig.CollapseModifier * Time.fixedDeltaTime);
                simulation.BlockSegment(simulation.SegmentsCount - 1, newPoint);
                
                if (Vector3.Distance(newPoint, simulation.GetSegment(0)) <= StopThrowDistanceToTarget)
                {
                    _collapsing.Remove(rope);
                    simulation.ReleaseSegment(simulation.SegmentsCount - 1);
                }
            }

            for (int i = 0; i < simulation.SegmentsCount; i++)
                rope.Segments[i] = simulation.GetSegment(i);
        }

        foreach (var rope in _needToRemove)
        {
            _viewPool.Release(_viewMap[rope]);
            _ropeSimulations.Remove(rope);
        }
        
        _needToRemove.Clear();
    }

    private RopeView Create()
    {
        var view = _assetProvider.LoadAsset<RopeView>("Level/RopeView");
        var spawned = Object.Instantiate(view);
        return spawned;
    }
}