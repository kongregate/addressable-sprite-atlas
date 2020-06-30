using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.XR;

public class LoadSpriteTest : MonoBehaviour
{
    IEnumerator Start()
    {
        var handle = Addressables.LoadAssetAsync<Sprite>("happy_non_atlas");
        yield return handle;

        if (AsyncOperationStatus.Succeeded == handle.Status)
        {
            Debug.Log($"Successfully loaded sprite: {handle.Result}");
        }
        else
        {
            Debug.LogError($"Failed to load sprite: {handle.OperationException}");
        }
    }
}
