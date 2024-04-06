// 
// 2023/11/30

using System;
using QFramework;
using SquareHero.Core;
using UnityEngine;
using YooAsset;

namespace SquareHero.Hotfix.AssetsBundle
{
    public class YooAssetPanelLoaderPool : AbstractPanelLoaderPool
    {
        public const string GroupNameUIPrefabs = "UIPrefabs";
        public const string GroupNameMobileUIPrefabs = "MobileUIPrefabs";
        protected override IPanelLoader CreatePanelLoader()
        {
            
            return new YooAssetPanelLoader();
        }

        public class YooAssetPanelLoader : IPanelLoader
        {
            private AssetOperationHandle _assetOperationHandle;
            public YooAssetPanelLoader()
            {
            }

            public GameObject LoadPanelPrefab(PanelSearchKeys panelSearchKeys)
            {
                if (_assetOperationHandle != null)
                {
                    LogKit.E($"PanelLoader need release ,already loaded asset {_assetOperationHandle.GetAssetInfo().AssetPath}, on load {GetLocation(panelSearchKeys)} ");
                }
                _assetOperationHandle = YooAssets.LoadAssetSync<GameObject>(GetLocation(panelSearchKeys));
                return _assetOperationHandle.GetAssetObject<GameObject>();
            }

            public void LoadPanelPrefabAsync(PanelSearchKeys panelSearchKeys, Action<GameObject> onPanelPrefabLoad)
            {
                if (_assetOperationHandle != null)
                {
                    LogKit.E($"PanelLoader need release ,already loaded asset {_assetOperationHandle.GetAssetInfo().AssetPath}, on load {GetLocation(panelSearchKeys)} ");
                }
                _assetOperationHandle = YooAssets.LoadAssetAsync<GameObject>(GetLocation(panelSearchKeys));
                _assetOperationHandle.Completed += handle =>
                {
                    onPanelPrefabLoad.Invoke(handle.GetAssetObject<GameObject>());
                };
            }

            public void Release()
            {
                if (_assetOperationHandle != null)
                {
                    _assetOperationHandle.Release();
                    _assetOperationHandle = null;
                }
            }

            public string GetLocation(PanelSearchKeys panelSearchKeys)
            {
                
                if (!string.IsNullOrEmpty(panelSearchKeys.GameObjName))
                {
                    return $"{GroupNameUIPrefabs}_{panelSearchKeys.GameObjName}";
                }

                if (!string.IsNullOrEmpty(panelSearchKeys.AssetBundleName))
                {
                    return panelSearchKeys.AssetBundleName;
                }

                var panelType = panelSearchKeys.PanelType;
                LogKit.I($"Get Location {panelType.Name}");
                if (GameStart.Instance.Platform == RuntimePlatform.Android || GameStart.Instance.Platform == RuntimePlatform.IPhonePlayer)
                {
                    return $"{GroupNameMobileUIPrefabs}_{panelType.Name}";
                }
                return $"{GroupNameUIPrefabs}_{panelType.Name}";
            }

            public void Unload()
            {
                Release();
                var resourcePackage = YooAssets.GetPackage("MainPackage");
                resourcePackage.UnloadUnusedAssets();
            }
        }
    }
}