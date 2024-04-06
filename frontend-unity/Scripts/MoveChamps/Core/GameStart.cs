#if UNITY_EDITOR
#define MetamaskPlugin
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using UnityCommon;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using YooAsset;


namespace SquareHero.Core
{
    public class GameStart : MonoSingleton<GameStart>
    {
        #region EDITOR EXPOSED FIELDS
        
        [FormerlySerializedAs("SplashScene")] [Scene] public string splashScene;
        [FormerlySerializedAs("VersionCheckScene")] [Scene] public string versionCheckScene;

        #endregion

        #region FIELDS
        
        private GameStartState _state;
        private bool checkedVersion;
        public RuntimePlatform Platform = RuntimePlatform.Android;
        #endregion

        #region PROPERTIES

        public bool IsNeedChcekVersion
        {
            get { return false; }
        }

        #endregion

        #region METHODS

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            Initialize();
            // PlaySplash();
        }

        // Update is called once per frameA
        void Update()
        {
        }


        void Initialize()
        {
            Application.targetFrameRate = 60;
            YooAssets.Initialize(new YooAssetLogImpl());
            DOTween.Init();
            StartCoroutine(CheckVersion());
        }
        
        public void SetPlatform(string platfrom)
        {
            RuntimePlatform platform = RuntimePlatform.WebGLPlayer;
            if (!Enum.TryParse(platfrom, out platform))
            {
                platform = RuntimePlatform.WebGLPlayer;
            }
            Platform = platform;
        }
        
        IEnumerator CheckVersion()
        {
            // var unityWebRequest = UnityWebRequest.Get(
            //     $"http://106.75.176.67/api/yooasset_version?client_name=squarehero&pre_test=0&platform=web");
            //
            // unityWebRequest.SetRequestHeader("x-app-id", "WADuzvdLJPM5g24eoS8qre34nU2biapDgCQgAwIOWHtCe");
            // unityWebRequest.SetRequestHeader("x-app-key", "gGpixYKzIk2JXcBPsrWzan7oCtFWKDfchuFcC3aryPdzg");
            // var downloadHandler = unityWebRequest.downloadHandler;
            // yield return unityWebRequest.SendWebRequest();
            //
            // if (downloadHandler.isDone)
            // {
            //     var result = downloadHandler.text;
            //
            //     LogKit.I($"Version Info : {result}");
            //
            //     SHServerConfig.Initialize(result);
            // }

            // if (_state == GameStartState.WaitCheckVersion)
            // {
            //     CheckAssetVersion();
            // }
            // else
            // {
            //     SwitchState(GameStartState.WaitSplash);
            // }
            DebugProfile.Instance.Initialized();
            CheckAssetVersion();
            
            yield break;
        }



        public void PlaySplash()
        {
            SwitchState(GameStartState.Splashing);
            SceneManager.LoadSceneAsync(splashScene);
        }

        public void CompletSplash()
        {
            if (_state == GameStartState.WaitSplash)
            {
                CheckAssetVersion();
            }
            else
            {
                SwitchState(GameStartState.WaitCheckVersion);
            }
        }

        public bool IsMobilePlatform()
        {
            return Platform == RuntimePlatform.Android || Platform == RuntimePlatform.IPhonePlayer;
        }


        public void CheckAssetVersion()
        {
            SceneManager.LoadSceneAsync(versionCheckScene);
        }

        private void SwitchState(GameStartState state)
        {
            if (_state == state) return;
            _state = state;
        }

        #endregion
    }

    public enum GameStartState
    {
        Splashing,
        WaitCheckVersion,
        WaitSplash,
    }
}