// 
// 2023/12/15


using System;
using System.Collections;
using System.Text;
using QFramework;
using SquareHero.Hotfix.GameLogic;
using SquareHero.Hotfix.Generate;
using SquareHero.Hotfix.Main;
using SquareHero.Hotfix.Model;
using SquareHero.Hotfix.Net;
using SquareHero.Hotfix.Toast;
using SquareHero.Hotfix.UI;
using UnityCommon.Util;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Video;


namespace SquareHero.Hotfix
{
    public static class LoginHelper
    {
        public static string Token = "7Gr9Bija2sSZeA1Fw31x5jQfHMWQgsXZLhz4AdsAPmbk";
        public static string Acccount = "760996255@qq.com";
        public static string Passward = "594Dcs00";

        private static string _account;
        private static string _nonce;
        private static string _signature;
        
        public static void Initialize()
        {
            // var locationURL = SHWebGLPlugin.GetLocationURLFunction();
            
            // LogKit.I($"location url : {locationURL}");
            
            // GetTokenFromUrl(locationURL);
            Web3.Instance.MetaMaskHandler.ConnectedHandler += OnMetaMaskConnected;
            Web3.Instance.MetaMaskHandler.RegisterHandler(MetaMaskMethodName.personal_sign, OnMetaMaskSign);


            PetraAdapter.Instance.OnSignHander += OnPetraSign;

        }


        private static void GetTokenFromUrl(string url)
        {
            var paramats = url.Split("?");

            for (int i = 0; i < paramats.Length; i++)
            {
                var strs = paramats[i].Split("=");
                if (strs.Length < 2)
                {
                    LogKit.E($"Url error  paramat : {paramats[i]}");
                    continue;
                }
                
                if (strs[0] == "token")
                {
                    Token = strs[1];
                }
                
                
                if (strs[0] == "account")
                {
                    Acccount = strs[1];
                }


                if (strs[0] == "passward")
                {
                    Passward = strs[1];
                }

                if (strs[0] == "debug")
                {
                    ConstValue.Debug = bool.Parse(strs[1]);
                    LogKit.I("Debug Mode open");
                }
            }
        }
        
        public static void Login()
        {
            UIKit.DisableUIInput();

            try
            {
                if (LoginHelper.Token.IsNullOrEmpty())
                {
                    var enumerator = Login(GameUrlConstValue.Login.Url(),
                        result =>
                        {
                            var loginResponse = JsonUtil.FromJson<LoginResponse>(result);
                            if (loginResponse.code == 0)
                            {
                                HttpHelper.Token = loginResponse.data;
                                Token = loginResponse.data;
                                RunxNetManager.Instance.Login();
                            }
                        });
                    Global.Instance.StartCoroutine(enumerator);
                }
                else
                {
                    HttpHelper.Token = LoginHelper.Token;
                    // SHNetManager.Instance.Login();
                    RunxNetManager.Instance.Login();
                }
            }
            catch (Exception e)
            {
                UIKit.EnableUIInput();
                throw e;
            }
            

        }
        
        public static void ConnecMetaMask()
        {
            Web3.Instance.ConnectToMetaMask();
        }

        public static void ConnectPreta()
        {
            PetraAdapter.Instance.ConnectPetra();
        }

        public static void Sign(string nonce, string account)
        {
            // MetaMaskJsLib.MetaMaskSign(nonce, account, OnMetaMaskSign);
            string[] ps = new[] {  Web3.Instance.Hexer(nonce), account };
            Web3.Instance.MetaMaskRequest(MetaMaskMethodName.personal_sign, ps);
        }


        public static void OnMetaMaskConnected(string account)
        {
            _account = account;
            // Web3.Instance.TestAddChain();
            ActionKit.DelayFrame(1, () =>
            {
                GetNonce(_account);
            }).Start(Global.Instance);
        }

        private static void GetNonce(string account)
        {
            // var token = PlayerPrefs.GetString($"Token_x_{account}", "");
            //
            // bool needGetNonce = true;
            // if (!string.IsNullOrEmpty(token))
            // {
            //     var split = token.Split("_x_");
            //
            //     long timeStamp = long.Parse(split[0]);
            //
            //     var nowTimeStamp = DateTimeHelper.DataTimeToTimestamp(DateTime.UtcNow);
            //
            //     if (nowTimeStamp - timeStamp < 604800)
            //     {
            //         needGetNonce = false;
            //
            //         Token = split[1];
            //     }
            // }
            //
            // if (needGetNonce)
            // {
                Global.Instance.StartCoroutine(GetNonce(GameUrlConstValue.GetNonce.Url(), account, result =>
                {
                    var getNonceResponse = JsonUtil.FromJson<GetNonceResponse>(result);

                    if (getNonceResponse.code == 0)
                    {
                        _nonce = getNonceResponse.data.nonce;
                        Sign(getNonceResponse.data.nonce, account);
                    }
                }));
            // }
            // else
            // {
            //     RunxNetManager.Instance.Login();
            // }

        }
        

