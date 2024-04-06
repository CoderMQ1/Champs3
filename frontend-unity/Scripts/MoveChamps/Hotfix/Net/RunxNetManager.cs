// 
// 2023/12/13

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using MetaMask.NativeWebSocket;
using QFramework;
using SquareHero.Core;
using SquareHero.Hotfix.Generate;
using SquareHero.Hotfix.UI;
using UnityCommon;
using UnityCommon.Util;
using UnityEngine;

namespace SquareHero.Hotfix.Net
{
    public class RunxNetManager : MonoSingleton<RunxNetManager>
    {
        #region EDITOR EXPOSED FIELDS

        [Tooltip("disables nagle algorithm. lowers CPU% and latency but increases bandwidth")]
        public bool noDelay = true;

        [Tooltip(
            "Send would stall forever if the network is cut off during a send, so we need a timeout (in milliseconds)")]
        public int sendTimeout = 5000;

        [Tooltip("How long without a message before disconnecting (in milliseconds)")]
        public int receiveTimeout = 20000;

        public float HeartTime = 0.5f;

        public bool Connected
        {
            get { return _webSocket!= null && _webSocket.State == WebSocketState.Open; }
        }

        private DateTime _lastHeartTime;
        
        // client //////////////////////////////////////////////////////////////
        /// <summary>Called by Transport when the client connected to the server.</summary>
        public Action OnClientConnected;

        /// <summary>Called by Transport when the client received a message from the server.</summary>
        public Action<ArraySegment<byte>, int> OnClientDataReceived;

        /// <summary>Called by Transport when the client sent a message to the server.</summary>
        // Transports are responsible for calling it because:
        // - groups it together with OnReceived responsibility
        // - allows transports to decide if anything was sent or not
        // - allows transports to decide the actual used channel (i.e. tcp always sending reliable)
        public Action<ArraySegment<byte>, int> OnClientDataSent;

        [Header("Ssl Settings")]
        [Tooltip(
            "Sets connect scheme to wss. Useful when client needs to connect using wss when TLS is outside of transport.\nNOTE: if sslEnabled is true clientUseWss is also true")]
        public bool clientUseWss;

        [Tooltip(
            "Requires wss connections on server, only to be used with SSL cert.json, never with reverse proxy.\nNOTE: if sslEnabled is true clientUseWss is also true")]
        public bool sslEnabled;

        #endregion

        #region FIELDS

        public const string NormalScheme = "ws";
        public const string SecureScheme = "wss";
        public Action OnClientDisconnected;
        string GetClientScheme() => (sslEnabled || clientUseWss) ? SecureScheme : NormalScheme;

        private float _heartTime;
        private UriBuilder _uriBuilder;
        public delegate void MessageDelegate(ushort opcodeId, byte[] data);

        // message handlers by messageId
        internal static readonly Dictionary<ushort, MessageDelegate> handlers =
            new Dictionary<ushort, MessageDelegate>();


        static MessageDelegate WrapHandler<T>(Action<T> handler)
            => (opcodeId, data) =>
            {
                var msg = (T)ProtobufHelper.FromBytes(typeof(T), data, 0, data.Length);
                LogKit.I($"Receive msg : {msg.GetType()}:{msg.ToJson()}");
                handler.Invoke(msg);
            };

        #endregion


        #region PROPERTIES



        #endregion

        #region EVENT FUNCTION

        private void Awake()
        {
            mInstance = this;
            DontDestroyOnLoad(this);
            HeartTime = 2f;
        }

