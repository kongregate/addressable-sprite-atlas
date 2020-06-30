using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.Initialization;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.AddressableAssets.ResourceProviders;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.ResourceManagement.Util;

namespace Synapse.AddressableAtlasSprite
{
    // NOTE: Most of this functionality is copied from `BuildScriptFastMode` from Addressables
    // 1.10.0. The updated version of `BuildScriptFastMode` introduced in Addressables 1.11.2 is
    // much faster but doesn't generate an asset catalog, which means the custom logic we added
    // never has a chance to add additional entries for atlas sprites.
    //
    // TODO: Investigate the updated approach used in Addressables 1.11.2 and see if we can get
    // our custom build logic working using the faster build script.

    [CreateAssetMenu(
        fileName = "BuildScriptFastModeWithAtlasSpriteAddresses.asset",
        menuName = "Addressables/Addressable Atlas Sprite Build/Fast Mode")]
    public class BuildScriptFastModeWithAtlasSpriteAddresses : BuildScriptBase
    {
        // HACK: Constants copied from Addressables internals. Needed by some of the
        // copied build code, needs more investigation to determine if these constants
        // are "safe" (i.e. are unlikely to change out from under us) or if we should
        // further refactor.
        private const string EditorSceneListPath = "Scenes In Build";
        private const string ResourcesPath = "*/Resources/";

        public override string Name => "Use Asset Database (faster, with atlas sprite addresses)";

        public override bool CanBuildData<T>()
        {
            return typeof(T).IsAssignableFrom(typeof(AddressablesPlayModeBuildResult));
        }

        public override void ClearCachedData()
        {
            DeleteFile(string.Format(PathFormat, "", "catalog"));
            DeleteFile(string.Format(PathFormat, "", "settings"));
        }

        public override bool IsDataBuilt()
        {
            var catalogPath = string.Format(PathFormat, "", "catalog");
            var settingsPath = string.Format(PathFormat, "", "settings");
            return File.Exists(catalogPath) &&
                   File.Exists(settingsPath);
        }

        private string PathFormat = "{0}Library/com.unity.addressables/{1}_BuildScriptFastMode.json";

        protected override TResult BuildDataImplementation<TResult>(AddressablesDataBuilderInput context)
        {
            TResult result = default(TResult);

            var aaSettings = context.AddressableSettings;

            //create runtime data
            var aaContext = new AddressableAssetsBuildContext
            {
                Settings = aaSettings,
                runtimeData = new ResourceManagerRuntimeData(),
                bundleToAssetGroup = null,
                locations = new List<ContentCatalogDataEntry>(),
                providerTypes = new HashSet<Type>()
            };
            aaContext.runtimeData.BuildTarget = context.Target.ToString();
            aaContext.runtimeData.LogResourceManagerExceptions = aaSettings.buildSettings.LogResourceManagerExceptions;
            aaContext.runtimeData.CatalogLocations.Add(new ResourceLocationData(
                new[] { ResourceManagerRuntimeData.kCatalogAddress },
                string.Format(PathFormat, "file://{UnityEngine.Application.dataPath}/../", "catalog"),
                typeof(ContentCatalogProvider),
                typeof(ContentCatalogData)));

            var errorString = ProcessAllGroups(aaContext);
            if (!string.IsNullOrEmpty(errorString))
            {
                result = AddressableAssetBuildResult.CreateResult<TResult>(null, 0, errorString);
            }

            if (result == null)
            {
                foreach (var io in aaSettings.InitializationObjects)
                {
                    if (io is IObjectInitializationDataProvider)
                    {
                        var initData = (io as IObjectInitializationDataProvider)
                            .CreateObjectInitializationData();

                        aaContext
                            .runtimeData
                            .InitializationObjects
                            .Add(initData);
                    }
                }

                var settingsPath = string.Format(PathFormat, "", "settings");
                WriteFile(settingsPath, JsonUtility.ToJson(aaContext.runtimeData), context.Registry);

                //save catalog
                var catalogData = new ContentCatalogData(
                    aaContext.locations,
                    ResourceManagerRuntimeData.kCatalogAddress);

                foreach (var t in aaContext.providerTypes)
                {
                    var provider = ObjectInitializationData.CreateSerializedInitializationData(t);
                    catalogData.ResourceProviderData.Add(provider);
                }

                catalogData.ResourceProviderData.Add(ObjectInitializationData.CreateSerializedInitializationData<AssetDatabaseProvider>());
                catalogData.InstanceProviderData = ObjectInitializationData.CreateSerializedInitializationData(instanceProviderType.Value);
                catalogData.SceneProviderData = ObjectInitializationData.CreateSerializedInitializationData(sceneProviderType.Value);
                WriteFile(string.Format(PathFormat, "", "catalog"), JsonUtility.ToJson(catalogData), context.Registry);

                //inform runtime of the init data path
                var runtimeSettingsPath = string.Format(PathFormat, "file://{UnityEngine.Application.dataPath}/../", "settings");
                PlayerPrefs.SetString(Addressables.kAddressablesRuntimeDataPath, runtimeSettingsPath);
                result = AddressableAssetBuildResult.CreateResult<TResult>(settingsPath, aaContext.locations.Count);
            }

            return result;
        }

        protected override string ProcessGroup(AddressableAssetGroup assetGroup, AddressableAssetsBuildContext aaContext)
        {
            foreach (var entry in assetGroup.entries)
            {
                if (entry == null || string.IsNullOrEmpty(entry.AssetPath))
                {
                    continue;
                }

                // If the entry is a folder, process each of the assets in the folder. Otherwise,
                // process the entry as a single asset.
                if (entry.AssetPath == ResourcesPath
                    || entry.AssetPath == EditorSceneListPath
                    || File.GetAttributes(entry.AssetPath).HasFlag(FileAttributes.Directory))
                {
                    var allEntries = new List<AddressableAssetEntry>();
                    entry.GatherAllAssets(allEntries, false, true, false);
                    foreach (var folderEntry in allEntries)
                    {
                        // Create default catalog entries.
                        folderEntry.CreateCatalogEntries(
                            aaContext.locations,
                            false,
                            typeof(AssetDatabaseProvider).FullName,
                            null,
                            null,
                            aaContext.providerTypes);

                        // Create additional catalog entries if the asset is a `SpriteAtlas`.
                        SpriteAtlasBuildUtils.ProcessEntry(folderEntry, aaContext);
                    }
                }
                else
                {
                    // Create default catalog entries.
                    entry.CreateCatalogEntries(
                        aaContext.locations,
                        false,
                        typeof(AssetDatabaseProvider).FullName,
                        null,
                        null,
                        aaContext.providerTypes);

                    // Create additional catalog entries if the asset is a `SpriteAtlas`.
                    SpriteAtlasBuildUtils.ProcessEntry(entry, aaContext);
                }
            }

            return string.Empty;
        }
    }
}
