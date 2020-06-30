using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Synapse.AddressableAtlasSprite
{
    [CreateAssetMenu(
        fileName = "BuildScriptVirtualModeWithAtlasSpriteAddresses.asset",
        menuName = "Addressables/Addressable Atlas Sprite Build/Virtual Mode")]
    public class BuildScriptVirtualModeWithAtlasSpriteAddresses : BuildScriptVirtualMode
    {
        public override string Name => "Simulate Groups (advanced, with atlas sprite addresses)";

        protected override string ProcessGroup(
            AddressableAssetGroup assetGroup,
            AddressableAssetsBuildContext context)
        {
            foreach (var entry in assetGroup.entries)
            {
                SpriteAtlasBuildUtils.ProcessEntry(entry, context);
            }

            return base.ProcessGroup(assetGroup, context);
        }
    }
}
