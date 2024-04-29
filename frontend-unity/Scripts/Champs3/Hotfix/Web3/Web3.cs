using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using QFramework;
#if MetamaskPlugin
using evm.net;
using evm.net.Models;
using MetaMask;
using MetaMask.Models;
using MetaMask.Unity;
using champs3.Hotfix.MetaMask;
using MetaMask.Transports.Unity;
#endif
using UnityCommon;
using UnityCommon.Util;
using UnityEngine;


using Random = UnityEngine.Random;

namespace champs3.Hotfix
{
    public class Web3 : MonoSingleton<Web3>
    {
        #region EDITOR EXPOSED FIELDS

        public string Account;

        public MetaMaskHandler MetaMaskHandler;
        public OnUpdateBalance UpdateBalanceHandler;

        public OnBuyGoodsHandler BuyGoodsHandler;

        public OnUpgradeRoleHandler UpgradeRoleHandler;
        public OnExchangeEnergyHandler ExchangeEnergyHandler;
        public OnTransactionErrorHandler TransactionErrorHandler;
        public OnMatchHandler MatchHandler;
        public OnGetRewardPoolHandler GetRewardPoolHandler;
        public OnRotateSpinHandler RotateSpinHandler;
        public OnApproveGameHandler ApproveGameHandler;
        public delegate void OnUpdateBalance(float balance);
        public delegate void OnApproveGameHandler(ApproveOrder order);
        public delegate void OnBuyGoodsHandler(OrderInfo orderInfo);
        public delegate void OnUpgradeRoleHandler(UpgradeRoleOrderInfo orderInfo);
        public delegate void OnExchangeEnergyHandler(ExchangeEnergyOrderInfo orderInfo);
        public delegate void OnTransactionErrorHandler();
        public delegate void OnMatchHandler(bool result);
        public delegate void OnRotateSpinHandler(bool result);
        public delegate void OnGetRewardPoolHandler(RewardPool result);
        #endregion

        #region FIELDS

        #endregion

        #region PROPERTIES



        
        
        #endregion

        #region EVENT FUNCTION

        private void Awake()
        {
            mInstance = this;
            MetaMaskHandler = new MetaMaskHandler();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            MetaMaskHandler.Dispose();
        }

        #endregion

        #region METHODS

        private static bool _initializedMetamask;
        public void ConnectToMetaMask()
        {
#if UNITY_WEBGL && !MetamaskPlugin
            Web3JSLib.ConnectToMetaMask();
#else
            if (!_initializedMetamask)
            {
                MetaMaskHandler.ConnectedHandler += LoginHelper.OnMetaMaskConnected;
                MetaMaskHandler.RegisterHandler(MetaMaskMethodName.personal_sign, LoginHelper.OnMetaMaskSign);
                MetaMaskAdatper.Instance.Initlized();
                MetaMaskAdatper.Instance.onTransactionResult += OnTransactionResult;
                MetaMaskUnity.Instance.Events.EthereumRequestFailed += OnTransactionFail;
                MetaMaskAdatper.Instance.onTransactionSend += EthereumRequestResultSend;
                MetaMaskAdatper.Instance.onWalletConnected += (sender, args) =>
                {
                   MetaMaskAdatper.Instance.Requeat(new MetaMaskEthereumRequest(){Method = "eth_accounts", Parameters = new string[]{}});
                };
            }

            MetaMaskAdatper.Instance.Connect();
#endif
        }

        
        public void MetaMaskRequest(MetaMaskMethodName methodName, object paramsList)
        {
            var paramsJson = JsonUtil.ToJson(paramsList);
            LogKit.I($"MetaMask Call Rpc Method [{methodName} : {paramsJson}");
            string jsonStr = $"{{\"method\": \"{methodName}\", \"params\": {paramsJson}}}";
#if UNITY_WEBGL && !MetamaskPlugin
            Web3JSLib.MetaMaskRequest(jsonStr);
#else 
            MetaMaskAdatper.Instance.Requeat(new MetaMaskEthereumRequest(){Method = methodName.ToString(), Parameters = paramsList});
#endif
        }
        





        public void StartMatch(MartchInfo martchInfo)
        {
#if UNITY_WEBGL && !MetamaskPlugin
            
            Web3JSLib.StartMatch(martchInfo.ToJson(), Account);
            return;
#else
            StartMatchInternal(martchInfo);
#endif

        }



        

