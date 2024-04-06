// // 
// // 2023/12/13
//
// using System;
// using System.Collections.Generic;
// using System.ComponentModel;
// using System.IO;
// using System.Text;
// using Mirror;
// using Mirror.SimpleWeb;
// using QFramework;
// using SquareHero.Hotfix.Generate;
// using SquareHero.Hotfix.UI;
// using UnityCommon;
// using UnityCommon.Util;
// using UnityEngine;
//
// namespace SquareHero.Hotfix.Net
// {
//     public class SHNetManager : MonoSingleton<SHNetManager>
//     {
//         #region EDITOR EXPOSED FIELDS
//
//         [Tooltip("disables nagle algorithm. lowers CPU% and latency but increases bandwidth")]
//         public bool noDelay = true;
//
//         [Tooltip(
//             "Send would stall forever if the network is cut off during a send, so we need a timeout (in milliseconds)")]
//         public int sendTimeout = 5000;
//
//         [Tooltip("How long without a message before disconnecting (in milliseconds)")]
//         public int receiveTimeout = 20000;
//
//         public float HeartTime = 0.5f;
//
//         public bool Connected
//         {
//             get { return _client != null; }
//         }
//
//         private DateTime _lastHeartTime;
//         
//         // client //////////////////////////////////////////////////////////////
//         /// <summary>Called by Transport when the client connected to the server.</summary>
//         public Action OnClientConnected;
//
//         /// <summary>Called by Transport when the client received a message from the server.</summary>
//         public Action<ArraySegment<byte>, int> OnClientDataReceived;
//
//         /// <summary>Called by Transport when the client sent a message to the server.</summary>
//         // Transports are responsible for calling it because:
//         // - groups it together with OnReceived responsibility
//         // - allows transports to decide if anything was sent or not
//         // - allows transports to decide the actual used channel (i.e. tcp always sending reliable)
//         public Action<ArraySegment<byte>, int> OnClientDataSent;
//
//         [Header("Ssl Settings")]
//         [Tooltip(
//             "Sets connect scheme to wss. Useful when client needs to connect using wss when TLS is outside of transport.\nNOTE: if sslEnabled is true clientUseWss is also true")]
//         public bool clientUseWss;
//
//         [Tooltip(
//             "Requires wss connections on server, only to be used with SSL cert.json, never with reverse proxy.\nNOTE: if sslEnabled is true clientUseWss is also true")]
//         public bool sslEnabled;
//
//         #endregion
//
//         #region FIELDS
//
//         public const string NormalScheme = "ws";
//         public const string SecureScheme = "wss";
//         public Action OnClientDisconnected;
//         string GetClientScheme() => (sslEnabled || clientUseWss) ? SecureScheme : NormalScheme;
//         TcpConfig TcpConfig => new TcpConfig(noDelay, sendTimeout, receiveTimeout);
//         private ShWebClient _client;
//         private float _heartTime;
//
//         public delegate void MessageDelegate(ushort opcodeId, byte[] data);
//
//         // message handlers by messageId
//         internal static readonly Dictionary<ushort, MessageDelegate> handlers =
//             new Dictionary<ushort, MessageDelegate>();
//
//
//         static MessageDelegate WrapHandler<T>(Action<T> handler)
//             => (opcodeId, data) =>
//             {
//                 var msg = (T)ProtobufHelper.FromBytes(typeof(T), data, 0, data.Length);
//                 LogKit.I($"Receive msg : {msg.GetType()}:{msg.ToJson()}");
//                 handler.Invoke(msg);
//             };
//
//         #endregion
//
//
//         #region PROPERTIES
//
//
//
//         #endregion
//
//         #region EVENT FUNCTION
//
//         private void Awake()
//         {
//             mInstance = this;
//             DontDestroyOnLoad(this);
//             HeartTime = 2f;
//         }
//
//         private void Update()
//         {
//             if (_client != null)
//             {
//                 _client.ProcessMessageQueue(this);
//                 if (_client != null && _client.ConnectionState == ClientState.Connected)
//                 {
//                     var time = DateTime.Now;
//                     var timeSpan = time.Subtract(_lastHeartTime);
//                     
//                     if (timeSpan.Seconds >= HeartTime)
//                     {
//                         _client.Send(GetHeartPacket());
//                         _lastHeartTime = time;
//                         LogKit.I($"Heart , time: {time.ToString("HH:mm:ss zz")}");
//                     }
//                 }
//             }
//         }
//         public void Login()
//         {
//             string token = HttpHelper.Token;
//             this.PostJson($"{GameUrlConstValue.AssetAddress}api/login/token",
//                 $"{{\"app_id\":\"{HttpHelper.AppId}\",\"app_key\":\"{HttpHelper.AppKey}\",\"token\":\"{token}\"}}",
//                 result =>
//                 {
//                     
//
//                     if (string.IsNullOrEmpty(result))
//                     {
//                         LoginFail();
//                     }
//                     
//                     var jsonDecode = MiniJson.JsonDecode(result) as Dictionary<string, object>;
//                     var code = (long)jsonDecode["code"];
//
//                     if (code != 0)
//                     {
//                         LogKit.E("Login Fail");
//                         LoginFail();
//                         return;
//                     }
//
//                     var data = jsonDecode["Data"] as Dictionary<string, object>;
//                     var keep = data["keep"] as Dictionary<string, object>;
//                     OnClientConnected = () =>
//                     {
//                         _client.Send(GetHeartPacket());
//                         Send(KeepOpCode.Op_C2G_Login, new C2G_Login()
//                         {
//                             Token = keep["token"] as string
//                         });
//
//                         _heartTime = HeartTime;
//                     };
//                     var address = keep["url"] as string;
//                     var split = address.Split(":");
// #if !UNITY_EDITOR
//                     clientUseWss = !ConstValue.Debug;
// #endif
//                     
//                     UriBuilder builder = new UriBuilder
//                     {
//                         Scheme = GetClientScheme(),
//                         Host = split[0],
//                         Port = int.Parse(split[1])
//                     };
//                     Connect(builder.Uri);
//                 });
//         }
//
//
//         protected void LoginFail()
//         {
//             LogKit.E("Login Fail");
//             UIKit.OpenPanelAsync<NetworkErrorPanel>(panel =>
//             {
//                 panel.Content.text = "Verification failed, please re-enter the game from the official website.";
//                 panel.Confirm.onClick.RemoveAllListeners();
//                 panel.Confirm.onClick.AddListener(() =>
//                 {
//                     Application.OpenURL("http://baidu.ddnnk.com/");
//                 });
//             });
//             UIKit.EnableUIInput();
//         }
//
//         protected override void OnDestroy()
//         {
//             base.OnDestroy();
//             if (_client != null)
//             {
//                 _client.Disconnect();
//                 _client = null;
//             }
//         }
//         
//
//         #endregion
//
//         #region METHODS
//
//         public void Connect(Uri url)
//         {
//             try
//             {
//                 // _client = SimpleWebClient.Create(16384, 1000, TcpConfig);
//                 _client = ShWebClient.Create(16348, 1000, TcpConfig);
//                 if (_client == null)
//                 {
//                     LogKit.E("WebClient Connect Fail");
//                     return;
//                 }
//
//
//                 _client.onConnect += OnClientConnected;
//
//                 _client.onDisconnect += OnClientDisconnect;
//
//                 _client.onData += OnReceivedData;
//
//                 _client.onError += (Exception e) =>
//                 {
//                     LogKit.E($"Net Error : {e.Message}");
//                     ClientDisconnect();
//                 };
//
//                 _client.Connect(url);
//             }
//             catch (Exception e)
//             {
//                 LogKit.E($"Connent {url} error : {e.Message}");
//                 throw;
//             }
//         }
//
//
//         public void ClientDisconnect()
//         {
//             // don't set client null here of messages wont be processed
//             _client?.Disconnect();
//         }
//
//         protected void OnClientDisconnect()
//         {
//             LogKit.E("WebClient OnClientDisconnect");
//             OnClientDisconnected?.Invoke();
//             _client = null;
//         }
//
//         protected void OnReceivedData(ushort opcodeId, ArraySegment<byte> data)
//         {
//             LogKit.I($"OnReceivedData : {(KeepOpCode)opcodeId}");
//             if (handlers.ContainsKey(opcodeId))
//             {
//                 handlers[opcodeId].Invoke(opcodeId, data.ToArray());
//             }
//         }
//
//         public void Send<T>(KeepOpCode opCode, T message)
//         {
//             LogKit.I($"Send : {opCode}");
//             var data = ProtobufHelper.ToBytes(message);
//             var head = GetHead((ushort)opCode, (ushort)data.Length);
//             byte[] buffer = new byte[data.Length + head.Length];
//             Array.Copy(head, 0, buffer, 0, head.Length);
//             Array.Copy(data, 0, buffer, head.Length, data.Length);
//             _client.Send(buffer);
//         }
//
//         public static void RegisterHandler<T>(KeepOpCode opCode, Action<T> handler)
//         {
//             ushort msgType = (ushort)opCode;
//             if (handlers.ContainsKey(msgType))
//             {
//                 Debug.LogWarning(
//                     $"NetworkClient.RegisterHandler replacing handler for {typeof(T).FullName}, id={msgType}. If replacement is intentional, use ReplaceHandler instead to avoid this warning.");
//                 handlers[msgType] = WrapHandler(handler);
//             }
//             else
//             {
//                 handlers[msgType] = WrapHandler(handler);
//             }
//         }
//
//         public static void UnRegisterHandler<T>(KeepOpCode opCode, Action<T> handler)
//         {
//             ushort msgType = (ushort)opCode;
//             if (handlers.ContainsKey(msgType))
//             {
//                 handlers.Remove(msgType);
//             }
//         }
//
//         private ArraySegment<byte> heartMsg;
//
//         public ArraySegment<byte> GetHeartPacket()
//         {
//             if (heartMsg == null)
//             {
//                 heartMsg = GetHead(0, 0);
//             }
//
//             return heartMsg;
//         }
//
//         public byte[] GetHead(ushort id, ushort length)
//         {
//             var lenB = BitConverter.GetBytes(length);
//             Array.Reverse(lenB);
//             var opcodeB = BitConverter.GetBytes(id);
//             Array.Reverse(opcodeB);
//
//             byte[] data = new byte[lenB.Length + opcodeB.Length];
//
//             Array.Copy(lenB, 0, data, 0, lenB.Length);
//             Array.Copy(opcodeB, 0, data, lenB.Length, opcodeB.Length);
//
//             return data;
//         }
//
//         #endregion
//     }
// }