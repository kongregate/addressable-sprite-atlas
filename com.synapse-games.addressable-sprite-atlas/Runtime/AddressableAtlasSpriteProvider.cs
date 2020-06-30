using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.U2D;

namespace Synapse.AddressableAtlasSprite
{
    public class AddressableAtlasSpriteProvider : ResourceProviderBase
    {
        public override void Provide(ProvideHandle providerInterface)
        {
            var atlas = providerInterface.GetDependency<SpriteAtlas>(0);
            if (atlas == null)
            {
                providerInterface.Complete<Sprite>(
                    null,
                    false,
                    new System.Exception(
                        $"Sprite atlas failed to load for location {providerInterface.Location.PrimaryKey}."));
                return;
            }

            var spriteKey = providerInterface.ResourceManager.TransformInternalId(providerInterface.Location);
            var sprite = atlas.GetSprite(spriteKey);
            providerInterface.Complete(sprite, sprite != null, sprite != null ? null : new System.Exception($"Sprite failed to load for location {providerInterface.Location.PrimaryKey}."));
        }
    }
}
