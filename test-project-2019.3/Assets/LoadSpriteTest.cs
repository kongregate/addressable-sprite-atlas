using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;
using UnityEngine.UI;

public class LoadSpriteTest : MonoBehaviour
{
    public string[] SpriteAddresses;
    public AssetReferenceSprite[] SpriteReferences;
    public LayoutGroup DisplayGroup;

    IEnumerator Start()
    {
        SpriteAtlasManager.atlasRegistered += atlas =>
        {
            Debug.Log($"Registered atlas: {atlas}");
        };

        SpriteAtlasManager.atlasRequested += (name, callback) =>
        {
            Debug.Log($"Atlas requested: {name}");
        };

        // NOTE: Explicitly initialize Addressables before trying to load anything so
        // that it's easier to debug the loading sequence. Otherwise the first attempt
        // to load a sprite will get chained onto the initialization process, which
        // makes it more complicated to debug.
        yield return Addressables.InitializeAsync();

        foreach (var address in SpriteAddresses)
        {
            yield return TryLoadSprite(address);
        }

        foreach (var spriteReference in SpriteReferences)
        {
            var handle = spriteReference.LoadAssetAsync();
            yield return handle;

            if (AsyncOperationStatus.Succeeded == handle.Status)
            {
                Debug.Log($"Successfully loaded sprite reference {spriteReference}: {handle.Result}");
            }
            else
            {
                Debug.LogError($"Failed to load sprite reference {spriteReference}: {handle.OperationException}");
            }
        }
    }

    public IEnumerator TryLoadSprite(string address)
    {
        Debug.Log($"Attempting to load Sprite @ {address}");

        var handle = Addressables.LoadAssetAsync<Sprite>(address);
        yield return handle;

        if (AsyncOperationStatus.Succeeded == handle.Status)
        {
            Debug.Log($"Successfully loaded Sprite @ {address}: {handle.Result}");

            var displayObject = new GameObject(address, typeof(Image));
            var image = displayObject.GetComponent<Image>();
            image.sprite = handle.Result;

            displayObject.transform.SetParent(DisplayGroup.transform, false);
        }
        else
        {
            Debug.LogError($"Failed to load Sprite @ {address}: {handle.OperationException}");
        }
    }
}