        public static void OnMetaMaskConnectFail(int code, string message)
        {
            
        }
        

        public static void OnMetaMaskSign(string signData)
        {
            _signature = signData;
            Global.Instance.StartCoroutine(LoginByWallet(GameUrlConstValue.WalletLogin.Url(), result =>
            {
                LogKit.I($"Login result : {result}");

                var loginResponse = JsonUtil.FromJson<WalletLoginResponse>(result);
                if (loginResponse.code == 0)
                {
                    HttpHelper.Token = loginResponse.data;
                    long timestamp = DateTimeHelper.DataTimeToTimestamp(DateTime.UtcNow);
                    PlayerPrefs.SetString($"Token_x_{_account}", $"{timestamp}_x_{HttpHelper.Token}");
                    Token = loginResponse.data;
                    ActionKit.DelayFrame(1, () =>
                    {
                        RunxNetManager.Instance.Login();
                    }).Start(Global.Instance);
                }
            }));
            
        }
        
        public static void OnAddEthereumChain(string result)
        {
            
           Web3.Instance.TestSwitchChain();

        }

        public static void OnSwitchEthereumChain(string result)
        {
            ActionKit.DelayFrame(1, () =>
            {
                GetNonce(_account);
            }).Start(Global.Instance);
        }


        private static IEnumerator GetNonce(string url, string account, Action<string> onCompleted)
        {
            string address = $"{url}?wallet_address={account}";
            LogKit.I($"Get Address : {address}");
            UnityWebRequest request = new UnityWebRequest(address, "Get");
            request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
            request.SetRequestHeader("x-app-id", "7a8hxez99pgvdIroTaDZh9mfON97LO1wldDGVMwEhWsHo");
            request.SetRequestHeader("x-app-key", "3XEhbwprbgf3akD8JplkTxYgx8JUIQT75hOIc9ldd8Wv6");
            request.SetRequestHeader("app", "admin");
            request.SetRequestHeader("chain-id", "80001");
            LogKit.I($"Get Address2 : ");
            yield return request.SendWebRequest();
            LogKit.I($"Get Address3 : ");
            if (request.isDone)
            {
                var result = request.downloadHandler.text;
                LogKit.I($"Get {address } done result is : {result}");
                onCompleted?.Invoke(result);
                request.Dispose();
            }
        }
        
        
        private static IEnumerator LoginByWallet(string url, Action<string> onCompleted)
        {
            WalletLoginData walletLoginData = new WalletLoginData()
            {
                wallet_address = _account,
                nonce = _nonce,
                signature = _signature
            };
            string data = JsonUtil.ToJson(walletLoginData);
            LogKit.I($"Post {url } - data :{data}");
            UnityWebRequest request = new UnityWebRequest(url, "Post");
            request.SetRequestHeader("x-app-id", "7a8hxez99pgvdIroTaDZh9mfON97LO1wldDGVMwEhWsHo");
            request.SetRequestHeader("x-app-key", "3XEhbwprbgf3akD8JplkTxYgx8JUIQT75hOIc9ldd8Wv6");
            request.SetRequestHeader("app", "admin");
            request.SetRequestHeader("chain-id", "80001");
            request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

            byte[] bytes = Encoding.UTF8.GetBytes(data);
            
            request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bytes);
            request.uploadHandler.contentType = "application/json";
            yield return request.SendWebRequest();
            UIKit.EnableUIInput();
            if (request.isDone)
            {
                var result = request.downloadHandler.text;
                LogKit.I($"Post {url } done result is : {result}");
                onCompleted?.Invoke(result);
                request.Dispose();
            }
        }

