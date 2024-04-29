// 
// 2023/11/30

using System;
using QFramework;

#if MetamaskPlugin

using champs3.Hotfix.MetaMask;
#endif
using champs3.Hotfix.UI;
using UnityEngine;
using UnityEngine.Video;

namespace champs3.Hotfix
{
    public class Global : MonoSingleton<Global>
    {
        #region EDITOR EXPOSED FIELDS

        public SceneMap SceneMap;
        public Camera MainCamera;
        public Camera UICamera;
        public UIRoot UIRoot;
        
        // TODO WebGL在加载场景后会先渲染一帧场景，这里把Loading改为单独用一个相机渲染
        public LoadingScreen LoadingScreen;


        public VideoPlayer VideoPlayer;

        public ShareToTwitter ShareToTwitter;

        #endregion

        #region FIELDS

        #endregion

        #region PROPERTIES

        #endregion

        #region METHODS

        #region EVENT FUNCTION

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            
#if MetamaskPlugin
            MetaMaskAdatper.Instance.Disconnect();
            MetaMaskAdatper.Instance.EndSession();
#endif

        }

        #endregion

        #endregion


    }

}