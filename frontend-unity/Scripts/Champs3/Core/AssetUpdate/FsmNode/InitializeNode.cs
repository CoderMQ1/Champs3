// 
// 2023/12/19

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using QFramework.Example;
using UnityCommon;
using UnityEngine;
using UnityEngine.Networking;
using YooAsset;

namespace champs3.Core
{
    public class InitializeNode : AbstractState<YooassetUpdateStates, AssetUpdater>
    {
        private Sequence _checkVersionSequence;
        public InitializeNode(FSM<YooassetUpdateStates> fsm, AssetUpdater target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            // TypeEventSystem.Global.Send();
            mTarget.StartCoroutine(InitPackage());
        }

        private void CheckVerionAnimation()
        {
            var sequence = DOTween.Sequence();
            _checkVersionSequence = sequence;
            mTarget.Tip.text = "CheckVersion.";

            sequence.AppendInterval(0.4f);
            sequence.AppendCallback(() =>
            {
                mTarget.Tip.text = "CheckVersion..";
            });
            sequence.AppendInterval(0.4f);
            sequence.AppendCallback(() =>
            {   
                mTarget.Tip.text = "CheckVersion...";
            });
            sequence.AppendInterval(0.4f);
            sequence.AppendCallback(() =>
            {
                mTarget.Tip.text = "CheckVersion.";
            });
            sequence.SetLoops(-1);

            sequence.Play();
        }
        

        private IEnumerator InitPackage()
        {

            CheckVerionAnimation();
//             string baseUrl = "http://106.75.176.67/";
//             
// #if UNITY_WEBGL && !UNITY_EDITOR
//             baseUrl = "https://info.champs3.io/";
// #endif
//             string url =
//                 $"{baseUrl}api/yooasset_version?client_name=champs3&pre_test=0&platform={GetPlatformTypeName()}";
//             var unityWebRequest = UnityWebRequest.Get(url
//                 );
//             
//             unityWebRequest.SetRequestHeader("x-app-id", "WADuzvdLJPM5g24eoS8qre34nU2biapDgCQgAwIOWHtCe");
//             unityWebRequest.SetRequestHeader("x-app-key","gGpixYKzIk2JXcBPsrWzan7oCtFWKDfchuFcC3aryPdzg");
//             var downloadHandler = unityWebRequest.downloadHandler;
//             yield return unityWebRequest.SendWebRequest();
//             
//             if (downloadHandler.isDone)
//             {
//                 var result = downloadHandler.text;
//                 
//                 LogKit.I($"Version Info : {result}");
//             
//                 SHServerConfig.Initialize(result);
//             }
            
            yield return new WaitForSeconds(1);
            _checkVersionSequence.Kill();
            mTarget.Progress.gameObject.SetActive(true);
            var playMode = mTarget.EPlayMode;

            // 创建默认的资源包
            string packageName = "MainPackage";
            var package = YooAssets.GetPackage(packageName);
            if (package == null)
            {
                package = YooAssets.CreatePackage(packageName);
                YooAssets.SetDefaultPackage(package);
            }

            // 编辑器下的模拟模式
            InitializationOperation initializationOperation = null;
            if (playMode == EPlayMode.EditorSimulateMode)
            {
                var createParameters = new EditorSimulateModeParameters();
                createParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild(packageName);
                initializationOperation = package.InitializeAsync(createParameters);
                yield return initializationOperation;
                mFSM.ChangeState(YooassetUpdateStates.Done);
                yield break;
            }

            // 单机运行模式
            if (playMode == EPlayMode.OfflinePlayMode)
            {
                var createParameters = new OfflinePlayModeParameters();
                initializationOperation = package.InitializeAsync(createParameters);

            }

            // 联机运行模式
            if (playMode == EPlayMode.HostPlayMode)
            {
                string defaultHostServer = GetHostServerURL();
                string fallbackHostServer = GetHostServerURL();
                var createParameters = new HostPlayModeParameters();
                // createParameters.DecryptionServices = new GameDecryptionServices();
                createParameters.BuildinQueryServices = new GameQueryServices();
                createParameters.DeliveryQueryServices = new DefaultDeliveryQueryServices();
                createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                initializationOperation = package.InitializeAsync(createParameters);
            }

            // WebGL运行模式
            if (playMode == EPlayMode.WebPlayMode)
            {
                string defaultHostServer = GetHostServerURL();
                string fallbackHostServer = GetHostServerURL();
                var createParameters = new WebPlayModeParameters();
                // createParameters.DecryptionServices = new GameDecryptionServices();
                createParameters.BuildinQueryServices = new GameQueryServices();
                createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                initializationOperation = package.InitializeAsync(createParameters);
            }

            yield return initializationOperation;
            if (initializationOperation.Status == EOperationStatus.Succeed)
            {
                // if (DebugProfile.Instance.Debug)
                // {
                    mFSM.ChangeState(YooassetUpdateStates.Done);
                // }
                // else
                // {
                //     mFSM.ChangeState(YooassetUpdateStates.UpdateVersion);
                // }
            }
            else
            {
                Debug.LogWarning($"{initializationOperation.Error}");
                //PatchEventDefine.InitializeFailed.SendEventMessage();
            }
        }
        
