// 
// 2023/11/30

using System;
using QFramework;
using UnityEngine;
using YooAsset;
using Object = UnityEngine.Object;

namespace champs3.Hotfix.AssetsBundle
{
    public class YooAssetResLoaderPool : AbstractResLoaderPool
    {
        protected override IResLoader CreateResLoader(string location)
        {
            return new YooAssetResLoader(location);
        }
    }

    public class YooAssetResLoader : IResLoader
    {
        public YooAssetResLoader(string location)
        {
            this.location = location;
        }
        
        private string location;
        private AssetOperationHandle _assetOperationHandle;
        private int _refCount;

        public void Release()
        {
            _refCount--;
            if (IsUnused())
            {
                ReleaseInternal();
            }
        }

        public void ReleaseInternal()
        {
            if (_assetOperationHandle != null)
            {
                _assetOperationHandle.Release();
                _assetOperationHandle = null;
                var resourcePackage = YooAssets.GetPackage(ResourceManager.MainPackage);
                resourcePackage.UnloadUnusedAssets();
            }
        }

        public float Progress()
        {
            return _assetOperationHandle == null ? 0 : _assetOperationHandle.Progress;
        }

        public bool IsUnused()
        {
            return _refCount <= 0;
        }

        public bool IsDone()
        {
            return _assetOperationHandle == null ? false : _assetOperationHandle.IsDone;
        }

        public void ResetLocation(string location)
        {
            // Debug.Log($"ResetLocation {this.location} : {location}");
            this.location = location;
            _assetOperationHandle = null;
        }

        public T GetAsset<T>() where T : Object
        {
            if (_assetOperationHandle == null)
            {
                _assetOperationHandle = YooAssets.LoadAssetSync<T>(location);
            }

            var assetObject = _assetOperationHandle.GetAssetObject<T>();
            _refCount ++;
            return assetObject;
        }

        public void GetAssetAsync<T>(Action<T> onResLoad) where T : Object
        {
            if (_assetOperationHandle == null)
            {
                _assetOperationHandle = YooAssets.LoadAssetAsync<T>(location);
                _assetOperationHandle.Completed += handle =>
                {
                    if (onResLoad != null)
                    {
                        onResLoad.Invoke(handle.GetAssetObject<T>());
                        _refCount++;
                    }
                    
                };
            }
            else
            {
                if (_assetOperationHandle.IsDone)
                {
                    if (onResLoad != null)
                    {
                        onResLoad.Invoke(_assetOperationHandle.GetAssetObject<T>());
                        _refCount++;
                    }
                }
                else
                {
                    _assetOperationHandle.Completed += handle =>
                    {
                        if (onResLoad != null)
                        {
                            onResLoad.Invoke(handle.GetAssetObject<T>());
                            _refCount++;
                        }
                    
                    };
                }
            }

        }

        public void Unload()
        {
            Release();
            
        }

    }
}