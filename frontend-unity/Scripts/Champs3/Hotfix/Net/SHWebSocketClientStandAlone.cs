// // 
// // 2023/12/13
//
// using System;
// using System.Collections.Concurrent;
// using System.IO;
// using System.Linq;
// using System.Net.Security;
// using System.Net.Sockets;
// using System.Security.Cryptography;
// using System.Security.Cryptography.X509Certificates;
// using System.Text;
// using System.Threading;
// using System.Threading.Tasks;
// using MetaMask.NativeWebSocket;
// using Mirror.SimpleWeb;
// using QFramework;
// using UnityEngine;
// using EventType = Mirror.SimpleWeb.EventType;
//
// namespace champs3.Hotfix.Net
// {
//     public class SHWebSocketClientStandAlone : ShWebClient
//     {
//         // readonly ClientSslHelper sslHelper;
//         // readonly ClientHandshake handshake;
//         readonly TcpConfig tcpConfig;
//
//         private TcpClient _client;
//         public ConcurrentQueue<ArrayBuffer> sendQueue = new ConcurrentQueue<ArrayBuffer>();
//         private Thread _receiveThread;
//
//         private WebSocket _webSocket;
//         // Connection conn;
//         public SHWebSocketClientStandAlone(int maxMessageSize, int maxMessagesPerTick, TcpConfig tcpConfig) : base(maxMessageSize, maxMessagesPerTick)
//         {
// #if UNITY_WEBGL && !UNITY_EDITOR
//             throw new NotSupportedException();
// #else
//             // sslHelper = new ClientSslHelper();
//             // handshake = new ClientHandshake();
//             this.tcpConfig = tcpConfig;
// #endif
//         }
//
//         public override void Connect(Uri serverAddress)
//         {
//
//             
//
//             _client = new TcpClient();
//             LogKit.I($"Conncet : {serverAddress.Host}:{serverAddress.Port}");
//             _client.Connect(serverAddress.Host, serverAddress.Port);
//             state = ClientState.Connecting;
//             // tcpConfig.ApplyTo(_client);
//             
//             var task = Task.Run(() =>
//             {
//                 ConnectAndReceiveLoop(serverAddress);
//             });
//
//             // _receiveThread = new Thread(() => ConnectAndReceiveLoop(serverAddress));
//             // _receiveThread.IsBackground = true;
//             // _receiveThread.Start();
//         }
//         
//         
//
//         void ConnectAndReceiveLoop(Uri serverAddress)
//         {
//             try
//             {
//                 while (_client != null)
//                 {
//                     if (state == ClientState.Connecting && _client.Connected)
//                     {
//                         
//
//                         var ssl = TryCreateStream(_client, serverAddress);
//
//                         if (ssl)
//                         {
//                             Debug.Log($"connected : {_client.Connected}");
//                             // TryHandshake(_client, serverAddress);
//                             state = ClientState.Connected;
//                             receiveQueue.Enqueue(new Message(EventType.Connected));
//                         }
//                         
//                     }
//                     var stream = _client.GetStream();
//                     byte[] headB = new byte[4];
//                     int bytesRead = stream.Read(headB, 0, headB.Length);
//                     if (bytesRead == 4) 
//                     {
//                         byte[] bodyB = new byte[2];
//                         Array.Copy(headB, 0, bodyB, 0, 2);
//                         ushort bodyLen = BitConverter.ToUInt16(bodyB.Reverse().ToArray(), 0);
//                         byte[] opcodeB = new byte[2];
//                         Array.Copy(headB, 2, opcodeB, 0, 2);
//                         ushort opcode = BitConverter.ToUInt16(opcodeB.Reverse().ToArray(), 0);
//                     
//                         if (bodyLen > 0)
//                         {
//                             while (_client.Available < bodyLen)
//                             {
//                                 //Debug.LogError($"{head.opcode}收到字节数不够..{stream.Length}/{head.bodyLen}");
//                                 Task.Delay(10);
//                             }
//                         }
//                     
//                         byte[] msg = new byte[bodyLen];
//                     
//                         stream.Read(msg, 0, msg.Length);
//                     
//                         ArrayBuffer arrayBuffer = bufferPool.Take(msg.Length);
//                         
//                         arrayBuffer.CopyFrom(msg, 0, msg.Length);
//                         
//                         receiveQueue.Enqueue(new Message(opcode, arrayBuffer));
//                     }
//                     
//                 }
//                 
//                 LogKit.E($"client connecter : {_client.Connected}");
//             }
//             catch (Exception e)
//             {
//                 Disconnect();
//                 LogKit.E(e.Message);
//                 LogKit.E( e.StackTrace);
//             }
//         }
//         
//         internal bool TryCreateStream(TcpClient client, Uri uri)
//         {
//             NetworkStream stream = client.GetStream();
//             if (uri.Scheme != "wss")
//             {
//                 return true;
//             }
//
//             try
//             {
//                 Stream sslStream = CreateStream(stream, uri);
//                 return true;
//             }
//             catch (Exception e)
//             {
//                 LogKit.E($"[SimpleWebTransport] Create SSLStream Failed: {e}", false);
//                 return false;
//             }
//         }
//         
//         Stream CreateStream(NetworkStream stream, Uri uri)
//         {
//             SslStream sslStream = new SslStream(stream, true, ValidateServerCertificate);
//             sslStream.AuthenticateAsClient(uri.Host);
//             return sslStream;
//         }
//         
//         public bool TryHandshake(TcpClient conn, Uri uri)
//         {
//             try
//             {
//                 Stream stream = conn.GetStream();
//
//                 byte[] keyBuffer = new byte[16];
//                 using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
//                     rng.GetBytes(keyBuffer);
//
//                 string key = Convert.ToBase64String(keyBuffer);
//                 // string keySum = key + Constants.HandshakeGUID;
//                 // byte[] keySumBytes = Encoding.ASCII.GetBytes(keySum);
//                 // Debug.Log($"[SimpleWebTransport] Handshake Hashing {Encoding.ASCII.GetString(keySumBytes)}");
//
//                 // SHA-1 is the websocket standard:
//                 // https://www.rfc-editor.org/rfc/rfc6455
//                 // we should follow the standard, even though SHA1 is considered weak:
//                 // https://stackoverflow.com/questions/38038841/why-is-sha-1-considered-insecure
//                 // byte[] keySumHash = SHA1.Create().ComputeHash(keySumBytes);
//
//                 // string expectedResponse = Convert.ToBase64String(keySumHash);
//                 string handshake =
//                     $"GET {uri.PathAndQuery} HTTP/1.1\r\n" +
//                     $"Host: {uri.Host}:{uri.Port}\r\n" +
//                     $"Upgrade: websocket\r\n" +
//                     $"Connection: Upgrade\r\n" +
//                     $"Sec-WebSocket-Key: {key}\r\n" +
//                     $"Sec-WebSocket-Version: 13\r\n" +
//                     "\r\n";
//                 byte[] encoded = Encoding.ASCII.GetBytes(handshake);
//                 stream.Write(encoded, 0, encoded.Length);
//
//                 byte[] responseBuffer = new byte[1000];
//
//                 int? lengthOrNull = ReadHelper.SafeReadTillMatch(stream, responseBuffer, 0, responseBuffer.Length, new byte[4] { (byte)'\r', (byte)'\n', (byte)'\r', (byte)'\n' });
//
//                 if (!lengthOrNull.HasValue)
//                 {
//                     Debug.LogError("[SimpleWebTransport] Connection closed before handshake");
//                     return false;
//                 }
//
//                 string responseString = Encoding.ASCII.GetString(responseBuffer, 0, lengthOrNull.Value);
//                 Debug.LogError($"[SimpleWebTransport] Handshake Response {responseString}");
//
//                 string acceptHeader = "Sec-WebSocket-Accept: ";
//                 int startIndex = responseString.IndexOf(acceptHeader, StringComparison.InvariantCultureIgnoreCase);
//
//                 if (startIndex < 0)
//                 {
//                     Debug.LogError($"[SimpleWebTransport] Unexpected Handshake Response {responseString}");
//                     return false;
//                 }
//
//                 startIndex += acceptHeader.Length;
//                 int endIndex = responseString.IndexOf("\r\n", startIndex);
//                 string responseKey = responseString.Substring(startIndex, endIndex - startIndex);
//
//                 // if (responseKey != expectedResponse)
//                 // {
//                     Debug.LogError($"[SimpleWebTransport] Response key incorrect\nResponse:{responseKey}\nExpected");
//                 //     return false;
//                 // }
//
//                 return true;
//             }
//             catch (Exception e)
//             {
//                 Debug.LogError(e);
//                 return false;
//             }
//         }
//         
//         static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
//         {
//             // Do not allow this client to communicate with unauthenticated servers.
//
//             // only accept if no errors
//             // return true;
//             return sslPolicyErrors == SslPolicyErrors.None;
//         }
//
//         public override void Disconnect()
//         {
//             _client.Dispose();
//             _client = null;
//             receiveQueue.Enqueue(new Message(EventType.Disconnected));
//         }
//
//         public override void Send(ArraySegment<byte> segment)
//         {
//             if (_client != null)
//             {
//                 var stream = _client.GetStream();
//                 var array = segment.ToArray();
//                 stream.Write(array, 0, segment.Count);
//             }
//
//         }
//     }
//
// }