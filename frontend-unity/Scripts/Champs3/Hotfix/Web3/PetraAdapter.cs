using System;
using QFramework;
using UnityEngine;

namespace champs3.Hotfix
{
    public class PetraAdapter : MonoSingleton<PetraAdapter>
    {
        #region EDITOR EXPOSED FIELDS

        public OnSignEvent OnSignHander;
        public OnMintEvent OnMintHander;
        public OnMatchEvent OnMatchHander;
        

        public delegate void OnSignEvent(string data);
        public delegate void OnMintEvent(string data);
        public delegate void OnMatchEvent(string data);
        public string Account = "TestAccount";
        
        #endregion

        #region FIELDS

        #endregion

        #region PROPERTIES

        #endregion

        #region EVENT FUNCTION
        
        private void Awake()
        {
            mInstance = this;
        }

        #endregion

        #region METHODS

        public void ConnectPetra()
        {
            PetraJSLib.ConnectToPetra();
        }

        public void Mint()
        {
            PetraJSLib.Mint();
        }

        public void Match()
        {
            PetraJSLib.Match();
        }

        public void OnConnectedPetra(string account)
        {
            LogKit.I($"OnConnectedPetra {account}");
            Account = account;
        }

        public void OnSign(string data)
        {
            LogKit.I($"OnSign {data}");
            OnSignHander?.Invoke(data);
        }

        public void OnMint(string data)
        {
            LogKit.I($"OnMint {data}");
            if (!string.IsNullOrEmpty(data))
            {
                OnMintHander?.Invoke(data);
            }
            
        }
        
        public void OnMatch(string data)
        {
            LogKit.I($"OnMint {data}");
            OnMatchHander?.Invoke(data);
        }


        #endregion

        
    }

}