        public void ExchangeEnergy(long token, int amount)
        {
            LogKit.I($"ExchangeEnergy : {token}, {Account}");
#if UNITY_WEBGL && !MetamaskPlugin
            Web3JSLib.ExchangeEnergy(new ExchangeEnergyOrderInfo(){RoleToken = token, Amount = amount}.ToJson(), Account);
#else
            ExchangeEnergyInternal(new ExchangeEnergyOrderInfo(){RoleToken = token, Amount = amount});
#endif
        }
        
        public void RotateSpin()
        {
            int id = Random.Range(0, 1000);
            
#if UNITY_WEBGL && !MetamaskPlugin
            
            Web3JSLib.RotateSpin(id, Account);
            return;
#else
            RotateSpinInternal(id);
#endif
        }


        

        public void GetBalance()
        {
#if UNITY_WEBGL && !MetamaskPlugin
            
            Web3JSLib.GetBalance(Account);
            return;
#else
            GetBalanceInternal();
#endif
        }


        public void GetRewardPool()
        {
            Web3JSLib.GetRewardPool(Account);
        }

        public void OnGetRewardPool(string result)
        {
            if (GetRewardPoolHandler != null)
            {
                var rewardPool = JsonUtil.FromJson<RewardPool>(result);
                GetRewardPoolHandler(rewardPool);
            }
        }

        public void OnMatch(string result)
        {
            
            OnMatch(bool.Parse(result));
        }
        
        public void OnMatch(bool result)
        {
            if (MatchHandler != null)
            {
                MatchHandler(result);
            }
        }

        public void BuyGoods(OrderInfo orderInfo)
        {
#if UNITY_WEBGL && !MetamaskPlugin
            Web3JSLib.BuyGoods(JsonUtil.ToJson(orderInfo), Account);
#else
            BuyGoodsInternal(orderInfo);
#endif
        }
        
        public void UpgradeRole(UpgradeRoleOrderInfo orderInfo)
        {
            
            
#if UNITY_WEBGL && !MetamaskPlugin
            
            Web3JSLib.UpgradeRole(JsonUtil.ToJson(orderInfo), Account);
            return;
#else
            UpgradeRoleInternal(orderInfo);
#endif
        }

        public void ApproveMatchGame(int gameType, bool approved)
        {
#if MetamaskPlugin
            LogKit.I($"ApproveMatchGame : {gameType} , {approved}");
            ApproveGameInternal(new ApproveOrder()
            {
                GameType = gameType,
                Approve = approved
            });
#else
            Web3JSLib.ApproveMatchGame(new ApproveOrder(){GameType = gameType, Approve = approved}.ToJson(), Account);
#endif
        }
        
#if MetamaskPlugin
        private TokenContract.TokenContract _tokenContract;
        private GameLogicContract.GameLogicContract _gameLogicContract;
        private SpinContract.SpinContract _spinContract;
        private ShopContract.ShopContract _shopContract;
        private RunHeroContract.RunHeroContract _heroContract;
        private List<object> orderList = new List<object>();
        private readonly string TokenContractAddress = "0x78c7467C0Ae34fAfc42167f7307BE65665b6D3Bd";
        private readonly string GameLogicContractAddress = "0x18fDdAeB739A1E25F85A97E922cdB750C1d38be5";
        private readonly string SpinContractAddress = "0xD246Dc3E7C4775D435101Aba6B0eA1F906e12107";
        private readonly string ShopContractAddress = "0xB3B3599492d336F8820Dec731F350B6EE151Ff85";
        private readonly string HeroContractAddress = "0x4674D1ddd691BE67032253E6E97FD291Bb160CAb";
        private void InitContract()
        {
            _tokenContract = Contract.Attach<TokenContract.TokenContract>(MetaMaskUnity.Instance.Wallet, TokenContractAddress);
            _gameLogicContract = Contract.Attach<GameLogicContract.GameLogicContract>(MetaMaskUnity.Instance.Wallet, GameLogicContractAddress);
            _spinContract = Contract.Attach<SpinContract.SpinContract>(MetaMaskUnity.Instance.Wallet, SpinContractAddress);
            _shopContract = Contract.Attach<ShopContract.ShopContract>(MetaMaskUnity.Instance.Wallet, ShopContractAddress);
            _heroContract = Contract.Attach<RunHeroContract.RunHeroContract>(MetaMaskUnity.Instance.Wallet, HeroContractAddress);
        }
        public async void CheckApproveBalance(long price, string address)
        {
            var balance = await  _tokenContract.Allowance(Account, address);

            if (balance < price)
            {
                await _tokenContract.Approve(address, 9999000000);
            }
        }

