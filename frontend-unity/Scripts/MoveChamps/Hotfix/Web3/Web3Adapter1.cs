// // 
// // 2024/01/26
//
// using System;
// using System.Collections.Generic;
// using QFramework;
// using UnityCommon.Util;
// using UnityEngine;
//
// namespace SquareHero.Hotfix
// {
//     public class Web3Adatper : MonoSingleton<Web3Adatper>
//     {
//         #region EDITOR EXPOSED FIELDS
//
//         public string Account;
//         #endregion
//
//         #region FIELDS
//
//         #endregion
//
//         #region PROPERTIES
//
//         #endregion
//
//         #region EVENT FUNCTION
//
//         private void Awake()
//         {
//             mInstance = this;
//         }
//
//         #endregion
//
//         #region METHODS
//
//         public void ConnectToMetaMask()
//         {
//             Web3JSLib.ConnectToMetaMask();
//         }
//
//         
//         public void MetaMaskRequest(MetaMaskMethodName methodName, object paramsList)
//         {
//             var paramsJson = JsonUtil.ToJson(paramsList);
//             LogKit.I($"MetaMask Call Rpc Method [{methodName} : {paramsJson}");
//             string jsonStr = $"{{\"method\": \"{methodName}\", \"params\": {paramsJson}}}";
//
//             Web3JSLib.MetaMaskRequest(jsonStr);
//         }
//
//         
//         
//         public void StartMatch()
//         {
//             
//             
//             
//             Web3JSLib.StartMatch(Account, "2");
//         }
//         
//         public void OnWalletConnect(string jsonResult)
//         {
//             var walletConnectResult = JsonUtility.FromJson<WalletConnectResult>(jsonResult);
//
//             Account = walletConnectResult.Accounts[0];
//         }
//
//         public void MetaMaskRequestHandler(string jsonResult)
//         {
//             var metaMaskRequestResult = JsonUtility.FromJson<MetaMaskRequestResult>(jsonResult);
//             LogKit.I($"Handler [{metaMaskRequestResult.MethodName}] : {metaMaskRequestResult.Result}"); 
//         }
//
//
//         public void TestPersonalSign()
//         {
//             string[] ps = new[] { "0x13881", Account };
//             MetaMaskRequest(MetaMaskMethodName.personal_sign, ps);
//         }
//
//         public void TestAddChain()
//         {
//             var ethereumChainInfo = new EthereumChainInfo()
//             {
//                 chainName = "Mumbai",
//                 chainId = "0x13881",
//                 rpcUrls = new string[]{"https://polygon-mumbai-pokt.nodies.app","https://endpoints.omniatech.io/v1/matic/mumbai/public"},
//                 nativeCurrency = new NativeCurrency(){name = "MATIC", symbol = "MATIC", decimals = 18}
//             };
//             EthereumChainInfo[] ethereumChainInfos = new EthereumChainInfo[1];
//             ethereumChainInfos[0] = ethereumChainInfo;
//             MetaMaskRequest(MetaMaskMethodName.wallet_addEthereumChain, ethereumChainInfos);
//         }
//         
//         
//         public void TestSwitchChain()
//         {
//             var ethereumChainInfo = new ChainId()
//             {
//                 chainId = "0x13881",
//             };
//             ChainId[] chainIds = new ChainId[1];
//             chainIds[0] = ethereumChainInfo;
//             MetaMaskRequest(MetaMaskMethodName.wallet_addEthereumChain, chainIds);
//         }
//
//         #endregion
//         
//         
//     }
//     
//     
//     public enum WalletType
//     {
//         MetaMask
//     }
//
//
//     public struct WalletConnectResult
//     {
//         public string Wallet;
//         public string[] Accounts;
//     }
//
//
//     public enum MetaMaskMethodName
//     {
//         wallet_addEthereumChain,
//         wallet_switchEthereumChain,
//         personal_sign,
//         
//     }
//
//
//     public interface MetaMaskRequestParams
//     {
//         string method { get; }
//     }
//
//     
//
//     public struct MetaMaskRequestResult
//     {
//         public string MethodName;
//         public string Result;
//     }
//
//
//     public struct EthereumChainInfo
//     {
//         public string chainId;
//         public string chainName;
//         public string[] rpcUrls;
//         public NativeCurrency nativeCurrency;
//     }
//
//     public struct NativeCurrency
//     {
//         public string name;
//         public string symbol;
//         public int decimals;
//     }
//
//     public struct ChainId
//     {
//         public string chainId;
//     }
// }