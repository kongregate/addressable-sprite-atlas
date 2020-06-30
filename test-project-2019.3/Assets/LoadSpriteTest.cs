using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.XR;

public class LoadSpriteTest : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return TryLoadSprite("happy_non_atlas");
        yield return TryLoadSprite("sad_non_atlas");
    }

    private IEnumerator TryLoadSprite(string address)
    {
        Debug.Log($"Attempting to load sprite @ {address}");

        var handle = Addressables.LoadAssetAsync<Sprite>(address);
        yield return handle;

        if (AsyncOperationStatus.Succeeded == handle.Status)
        {
            Debug.Log($"Successfully loaded sprite @ {address}: {handle.Result}");
        }
        else
        {
            Debug.LogError($"Failed to load sprite @ {address}: {handle.OperationException}");
        }
    }
}