        private Dictionary<string, object> waitOrder = new Dictionary<string, object>();
        private string accountRequestId;
        private void EthereumRequestResultSend(object sender, MetaMaskUnityRequestEventArgs args)
        {
            if (args.Request.Method == "eth_sendTransaction")
            {
                if (orderList.Count > 0)
                {
                    var order = orderList[0];
                    waitOrder.Add(args.Request.Id, order);
                    orderList.RemoveAt(0);
                }
            }

            if (args.Request.Method == "eth_requestAccounts")
            {
                accountRequestId = args.Request.Id;
            }
        }

        protected void AddNetWork()
        {
            var networkSwitcher = MetaMaskUnity.Instance.GetComponent<NetworkSwitcher>();
            networkSwitcher.AddNetwork();
        }

        protected void SwitchNetwork()
        {
            var networkSwitcher = MetaMaskUnity.Instance.GetComponent<NetworkSwitcher>();
            networkSwitcher.SwitchNetwork();
        }

        protected bool CheckChainId(string chainId)
        {
            var networkSwitcher = MetaMaskUnity.Instance.GetComponent<NetworkSwitcher>();

            return networkSwitcher.chainToSwitchTo.ChainId == chainId;
        }

        protected void OnTransactionResult(object sender, MetaMaskEthereumRequestResultEventArgs args)
        {
            Dictionary<string, object> jsonDecode = MiniJson.JsonDecode(args.Result) as Dictionary<string, object>;
            LogKit.I("Test-----" + args.ToJson());

            switch (args.Request.Method)
            {
                case "wallet_switchEthereumChain":
                    SwitchNetwork();
                    break;
                    
                case "eth_chainId":
                    LogKit.I($"Test -{args.Result}");
                    var chainIdStr = jsonDecode["result"] as string;
                    if (!CheckChainId(chainIdStr))
                    {
                        AddNetWork();
                    }
                    break;
                case "eth_accounts":
                case "eth_requestAccounts":
                LogKit.I($"eth_accounts {args.Request.Id}-{args.Result}");
                    var objects = jsonDecode["result"] as List<object>;
                    if (objects.Count > 0)
                    {
                        string account = (string)objects[0];
                        LogKit.I($"Account : {account}");
                        if (string.IsNullOrEmpty(Account))
                        {
                            Account = account;
                            InitContract();
                            MetaMaskHandler.ConnectedHandler?.Invoke(Account);
                        }
                    }


                    break;
                case "personal_sign":
                    string sign = jsonDecode["result"] as string;
                    MetaMaskHandler.InvokeHander(args.Request.Method, sign);
                    break;
                case "eth_sendTransaction":
                    if (waitOrder.TryGetValue(args.Request.Id, out object order))
                    {
                        if (order is MartchInfo martchInfo)
                        {
                            OnMatch(true);
                            
                            return;
                        }

                        if (order is UpgradeRoleOrderInfo upgradeRoleOrderInfo)
                        {
                            OnUpgradeRole(upgradeRoleOrderInfo);
                            return;
                        }

                        if (order is ExchangeEnergyOrderInfo energyOrderInfo)
                        {
                            OnExchangeEnergy(energyOrderInfo);
                            return;
                        }

                        if (order is OrderInfo shopOrder)
                        {
                            OnBuyGood(shopOrder);
                            return;
                        }

                        if (order is ApproveOrder approveOrder)
                        {
                            approveOrder.OnCompleted.Invoke();
                            return;
                        }

                        if (order is SpinOrder spinOrder)
                        {
                            OnRotateSpin(true);
                        }
                    }
                    break;
            }
        }
        
        protected void OnTransactionFail(object sender, MetaMaskEthereumRequestFailedEventArgs args)
        {
            switch (args.Request.Method)
            {
                case "eth_sendTransaction":
                    if (waitOrder.TryGetValue(args.Request.Id, out object order))
                    {
                        if (order is SpinOrder)
                        {
                            OnRotateSpin(false);
                        }
                        else
                        {
                            OnTransactionError();
                        }
                    }
                    break;
            }
			
        }