        private static IEnumerator Login(string url, Action<string> onCompleted)
        {
            string data = $"{{\"password\":\"{LoginHelper.Passward}\",\"mobile_or_email\":\"{LoginHelper.Acccount}\"}}";
            LogKit.I($"Post {url } - data :{data}");
            UnityWebRequest request = new UnityWebRequest(url, "Post");
            request.SetRequestHeader("x-app-id", "WADuzvdLJPM5g24eoS8qre34nU2biapDgCQgAwIOWHtCe");
            request.SetRequestHeader("x-app-key", "gGpixYKzIk2JXcBPsrWzan7oCtFWKDfchuFcC3aryPdzg");
            request.SetRequestHeader("app", "admin");
            request.SetRequestHeader("chain-id", "80001");
            request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

            byte[] bytes = Encoding.UTF8.GetBytes(data);
            
            request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bytes);
            request.uploadHandler.contentType = "application/json";
            yield return request.SendWebRequest();
            UIKit.EnableUIInput();
            if (request.isDone)
            {
                var result = request.downloadHandler.text;
                LogKit.I($"Post {url } done result is : {result}");
                onCompleted?.Invoke(result);
                request.Dispose();
            }
        }

        public static void OnLogin(G2C_Login msg)
        {
            UIKit.OpenPanelAsync<LoadingPanel>(panel =>
            {
                UIKit.HidePanel<WaitingPanel>();
            });
            // Hotfix.Global.Instance.LoadingScreen.ShowLoading();
            UIKit.EnableUIInput();
            LogKit.I($"Login Success : {msg.ToJson()}");
            
            var gameStartPanel = UIRoot.Instance.Common.GetComponentInChildren<GameStartPanel>();
            ResourceManager.Instance.LoadSceneAsync("Scene_Main", () =>
            {
                LogKit.I($"Start Pnale : {gameStartPanel}");
                ExcelConfig.Initialize();

                var mainModel = MainController.Instance.GetArchitecture().GetModel<MainModel>();
                mainModel.Player.PlayerName = msg.UserName;
                mainModel.Player.PlayerCode = msg.PlayerCode;
                mainModel.Player.UserId = msg.UserID;
                mainModel.Player.SkinId = msg.SkinID;
                
                UIKit.OpenPanelAsync<VideoPanel>(panel =>
                {
                    UIKit.OpenPanelAsync<MainPanel>(mainPanel =>
                    {
                        if (gameStartPanel)
                        {
                            GameObject.Destroy(gameStartPanel.gameObject);
                        }
                        UIKit.HidePanel<MetaMaskPanel>();
                    });
                });
                
                UIKit.OpenPanelAsync<ConfirmBoxPanel>(panel =>
                {
                    UIKit.HidePanel<ConfirmBoxPanel>();
                });
                UIKit.OpenPanelAsync<MaskPanel>(null, UILevel.PopUI);
            });
            

        }
        public static void OnDisconnectServer()
        {

            var confirmBoxPanel = UIKit.GetPanel<NetworkErrorPanel>();
            if (confirmBoxPanel != null && confirmBoxPanel.State == PanelState.Opening)
            {
                return;
            }
            UIKit.OpenPanelAsync<NetworkErrorPanel>(panel =>
            {
                
                Action<G2C_Login> callback = null;
                callback = login =>
                {
                    UIKit.EnableUIInput();
                    var mainPanel = UIKit.GetPanel<MainPanel>();
                    if (mainPanel != null && mainPanel.State != PanelState.Closed)
                    {
                        if (mainPanel.State == PanelState.Opening)
                        {
                            UIKit.HidePanel<NetworkErrorPanel>();
                        }
                        else
                        {
                            var maskPanel = UIKit.GetPanel<MaskPanel>();
                            maskPanel.FadeIn(() =>
                            {
                                UIKit.HideOtherAllPanel<MainPanel>();
                                UIKit.ShowPanel<MainPanel>();
                                UIKit.ShowPanel<VideoPanel>();
                                maskPanel.FadeOut();
                            });
                        }
                    }
                    else
                    {
                        // Hotfix.Global.Instance.LoadingScreen.HideLoading();
                        UIKit.CloseAllOtherPanel<LoadingPanel>();
                        var activeScene = SceneManager.GetActiveScene();
                        if (LevelManager.isCreated)
                        {
                            var playerModel = LevelManager.Instance.CurrentLevelController.GetArchitecture().GetModel<PlayerModel>();
                            playerModel.PlayerModes.Clear();
                        }
                        var videoPlayerGameObject = Global.Instance.VideoPlayer.gameObject;
                        var instantiate = GameObject.Instantiate(videoPlayerGameObject, videoPlayerGameObject.transform.parent);
                        var videoPlayer = instantiate.GetComponent<VideoPlayer>();
                        GameObject.Destroy(videoPlayerGameObject);
                        Global.Instance.VideoPlayer = videoPlayer;
                        Global.Instance.VideoPlayer.url = "https://resource.squarehero.io/HotfixAsset/WebGL/Video/Main.mp4";
                        Global.Instance.VideoPlayer.targetTexture = new RenderTexture(1920, 1080, 16);
                        videoPlayer.Play();
                        OnLogin(login);

                    }

                    RunxNetManager.UnRegisterHandler<G2C_Login>(KeepOpCode.Op_G2C_Login, callback);
                };

                RunxNetManager.RegisterHandler<G2C_Login>(KeepOpCode.Op_G2C_Login, callback);
                    
                panel.Content.text = "Network Error!";
                panel.Cancle.gameObject.SetActive(false);
                panel.Confirm.onClick.RemoveAllListeners();
                panel.Confirm.onClick.AddListener(LoginHelper.Login);
            });
        }


        public static void OnPetraSign(string data)
        {

            if (string.IsNullOrEmpty(data))
            {
                return;
            }
            
            var gameStartPanel = UIRoot.Instance.Common.GetComponentInChildren<GameStartPanel>();
            UIKit.OpenPanelAsync<LoadingPanel>(panel =>
            {
                UIKit.HidePanel<WaitingPanel>();
                ResourceManager.Instance.LoadSceneAsync("Scene_Main", () =>
                {
                    UIKit.OpenPanelAsync<MainPanel>(mainPanel =>
                    {
                        if (gameStartPanel)
                        {
                            GameObject.Destroy(gameStartPanel.gameObject);
                        }
                        // UIKit.HidePanel<LoadingPanel>();
                    });
                });
            });
        }
    }
}