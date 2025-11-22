using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Pool;
using Zenject;
using Object = UnityEngine.Object;

public class RopePresenter
{
    private readonly IAssetProvider _assetProvider;
    private readonly Dictionary<Rope, RopeView> _viewMap = new();
    private readonly ObjectPool<RopeView> _viewPool;

    public RopePresenter(IAssetProvider assetProvider, CoreConfig coreConfig, SignalBus signalBus)
    {
        _assetProvider = assetProvider ?? throw new ArgumentNullException(nameof(assetProvider));
        _viewPool = new ObjectPool<RopeView>(Create, view => view.gameObject.SetActive(true),
            view => view.gameObject.SetActive(false), null, true, coreConfig.MaxActiveRopes + 1);
        
        signalBus.Subscribe<RopeSpawned>(Callback);
        signalBus.Subscribe<RopeDeSpawned>(Callback);
    }

    private void Callback(RopeSpawned signal)
    {
        var view = _viewPool.Get();

        signal.Rope.Segments.ObserveReplace().Subscribe(replaced =>
        {
            view.UpdateSegment(replaced.Index, replaced.NewValue);
        }).AddTo(view);
        _viewMap.Add(signal.Rope, view);
    }

    private void Callback(RopeDeSpawned signal)
    {
        Object.Destroy(_viewMap[signal.Rope].gameObject);
        _viewMap.Remove(signal.Rope);
    }

    private RopeView Create()
    {
        var view = _assetProvider.LoadAsset<RopeView>("Level/RopeView");
        var spawned = Object.Instantiate(view);
        return spawned;
    }
}