        private void  ApproveGameInternal(ApproveOrder order)
        {

            
            order.OnCompleted = () => {  OnApproveMatchGame(order); };
            
            orderList.Add(order);

            if (order.GameType == 1)
            {
                _heroContract.ApproveGame(order.Approve);
            }
            else
            {
                _tokenContract.ApproveGame(order.Approve);
            }
        }
        

        private void RotateSpinInternal(int id)
        {
            try
            {
                SpinOrder order = new SpinOrder();
                orderList.Add(order);
                _spinContract.RequestRandomWords(id, new CallOptions(){Gas = "400000"});
                
            }
            catch (Exception e)
            {
                LogKit.I(e.Message);
                OnTransactionError();
                throw;
            }
        }

        private async void BuyGoodsInternal(OrderInfo orderInfo)
        {
            BigInteger approve =  await _tokenContract.Allowance(Account, ShopContractAddress);
            var price = new BigInteger(orderInfo.Amount * orderInfo.Price * TokenHelper.TokenAccuracy);
            LogKit.I($"Goods {orderInfo.GoodsId}, prive {price}, approve {approve}");
            if (approve <  price)
            {
                ApproveOrder order = new ApproveOrder(){OnCompleted = () =>
                {
                    orderList.Add(orderInfo);
                    _shopContract.BuyGood(orderInfo.GoodsId, orderInfo.Amount);
                } };
                orderList.Add(order);
                _tokenContract.Approve(ShopContractAddress, 99999000000);
            }
            else
            {
                orderList.Add(orderInfo);
                _shopContract.BuyGood(orderInfo.GoodsId, orderInfo.Amount);
            }
        }

        private async void GetBalanceInternal()
        {
            var balance = await _tokenContract.BalanceOf(Account);
           
            UpdateBalance(balance.ToString());
        }

        private void ExchangeEnergyInternal(ExchangeEnergyOrderInfo orderInfo)
        {
            orderList.Add(orderInfo);
            _gameLogicContract.ExchangeEnergy(orderInfo.RoleToken, orderInfo.Amount);
        }

        public async void UpgradeRoleInternal(UpgradeRoleOrderInfo orderInfo)
        {
            BigInteger approve =  await _tokenContract.Allowance(Account, GameLogicContractAddress);
            LogKit.I($"UpgradeRole {orderInfo.RoleToken}, prive {orderInfo.Price}, approve {approve}");
            if (approve < new BigInteger(orderInfo.Price * TokenHelper.TokenAccuracy))
            {
                ApproveOrder order = new ApproveOrder(){OnCompleted = () =>
                {
                    orderList.Add(orderInfo);
                    _gameLogicContract.RoleUpgrade(orderInfo.RoleToken);
                } };
                orderList.Add(order);
                _tokenContract.Approve(GameLogicContractAddress, 99999000000);
            }
            else
            {
                orderList.Add(orderInfo);
                _gameLogicContract.RoleUpgrade(orderInfo.RoleToken, new CallOptions(){Gas = "400000"});
            }
        }
        
        private async  void StartMatchInternal(MartchInfo martchInfo)
        {
            LogKit.I("StartMatchInternal : " + martchInfo.ToJson());
            
            BigInteger approve =  await _tokenContract.Allowance(Account, GameLogicContractAddress);
            var price = new BigInteger(martchInfo.PvPCost * TokenHelper.TokenAccuracy);
            LogKit.I($"StartMatch approve {approve}, {price}");
            if (approve < price)
            {
                ApproveOrder order = new ApproveOrder(){OnCompleted = () =>
                {
                    orderList.Add(martchInfo);
                    _gameLogicContract.StartPvpGame();
                } };
                orderList.Add(order);
                _tokenContract.Approve(GameLogicContractAddress, 99999000000);
            }
            else
            {
                orderList.Add(martchInfo);
                _gameLogicContract.StartPvpGame();
            }
        }


        // private async bool CheckApprove(string from, long price)
        // {
        //     BigInteger approve =  await _tokenContract.Allowance(Account, ShopContractAddress);
        //
        //     if (approve < price)
        //     {
        //         ApproveOrder order = new ApproveOrder(){OnCompleted = () =>
        //         {
        //             
        //         } };
        //         _tokenContract.Approve(from, 99999000000);
        //     }
        // }

