using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Synapse.AddressableAtlasSprite
{
    [CreateAssetMenu(
        fileName = "BuildScriptPackedModeWithAtlasSpriteAddresses.asset",
        menuName = "Addressables/Custom Build/Packed Mode with Atlas Sprite Addresses")]
    public class BuildScriptPackedModeWithAtlasSpriteAddresses : BuildScriptPackedMode
    {
        public override string Name => "Default Build Script (with atlas sprite addresses)";

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
