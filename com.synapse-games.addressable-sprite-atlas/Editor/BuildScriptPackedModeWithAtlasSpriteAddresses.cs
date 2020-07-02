using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Synapse.AddressableAtlasSprite
{
    [CreateAssetMenu(
        fileName = "BuildScriptPackedModeWithAtlasSpriteAddresses.asset",
        menuName = "Addressables/Addressable Atlas Sprite Build/Packed Mode")]
    public class BuildScriptPackedModeWithAtlasSpriteAddresses : BuildScriptPackedMode
    {
        public override string Name => "Packed with Atlas Sprite Addresses";

        protected override string ProcessGroup(AddressableAssetGroup assetGroup, AddressableAssetsBuildContext context)
        {
            foreach (var entry in assetGroup.entries)
            {
                SpriteAtlasBuildUtils.ProcessEntry(entry, context);
            }

            return base.ProcessGroup(assetGroup, context);
        }
    }
}