        private void Update()
        {

            if (_webSocket != null && _webSocket.State == WebSocketState.Open)
            {
                
#if UNITY_WEBGL && UNITY_EDITOR
                _webSocket.DispatchMessageQueue();
#endif
                
                
                var time = DateTime.Now;
                var timeSpan = time.Subtract(_lastHeartTime);
                
                if (timeSpan.Seconds >= HeartTime)
                {
                    _webSocket.Send(GetHeartPacket());
                    _lastHeartTime = time;
                }

                
                
                var ushorts = _notHandlerMsgs.Keys.ToArray();
                List<ushort> removeList = new List<ushort>();
                for (int i = 0; i < ushorts.Length; i++)
                {
                    var opcode = ushorts[i];
                    bool handler = HandlerMessage(opcode, _notHandlerMsgs[opcode]);
                    if (handler)
                    {
                        removeList.Add(opcode);
                    }
                }

                for (int i = 0; i < removeList.Count; i++)
                {
                    _notHandlerMsgs.Remove(removeList[i]);
                }
                
            }
        }
        public void Login()
        {
            string token = HttpHelper.Token;
            this.PostJson($"{GameUrlConstValue.AssetAddress}api/login/token",
                $"{{\"app_id\":\"{HttpHelper.AppId}\",\"app_key\":\"{HttpHelper.AppKey}\",\"token\":\"{token}\"}}",
                result =>
                {
                    

                    if (string.IsNullOrEmpty(result))
                    {
                        LoginFail();
                    }
                    
                    var jsonDecode = MiniJson.JsonDecode(result) as Dictionary<string, object>;
                    var code = (long)jsonDecode["code"];

                    if (code != 0)
                    {
                        LogKit.E("Login Fail");
                        LoginFail();
                        return;
                    }

                    var data = jsonDecode["Data"] as Dictionary<string, object>;
                    var keep = data["keep"] as Dictionary<string, object>;
                    OnClientConnected = () =>
                    {
                        _webSocket.Send(GetHeartPacket());
                        Send(KeepOpCode.Op_C2G_Login, new C2G_Login()
                        {
                            Token = keep["token"] as string
                        });

                        _heartTime = HeartTime;
                    };
                    var address = keep["url"] as string;
                    var split = address.Split(":");
                    
#if !UNITY_EDITOR
                    clientUseWss = !DebugProfile.Instance.Debug;
#endif
                    
                    _uriBuilder = new UriBuilder
                    {
                        Scheme = GetClientScheme(),
                        Host = split[0],
                        Port = int.Parse(split[1])
                    };
                    Connect(_uriBuilder.Uri);
                });
        }


