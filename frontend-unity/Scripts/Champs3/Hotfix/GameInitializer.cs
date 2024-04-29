// 
// 2023/11/30

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using QFramework;
using champs3.Core;
using champs3.Hotfix.AssetsBundle;
using champs3.Hotfix.Generate;
using champs3.Hotfix.Main;
using champs3.Hotfix.Model;
using champs3.Hotfix.Net;
using champs3.Hotfix.Toast;
using champs3.Hotfix.UI;
using UnityCommon;
using UnityCommon.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YooAsset;

namespace champs3.Hotfix
{
    public class GameInitializer : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        public Global Global;

        #endregion

        #region FIELDS

        private GameStartPanel _gameStartPanel;
        #endregion

        #region PROPERTIES

        #endregion

        #region METHODS

        #region EVENT FUNCTION

        private void Awake()
        {
            // if (GameStart.Instance.Platform == RuntimePlatform.Android || GameStart.Instance.Platform == RuntimePlatform.IPhonePlayer)
            // {
            //     UIRoot.Instance.SetResolution(1080, 1920, 1f);
            // }
            // else
            // {
            //     UIRoot.Instance.SetResolution(1920, 1080, 1f);
            // }
            string gameStartPanelAddress = "GameStartPanel";

            if (GameStart.Instance.IsMobilePlatform())
            {
                gameStartPanelAddress = "MobileGameStartPanel";
                
                UIRoot.Instance.SetResolution(1080, 1920, 1f);
            }
            else
            {
                UIRoot.Instance.SetResolution(1920, 1080, 1f);
            }

            _gameStartPanel = Global.UIRoot.Common.Find(gameStartPanelAddress).GetComponent<GameStartPanel>();
            _gameStartPanel.gameObject.SetActive(true);
        }

        private void Start()
        {
            LoginHelper.Initialize();
            Initialize();
        }

        #endregion


        public void Initialize()
        {

            champs3NetManager.RegisterHandler<G2C_Login>(KeepOpCode.Op_G2C_Login, LoginHelper.OnLogin);

            UIKit.Config.PanelLoaderPool = new YooAssetPanelLoaderPool();
            AudioKit.Config.AudioLoaderPool = new YooassetAudioLoaderPool();

            Web3.Instance.TransactionErrorHandler += () =>
            {
                UIKit.HidePanel<WaitingPanel>();
                UIKit.EnableUIInput();
            };

            // champs3NetManager.Instance.OnClientDisconnected += () =>
            // {
            //     LoginHelper.OnDisconnectServer();
            // };


            
            ResourceManager.Instance.Initialize(() =>
            {
                // Global.VideoPlayer.url = "https://resource.champs3.io/HotfixAsset/WebGL/Video/Main.mp4";
                // Global.VideoPlayer.Play();
                _gameStartPanel.StartGame.onClick.AddListener(() =>
                {
                    AudioKit.PlaySound(SoundName.clickbutton.ToString());
                    
#if !UNITY_EDITOR && UNITY_WEBGL
                    UIKit.OpenPanelAsync<WaitingPanel>(panel =>
                    {
                        LoginHelper.ConnectPreta();
                    });
#else
                        // UIKit.OpenPanelAsync<MetaMaskPanel>();
                        // LoginHelper.Login();
                        
                    LoginHelper.OnPetraSign("Test");
#endif
                });
            });
        }

        

        private void OnDestroy()
        {
            champs3NetManager.UnRegisterHandler<G2C_Login>(KeepOpCode.Op_G2C_Login, LoginHelper.OnLogin);
        }

        #endregion


    }

}