        /// <summary>
        /// 获取资源服务器地址
        /// </summary>
        private string GetHostServerURL()
        {
            //string hostServerIP = "http://10.0.2.2"; //安卓模拟器地址
            string hostServerIP = "http://127.0.0.1";
            string appVersion = "v1.0";
            return SHServerConfig.Instance.data.resource_url;
            // return "https://resource.champs3.io/HotfixAsset/WebGL/2023-12-29-1336";
// #if UNITY_EDITOR
//             if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
//                 return $"{hostServerIP}/CDN/Android/{appVersion}";
//             else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
//                 return $"{hostServerIP}/CDN/IPhone/{appVersion}";
//             else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.WebGL)
//                 return $"{hostServerIP}/CDN/WebGL/{appVersion}";
//             else
//                 return $"{hostServerIP}/CDN/PC/{appVersion}";
// #else
// 		if (Application.platform == RuntimePlatform.Android)
// 			return $"{hostServerIP}/CDN/Android/{appVersion}";
// 		else if (Application.platform == RuntimePlatform.IPhonePlayer)
// 			return $"{hostServerIP}/CDN/IPhone/{appVersion}";
// 		else if (Application.platform == RuntimePlatform.WebGLPlayer)
// 			return $"{hostServerIP}/CDN/WebGL/{appVersion}";
// 		else
// 			return $"{hostServerIP}/CDN/PC/{appVersion}";
// #endif
        }
        

        
        /// <summary>
        /// 操作系统平台类型
        /// </summary>
        public static string GetPlatformTypeName()
        {
            string value = "pc";
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsPlayer:
                    value = GamePlatformType.PC.ToString();
                    break;
                case RuntimePlatform.Android:
                    value = GamePlatformType.ANDROID.ToString();
                    break;
                case RuntimePlatform.IPhonePlayer:
                    value = GamePlatformType.IOS.ToString();
                    break;
                case RuntimePlatform.OSXPlayer:
                    value = GamePlatformType.MAC.ToString();
                    break;
                case RuntimePlatform.WebGLPlayer:
                    value = GamePlatformType.WEB.ToString();
                    break;
            }

            return value.ToLower();
        }

        public enum GamePlatformType
        {
            PC,
            IOS,
            ANDROID,
            MAC,
            WEB,
        }
    }
    
    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
    class RemoteServices : IRemoteServices
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
    
    /// <summary>
    /// 默认的分发资源查询服务类
    /// </summary>
     class DefaultDeliveryQueryServices : IDeliveryQueryServices
    {
        public DeliveryFileInfo GetDeliveryFileInfo(string packageName, string fileName)
        {
            throw new NotImplementedException();
        }
        public bool QueryDeliveryFiles(string packageName, string fileName)
        {
            return false;
        }
    }
}