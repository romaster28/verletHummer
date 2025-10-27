using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class ResourcesAssetProvider : IAssetProvider
{
    [SerializeField, FoldoutGroup("Asset Paths")]
    private List<string> _assetPaths = new List<string>(); // Пути к активам для редактора

    private readonly Dictionary<string, Object> _loadedAssets = new Dictionary<string, Object>();

    [Inject]
    private void Construct()
    {
        Debug.Log("AssetProvider initialized");
    }

    public T LoadAsset<T>(string path) where T : Object
    {
        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Asset path is empty");
            return null;
        }
        
        if (_loadedAssets.TryGetValue(path, out var cachedAsset) && cachedAsset is T typedAsset)
        {
            return typedAsset;
        }
        
        var asset = Resources.Load<T>(path);
        if (asset == null)
        {
            Debug.LogError($"Failed to load asset at path: {path}");
            return null;
        }

        _loadedAssets[path] = asset;
        return asset;
    }

    public IEnumerable<T> LoadAssets<T>(string folderPath) where T : Object
    {
        var assets = Resources.LoadAll<T>(folderPath);
        
        foreach (var asset in assets)
        {
            _loadedAssets.TryAdd(asset.name, asset);
            yield return asset;
        }
    }

    public void UnloadAsset<T>(T asset) where T : Object
    {
        if (asset == null) return;

        var path = _loadedAssets.FirstOrDefault(x => x.Value == asset).Key;
        
        if (string.IsNullOrEmpty(path)) 
            return;
        
        _loadedAssets.Remove(path);
        Resources.UnloadAsset(asset);
    }

#if UNITY_EDITOR
    [Button("Preload Assets"), FoldoutGroup("Asset Paths")]
    private void PreloadAssetsInEditor()
    {
        foreach (var path in _assetPaths)
        {
            var asset = LoadAsset<Object>(path);
            if (asset != null)
            {
                Debug.Log($"Preloaded asset: {path}");
            }
        }
    }

    [Button("Clear Cache"), FoldoutGroup("Asset Paths")]
    private void ClearCacheInEditor()
    {
        foreach (var asset in _loadedAssets.Values)
        {
            Resources.UnloadAsset(asset);
        }
        _loadedAssets.Clear();
        Debug.Log("Asset cache cleared");
    }
#endif
}