// 
// 2024/01/26
using System;
using System.Runtime.InteropServices;

namespace SquareHero.Hotfix
{
    public static class Web3JSLib
    {
// #if UNITY_WEBGL && !UNITY_EDITOR
//         [DllImport("__Internal")]
//         public static extern void ConnectToMetaMask();
//         [DllImport("__Internal")]
//         public static extern void MetaMaskRequest(string paramsJson);
//         [DllImport("__Internal")]
//         public static extern void StartMatch(string martchInfo, string account);
//         [DllImport("__Internal")]
//         public static extern void ExchangeEnergy(string orderInfo, string account);
//         [DllImport("__Internal")]
//         public static extern void RotateSpin(int spindId, string account);
//         [DllImport("__Internal")]
//         public static extern void GetBalance(string account);
//         [DllImport("__Internal")]
//         public static extern void GetRewardPool(string account);
//         [DllImport("__Internal")]
//         public static extern void BuyGoods(string orderInfo, string account);
//         [DllImport("__Internal")]
//         public static extern void UpgradeRole(string orderInfo, string account);
//         [DllImport("__Internal")]
//         public static extern void ApproveMatchGame(string orderInfo, string account);
//         
// #else
        public static void ConnectToMetaMask()
        {
        }


        public static void MetaMaskRequest(string paramsJson)
        {
        }
        
        public static void StartMatch(string martchInfo, string account)
        {
        }

        public static void ExchangeEnergy(string orderInfo, string account)
        {
        }
        
        public static void RotateSpin(int spindId, string account)
        {
        }

        public static void GetRewardPool(string account)
        {
            
        }

        public static void GetBalance(string account)
        {
        }


        public static void BuyGoods(string orderInfo, string account)
        {
        }
        
        public static void UpgradeRole(string orderInfo, string account)
        {
        }
        
        public static void ApproveMatchGame(string orderInfo, string account)
        {
        }

// #endif
    }
    

}