        private async void ApproveShop(string from)
        {
            
            BigInteger approve =  await _tokenContract.Allowance(Account, ShopContractAddress);
            LogKit.I("Approve success " + approve);
            await _tokenContract.Approve(from, 99999000000);
            
            LogKit.I("Approve success");
        }

#endif
        
        public void OnTransactionError()
        {
            TransactionErrorHandler?.Invoke();
        }

        public void OnExchangeEnergy(string orderInfo)
        {
            OnExchangeEnergy(JsonUtil.FromJson<ExchangeEnergyOrderInfo>(orderInfo));
        }
        
        public void OnExchangeEnergy(ExchangeEnergyOrderInfo orderInfo)
        {
            if (ExchangeEnergyHandler != null)
            {
                ExchangeEnergyHandler(orderInfo);
            }
        }

        public void OnRotateSpin(string result)
        {

            OnRotateSpin(bool.Parse(result));
            
        }
        
        public void OnRotateSpin(bool result)
        {
            if (RotateSpinHandler != null)
            {
                RotateSpinHandler.Invoke(result);
            }
        }
        
        public void OnUpgradeRole(string orderInfo)
        {
            var upgradeRoleOrderInfo = JsonUtil.FromJson<UpgradeRoleOrderInfo>(orderInfo);
            OnUpgradeRole(upgradeRoleOrderInfo);
        }
        
        public void OnApproveMatchGame(string orderStr)
        {
            var approveOrder = JsonUtil.FromJson<ApproveOrder>(orderStr);
            
            OnApproveMatchGame(approveOrder);
        }
        
        public void OnApproveMatchGame(ApproveOrder order)
        {
            ApproveGameHandler?.Invoke(order);
        }

        public void OnUpgradeRole(UpgradeRoleOrderInfo orderInfo)
        {
            if (UpgradeRoleHandler != null)
            {
                UpgradeRoleHandler(orderInfo);
            }
        }

        public void OnBuyGood(string orderInfo)
        {

            OnBuyGood(JsonUtil.FromJson<OrderInfo>(orderInfo));
            
        }
        
        public void OnBuyGood(OrderInfo orderInfo)
        {
            if (BuyGoodsHandler != null)
            {
                BuyGoodsHandler(orderInfo);
            }
        }

        public void OnWalletConnect(string jsonResult)
        {
            var walletConnectResult = JsonUtility.FromJson<WalletConnectResult>(jsonResult);

            Account = walletConnectResult.Accounts[0];
            MetaMaskHandler.ConnectedHandler?.Invoke(Account);
        }

        public void MetaMaskRequestHandler(string jsonResult)
        {
            var metaMaskRequestResult = JsonUtility.FromJson<MetaMaskRequestResult>(jsonResult);
            LogKit.I($"Handler [{metaMaskRequestResult.MethodName}] : {metaMaskRequestResult.Result}");
            MetaMaskHandler.InvokeHander(metaMaskRequestResult.MethodName, metaMaskRequestResult.Result);
        }

        public void UpdateBalance(string balance)
        {
            LogKit.I($"UpdateBalance {balance}");
            var value = float.Parse(balance) / TokenHelper.TokenAccuracy;
            UpdateBalanceHandler?.Invoke(value);
        }


        public void TestPersonalSign()
        {
            string[] ps = new[] { Hexer("123456789"), Account };
            MetaMaskRequest(MetaMaskMethodName.personal_sign, ps);
        }

        public void TestAddChain()
        {
            var ethereumChainInfo = new EthereumChainInfo()
            {
                chainName = "Mumbai",
                chainId = "0x13881",
                rpcUrls = new string[]{"https://rpc-mumbai.maticvigil.com"},
                nativeCurrency = new NativeCurrency(){name = "MATIC", symbol = "MATIC", decimals = 18}
            };
            EthereumChainInfo[] ethereumChainInfos = new EthereumChainInfo[1];
            ethereumChainInfos[0] = ethereumChainInfo;
            MetaMaskRequest(MetaMaskMethodName.wallet_addEthereumChain, ethereumChainInfos);
        }
        
        
        public void TestSwitchChain()
        {
            var ethereumChainInfo = new ChainId()
            {
                chainId = "0x13881",
            };
            ChainId[] chainIds = new ChainId[1];
            chainIds[0] = ethereumChainInfo;
            MetaMaskRequest(MetaMaskMethodName.wallet_switchEthereumChain, chainIds);
        }



