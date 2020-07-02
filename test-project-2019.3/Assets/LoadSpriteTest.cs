using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadSpriteTest : MonoBehaviour
{
    public List<Sprite> _loadedSprites = new List<Sprite>();

    IEnumerator Start()
    {
        yield return TryLoadSprite("happy");
        yield return TryLoadTexture("happy");
        yield return TryLoadSprite("sad_non_atlas");
        yield return TryLoadTexture("sad_non_atlas");
    }

    public static IEnumerator TryLoadSprite(string address)
    {
        Debug.Log($"Attempting to load Sprite @ {address}");

        var handle = Addressables.LoadAssetAsync<Sprite>(address);
        yield return handle;

        if (AsyncOperationStatus.Succeeded == handle.Status)
        {
            Debug.Log($"Successfully loaded Sprite @ {address}: {handle.Result}");
        }
        else
        {
            Debug.LogError($"Failed to load Sprite @ {address}: {handle.OperationException}");
        }
    }

    public static IEnumerator TryLoadTexture(string address)
    {
        Debug.Log($"Attempting to load Texture2D @ {address}");

        var handle = Addressables.LoadAssetAsync<Texture2D>(address);
        yield return handle;

        if (AsyncOperationStatus.Succeeded == handle.Status)
        {
            Debug.Log($"Successfully loaded Texture2D @ {address}: {handle.Result}");
        }
        else
        {
            Debug.LogError($"Failed to load Texture2D @ {address}: {handle.OperationException}");
        }
    }
}
