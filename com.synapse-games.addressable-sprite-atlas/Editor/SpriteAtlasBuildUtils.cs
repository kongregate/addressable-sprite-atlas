using UnityEditor;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.U2D;

namespace Synapse.AddressableAtlasSprite
{
    public static class SpriteAtlasBuildUtils
    {
        public static void ProcessEntry(AddressableAssetEntry entry, AddressableAssetsBuildContext context)
        {
            var type = AssetDatabase.GetMainAssetTypeAtPath(entry.AssetPath);

            // Add special handling for SpriteAtlas assets by adding extra catalog
            // entries for each of the sprites in the atlas.
            if (type == typeof(SpriteAtlas))
            {
                var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(entry.AssetPath);

                var sprites = new Sprite[atlas.spriteCount];
                atlas.GetSprites(sprites);

                var keyList = entry.CreateKeyList();

                for (int i = 0; i < atlas.spriteCount; i++)
                {
                    var spriteName = sprites[i].name;

                    // Remove the "(Clone)" suffix that is appended to the sprite name when we
                    // call GetSprites().
                    if (spriteName.EndsWith("(Clone)"))
                    {
                        spriteName = spriteName.Replace("(Clone)", "");
                    }

                    context.locations.Add(new ContentCatalogDataEntry(
                        typeof(Sprite),
                        spriteName,
                        typeof(AddressableAtlasSpriteProvider).FullName,
                        new object[] { spriteName },
                        new object[] { keyList[0] }));
                }

                context.providerTypes.Add(typeof(AddressableAtlasSpriteProvider));
            }
        }
    }
}
