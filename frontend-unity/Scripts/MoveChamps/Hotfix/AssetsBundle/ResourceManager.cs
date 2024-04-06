// 
// 2023/11/30

using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using SquareHero.Hotfix.AssetsBundle;
using UnityEngine;
using UnityEngine.SceneManagement;
using YooAsset;

namespace SquareHero.Hotfix
{
    public class ResourceManager : MonoSingleton<ResourceManager>
    {
        public const string MainPackage = "MainPackage";

        #region EDITOR EXPOSED FIELDS

        public bool Initialized
        {
            get { return _initialized; }
        }
        
        #endregion

        #region FIELDS

        private static AbstractResLoaderPool _resLoaderPool;
        private static AbstractSceneLoaderPool _sceneLoaderPool;
        private static Dictionary<string, IResLoader> _assetCashes = new Dictionary<string, IResLoader>();
        private static Dictionary<string, ISceneLoader> _sceneLoaders = new Dictionary<string, ISceneLoader>();
        private static bool _initialized;

        #endregion

        #region PROPERTIES

        #endregion

        #region METHODS

        #region EVENT FUNCTION

        private void Awake()
        {
            mInstance = this;
            DontDestroyOnLoad(this);
        }

        #endregion

        public void Initialize(Action onComplete = null)
        {
            StartCoroutine(InitializeSync(onComplete));
        }


        public IEnumerator InitializeSync(Action onComplete = null)
        {
            if (!_initialized)
            {
                _resLoaderPool = new YooAssetResLoaderPool();
                _sceneLoaderPool = new YooAssetSceneLoaderPool();
                if (!YooAssets.Initialized)
                {
                    yield return YooassetsAdapter.InitializeYooAsset(MainPackage);
                }
                
                yield return new WaitForSeconds(0.5f);
                _initialized = true;
                ExcelConfig.Initialize();
            }

            onComplete?.Invoke();
        }

        private bool CanLoaded()
        {
            if (!_initialized)
            {
                throw new Exception("ResourceManager has not initialize");
            }

            return _initialized;
        }

        // public T GetAsset<T>(string location) where T : UnityEngine.Object
        // {
        //     IResLoader resLoader = null;
        //     _assetCashes.TryGetValue(location, out resLoader);
        //     if (resLoader == null)
        //     {
        //         resLoader = CreateResLoader(location);
        //         _assetCashes.Add(location, resLoader);
        //     }
        //     return resLoader.GetAsset<T>();
        // }

        public IResLoader GetAssetAsync<T>(string location, Action<T> onResLoad) where T : UnityEngine.Object
        {
            CanLoaded();
            IResLoader resLoader = null;
            _assetCashes.TryGetValue(location, out resLoader);
            if (resLoader == null)
            {
                resLoader = CreateResLoader(location);
                _assetCashes.Add(location, resLoader);
            }
            else
            {
                if (location == "Prefabs_Ripples")
                {
                    LogKit.I($"Cash load {location}");
                }
            }
            
            

            resLoader.GetAssetAsync<T>(onResLoad);
            return resLoader;
        }

        public ISceneLoader LoadSceneAsync(string location, Action onCompleted,
            LoadSceneMode sceneMode = LoadSceneMode.Single, bool suspendLoad = false, int priority = 100)
        {
            LogKit.I($"Start Load Scene {location}");
            CanLoaded();
            var sceneLoader = _sceneLoaderPool.AllocateLoader();
            
            sceneLoader.LoadSceneAsync(location, onCompleted, sceneMode, suspendLoad, priority);
            // _sceneLoaders.Add(location ,sceneLoader);
            
            return sceneLoader;
        }



        public void UnLoadSubScene(string location, Action onCompleted = null)
        {
            CanLoaded();
            if (_sceneLoaders.TryGetValue(location, out ISceneLoader sceneLoader))
            {
                sceneLoader.UnLoadSceneAsync(() =>
                {
                    _sceneLoaderPool.RecycleLoader(sceneLoader);
                    onCompleted?.Invoke();
                });
            }
            else
            {
                LogKit.E($"{location} scene is not load");
            }
        }

        public void UnLoadAsset(string location)
        {
            if (_assetCashes.TryGetValue(location, out IResLoader resLoader))
            {
                resLoader.Release();
                if (resLoader.IsUnused())
                {
                    _assetCashes.Remove(location);
                    RecycleLoader(resLoader);
                }
            }
            else
            {
                LogKit.E($"{location} is not load");
            }
        }

        public void UnloadRes(IResLoader resLoader)
        {
            resLoader.Release();
        }

        public void RecycleLoader(IResLoader resLoader)
        {
            _resLoaderPool.RecycleLoader(resLoader);
        }

        private IResLoader CreateResLoader(string location)
        {
            return _resLoaderPool.AllocateLoader(location);
        }

        #endregion
    }


    public interface IResLoader
    {
        T GetAsset<T>() where T : UnityEngine.Object;
        void GetAssetAsync<T>(Action<T> onResLoad) where T : UnityEngine.Object;
        void Release();
        float Progress();

        bool IsUnused();

        bool IsDone();

        void ResetLocation(string location);
    }

    public interface ISceneLoader
    {
        void LoadSceneAsync(string location, Action onCompleted, LoadSceneMode sceneMode = LoadSceneMode.Single,
            bool suspendLoad = false, int priority = 100);

        void UnLoadSceneAsync(Action onCompleted = null);
        float Progress();
        bool isDone();

        Scene GetResult();

        void ActiveScene();
    }

    public interface ISceneLoaderPool
    {
        ISceneLoader AllocateLoader();
        void RecycleLoader(ISceneLoader resLoader);
    }

    public abstract class AbstractSceneLoaderPool : ISceneLoaderPool
    {
        private Stack<ISceneLoader> mPool = new Stack<ISceneLoader>(16);

        public ISceneLoader AllocateLoader()
        {
            return mPool.Count > 0 ? mPool.Pop() : CreateResLoader();
        }

        protected abstract ISceneLoader CreateResLoader();


        public void RecycleLoader(ISceneLoader resLoader)
        {
            mPool.Push(resLoader);
        }
    }


    public interface IResLoaderPool
    {
        IResLoader AllocateLoader(string location);
        void RecycleLoader(IResLoader resLoader);
    }

    public abstract class AbstractResLoaderPool : IResLoaderPool
    {
        private Stack<IResLoader> mPool = new Stack<IResLoader>(16);

        public IResLoader AllocateLoader(string location)
        {
            IResLoader loader = null;
            if (mPool.Count > 0)
            {
                loader = mPool.Pop();
                loader.ResetLocation(location);
            }
            else
            {
                loader = CreateResLoader(location);
            }

            return loader;
        }

        protected abstract IResLoader CreateResLoader(string location);


        public void RecycleLoader(IResLoader resLoader)
        {
            mPool.Push(resLoader);
        }
    }

    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
    public class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }

        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{_defaultHostServer}/{fileName}";
        }

        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }
}