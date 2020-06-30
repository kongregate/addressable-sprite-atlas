using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Synapse.AddressableAtlasSprite
{
    [CreateAssetMenu(
        fileName = "BuildScriptFastModeWithAtlasSpriteAddresses.asset",
        menuName = "Addressables/Addressable Atlas Sprite Build/Fast Mode")]
    public class BuildScriptFastModeWithAtlasSpriteAddresses : BuildScriptFastMode
    {
        public override string Name => "Use Asset Database (faster, with atlas sprite addresses)";

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