        protected void LoginFail()
        {
            LogKit.E("Login Fail");
            UIKit.OpenPanelAsync<NetworkErrorPanel>(panel =>
            {
                panel.Content.text = "Verification failed, please re-enter the game from the official website.";
                panel.Confirm.onClick.RemoveAllListeners();
                panel.Confirm.onClick.AddListener(() =>
                {
                    Application.OpenURL("https://minttest19.runx.xyz/#/mint");
                });
            });
            UIKit.EnableUIInput();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_webSocket != null)
            {
                _webSocket.Close();
                _webSocket = null;
            }
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus && _webSocket != null)
            {
                if (_webSocket.State == WebSocketState.Closed || _webSocket.State == WebSocketState.Closing)
                {
                    UIKit.OpenPanelAsync<WaitingPanel>(panel =>
                    {
                        UnRegisterHandler<G2C_Login>(KeepOpCode.Op_G2C_Login, LoginHelper.OnLogin);
                        panel.Message.gameObject.SetActive(false);
                        OnClientConnected += OnReConnected;
                        Connect(_uriBuilder.Uri);
                    });
                }
            }
        }


        private void OnReConnected()
        {
            UIKit.HidePanel<WaitingPanel>();
            OnClientConnected -= OnReConnected;
        }

        #endregion

        #region METHODS

        private WebSocket _webSocket;
        public void Connect(Uri url)
        {
            var address = url.Scheme + "://" + url.Host + "/websocket/";
            LogKit.I($"Connect : {address}");
            _webSocket = new WebSocket(address);

            _webSocket.OnClose += OnClientDisconnect;
            _webSocket.OnError += msg =>
            {
                _notHandlerMsgs.Clear();
                LogKit.E(msg);
            };
            
            _webSocket.OnOpen += () =>
            {
                LogKit.I($"Connect success");
                OnClientConnected.Invoke();
            };


            _webSocket.OnMessage += OnReceivedData;

            _webSocket.Connect();
            
        }

        
        protected void OnClientDisconnect(WebSocketCloseCode closeCode)
        {
            LogKit.E($"WebClient OnClientDisconnect : {closeCode}");
            OnClientDisconnected?.Invoke();
            _notHandlerMsgs.Clear();
        }

        private Dictionary<ushort, byte[]> _notHandlerMsgs = new Dictionary<ushort, byte[]>();

        protected bool HandlerMessage(ushort opcodeId, byte[] data)
        {
            if (handlers.ContainsKey(opcodeId))
            {
                LogKit.I($"HandlerMessage : {opcodeId}");
                handlers[opcodeId].Invoke(opcodeId, data);
                return true;
            }
            return false;
        }

        protected void OnReceivedData(byte[] data)
        {
            if (data.Length > 4)
            {
                int headLength = 4;
                byte[] headBytes = new byte[headLength];
                Array.Copy(data, 0, headBytes,0, headLength);
            
                byte[] bodyB = new byte[2];
                Array.Copy(headBytes, 0, bodyB, 0, 2);
                ushort bodyLen = BitConverter.ToUInt16(bodyB.Reverse().ToArray(), 0);
            
                byte[] opcodeB = new byte[2];
                Array.Copy(headBytes, 2, opcodeB, 0, 2);
                ushort opcode = BitConverter.ToUInt16(opcodeB.Reverse().ToArray(), 0);

                byte[] dataBodyB = new byte[bodyLen];
                Array.Copy(data, headLength, dataBodyB, 0, bodyLen);
                
                LogKit.I($"OnReceivedData : {(KeepOpCode)opcode}");
                if (_notHandlerMsgs.ContainsKey(opcode))
                {
                    _notHandlerMsgs[opcode] = dataBodyB;
                }
                else
                {
                    _notHandlerMsgs.Add(opcode, dataBodyB);
                }
            }
        }

        public void Send<T>(KeepOpCode opCode, T message)
        {
            LogKit.I($"Send : {opCode}");
            var data = ProtobufHelper.ToBytes(message);
            var head = GetHead((ushort)opCode, (ushort)data.Length);
            byte[] buffer = new byte[data.Length + head.Length];
            Array.Copy(head, 0, buffer, 0, head.Length);
            Array.Copy(data, 0, buffer, head.Length, data.Length);
            // _client.Send(buffer);
            _webSocket.Send(buffer);
        }

        public static void RegisterHandler<T>(KeepOpCode opCode, Action<T> handler)
        {
            ushort msgType = (ushort)opCode;
            if (handlers.ContainsKey(msgType))
            {
                Debug.LogWarning(
                    $"NetworkClient.RegisterHandler replacing handler for {typeof(T).FullName}, id={msgType}. If replacement is intentional, use ReplaceHandler instead to avoid this warning.");
                handlers[msgType] = WrapHandler(handler);
            }
            else
            {
                handlers[msgType] = WrapHandler(handler);
            }
        }

        public static void UnRegisterHandler<T>(KeepOpCode opCode, Action<T> handler)
        {
            ushort msgType = (ushort)opCode;
            if (handlers.ContainsKey(msgType))
            {
                handlers.Remove(msgType);
            }
        }

        private byte[] heartMsg;

        public byte[] GetHeartPacket()
        {
            if (heartMsg == null)
            {
                heartMsg = GetHead(0, 0);
            }
            
            return heartMsg;
        }

        public byte[] GetHead(ushort id, ushort length)
        {
            var lenB = BitConverter.GetBytes(length);
            Array.Reverse(lenB);
            var opcodeB = BitConverter.GetBytes(id);
            Array.Reverse(opcodeB);

            byte[] data = new byte[lenB.Length + opcodeB.Length];

            Array.Copy(lenB, 0, data, 0, lenB.Length);
            Array.Copy(opcodeB, 0, data, lenB.Length, opcodeB.Length);

            return data;
        }

        #endregion
    }
}