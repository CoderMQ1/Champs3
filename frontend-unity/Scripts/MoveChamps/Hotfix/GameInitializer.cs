// 
// 2023/11/30

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using QFramework;
using SquareHero.Core;
using SquareHero.Hotfix.AssetsBundle;
using SquareHero.Hotfix.Generate;
using SquareHero.Hotfix.Main;
using SquareHero.Hotfix.Model;
using SquareHero.Hotfix.Net;
using SquareHero.Hotfix.Toast;
using SquareHero.Hotfix.UI;
using UnityCommon;
using UnityCommon.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YooAsset;

namespace SquareHero.Hotfix
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

            RunxNetManager.RegisterHandler<G2C_Login>(KeepOpCode.Op_G2C_Login, LoginHelper.OnLogin);

            UIKit.Config.PanelLoaderPool = new YooAssetPanelLoaderPool();
            AudioKit.Config.AudioLoaderPool = new YooassetAudioLoaderPool();

            Web3.Instance.TransactionErrorHandler += () =>
            {
                UIKit.HidePanel<WaitingPanel>();
                UIKit.EnableUIInput();
            };

            // RunxNetManager.Instance.OnClientDisconnected += () =>
            // {
            //     LoginHelper.OnDisconnectServer();
            // };


            
            ResourceManager.Instance.Initialize(() =>
            {
                // Global.VideoPlayer.url = "https://resource.squarehero.io/HotfixAsset/WebGL/Video/Main.mp4";
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
            RunxNetManager.UnRegisterHandler<G2C_Login>(KeepOpCode.Op_G2C_Login, LoginHelper.OnLogin);
        }

        #endregion


    }

}