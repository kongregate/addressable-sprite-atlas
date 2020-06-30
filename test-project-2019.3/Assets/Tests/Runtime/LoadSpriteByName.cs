using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.TestTools;

namespace Tests
{
    public class LoadSpriteByName
    {
        [UnityTest]
        public IEnumerator LoadNonAtlasSprite()
        {
            var handle = Addressables.LoadAssetAsync<Sprite>("happy_non_atlas");
            yield return handle;
            Assert.AreEqual(AsyncOperationStatus.Succeeded, handle.Status);
        }

        [UnityTest]
        public IEnumerator LoadSpriteByFullAddress()
        {
            var handle = Addressables.LoadAssetAsync<Sprite>("Test Atlas[happy]");
            yield return handle;
            Assert.AreEqual(AsyncOperationStatus.Succeeded, handle.Status);
        }

        [UnityTest]
        public IEnumerator LoadSpriteByNameWithEnumeratorPasses()
        {
            var handle = Addressables.LoadAssetAsync<Sprite>("happy");
            yield return handle;
            Assert.AreEqual(AsyncOperationStatus.Succeeded, handle.Status);
        }
    }
}
