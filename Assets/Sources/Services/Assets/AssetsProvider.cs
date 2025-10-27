using System.Collections.Generic;
using UnityEngine;

public interface IAssetProvider
{
    T LoadAsset<T>(string path) where T : Object;
    IEnumerable<T> LoadAssets<T>(string folderPath) where T : Object;
    void UnloadAsset<T>(T asset) where T : Object;
}