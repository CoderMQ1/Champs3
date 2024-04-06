using System;
using QFramework;
using UnityEngine.SceneManagement;
using YooAsset;

namespace SquareHero.Hotfix.AssetsBundle
{
    public class YooAssetSceneLoaderPool : AbstractSceneLoaderPool
    {
        protected override ISceneLoader CreateResLoader()
        {
            return new YooAssetsSceneLoader();
        }
       
    }
    public class YooAssetsSceneLoader : ISceneLoader
    {
        private SceneOperationHandle _handle;
        private string _location;

        public void LoadSceneAsync(string location, Action onCompleted, LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false, int priority = 100)
        {
            _location = location;
            _handle = YooAssets.LoadSceneAsync(_location, sceneMode, suspendLoad, priority);
            _handle.Completed += handle =>
            {
                onCompleted?.Invoke();
            };
            
            
        }

        public void UnLoadSceneAsync(Action onCompleted = null)
        {
            if (_handle == null)
            {
                LogKit.E($" has not start load");
                return;
            }
            
            if (_handle.IsDone)
            {
                LogKit.E($"{_handle.SceneObject.name} has not loaded done");
                return;
            }
            if (_handle.IsMainScene())
            {
                LogKit.E($"{_handle.SceneObject.name} is not subScene");
                return;
            }

            var sceneOperation = _handle.UnloadAsync();

            sceneOperation.Completed += handle =>
            {
                onCompleted?.Invoke();
            };
        }
        
        public float Progress()
        {
            return _handle == null ? 0 : _handle.Progress;
        }

        public bool isDone()
        {
            return _handle == null ? false : _handle.IsDone;
        }

        public Scene GetResult()
        {
            return _handle.SceneObject;
        }

        public void ActiveScene()
        {
            _handle.ActivateScene();
        }
    }
}