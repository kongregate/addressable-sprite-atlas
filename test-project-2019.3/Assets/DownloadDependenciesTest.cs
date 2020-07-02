using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DownloadDependenciesTest : MonoBehaviour
{
    IEnumerator Start()
    {
        if (!Caching.ClearCache())
        {
            throw new Exception("Failed to clear bundle cache, won't be able to download bundles");
        }

        yield return DownloadDependenciesFor("happy");
        yield return LoadSpriteTest.TryLoadSprite("happy");

        yield return DownloadDependenciesFor("sad");
        yield return LoadSpriteTest.TryLoadSprite("sad");
    }

    public static IEnumerator DownloadDependenciesFor(string key)
    {
        Debug.Log($"Downloading dependencies for {key}");

        var handle = Addressables.DownloadDependenciesAsync(key);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log($"Successfully downloaded dependencies for {key}");
        }
        else
        {
            Debug.LogError($"Failed to download dependencies for {key}: {handle.OperationException}");
        }
    }
}