        public string Hexer(string str)
        {
            var utf8Array = ToUTF8Array(str);
            StringBuilder sb = new StringBuilder();
            sb.Append("0x");


            for (int i = 0; i < utf8Array.Length; i++)
            {
                sb.Append(Convert.ToString(utf8Array[i], 16));
            }

            return sb.ToString();
        }

        public int[] ToUTF8Array(string str)
        {
            var utf8 = new List<int>();
            
            for (int i = 0; i < str.Length; i++)
            {
                var charCode = str[i];
                if (charCode < 0x80)
                {
                    utf8.Add(charCode);
                }else if (charCode < 0x800)
                {
                    utf8.Add(0xc0 | (charCode >> 6));
                    utf8.Add(0x80 | (charCode & 0x3f));
                }
                else if (charCode < 0xd800 || charCode >= 0xe000)
                {
                    utf8.Add(0xe0 | (charCode >> 12));
                    utf8.Add(0x80 | ((charCode >> 6) & 0x3f));
                    utf8.Add( 0x80 | (charCode & 0x3f));
                }
            }

            return utf8.ToArray();
        }
        

        #endregion
        
    }


    public class MetaMaskHandler : IDisposable
    {
        private Dictionary<string, OnMetaMaskRquestCallback> _metaMaskRquestHandler;
        public OnMetaMaskConnectedCallback ConnectedHandler;
        public delegate void OnMetaMaskConnectedCallback(string account);
        public delegate void OnMetaMaskRquestCallback(string result);
        


        public MetaMaskHandler()
        {
            _metaMaskRquestHandler = new Dictionary<string, OnMetaMaskRquestCallback>();
        }

        public void RegisterHandler(MetaMaskMethodName methodName, OnMetaMaskRquestCallback callback)
        {
            if (_metaMaskRquestHandler.TryGetValue(methodName.ToString(),
                    out OnMetaMaskRquestCallback handler))
            {
                handler += callback;
            }
            else
            {
                _metaMaskRquestHandler.Add(methodName.ToString(), callback);
            }
        }
        
        public void UnRegisterHandler(MetaMaskMethodName methodName, OnMetaMaskRquestCallback callback)
        {
            if (_metaMaskRquestHandler.TryGetValue(methodName.ToString(),
                    out OnMetaMaskRquestCallback handler))
            {
                handler -= callback;
            }
            else
            {
                LogKit.I($"Not Contain Handler[{methodName}] : {callback.ToString()}");
            }
        }

        public void InvokeHander(string methodName, string result)
        {
            if (_metaMaskRquestHandler.TryGetValue(methodName,
                    out OnMetaMaskRquestCallback handler))
            {
                handler.Invoke(result);
            }
        }
        

        public void Dispose()
        {
            _metaMaskRquestHandler.Clear();
            _metaMaskRquestHandler = null;
            ConnectedHandler = null;
        }
    }

    public enum WalletType
    {
        MetaMask
    }


    public struct WalletConnectResult
    {
        public string Wallet;
        public string[] Accounts;
    }


    public enum MetaMaskMethodName
    {
        wallet_addEthereumChain,
        wallet_switchEthereumChain,
        personal_sign,
        
    }


    public interface MetaMaskRequestParams
    {
        string method { get; }
    }

    

    public struct MetaMaskRequestResult
    {
        public string MethodName;
        public string Result;
    }


    public struct EthereumChainInfo
    {
        public string chainId;
        public string chainName;
        public string[] rpcUrls;
        public NativeCurrency nativeCurrency;
    }

    public struct NativeCurrency
    {
        public string name;
        public string symbol;
        public int decimals;
    }

    public struct ChainId
    {
        public string chainId;
    }
    
    public struct OrderInfo
    {
        public long GoodsId;
        public int Amount;
        public float Price;
    }

    public struct SpinOrder
    {
        
    }

    public struct UpgradeRoleOrderInfo
    {
        public long RoleToken;
        public long Price;
        public int Level;
    }

    public struct ExchangeEnergyOrderInfo
    {
        public long RoleToken;
        public int Amount;
    }

    public struct MartchInfo
    {
        public int GameMode;
        public long RaceMapKey;
        public long RoleToken;
        public long PropsToken;
        public float PvPCost;
    }

    public struct RewardPool
    {
        public string JackPot;
        public string BonusPool;
    }

    public struct ApproveOrder
    {
        public int GameType;
        public bool Approve;
        public Action OnCompleted;
    }
    

}
