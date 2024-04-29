// // 
// // 2023/12/14
//
// using System;
// using System.Collections.Concurrent;
// using System.Linq;
// using Mirror.SimpleWeb;
// using UnityEngine;
// using EventType = Mirror.SimpleWeb.EventType;
//
// namespace champs3.Hotfix.Net
// {
//
//         public enum ClientState
//         {
//             NotConnected = 0,
//             Connecting = 1,
//             Connected = 2,
//             Disconnecting = 3,
//         }
//         public struct Message
//         {
//             public readonly ushort opcodeId;
//             public readonly EventType type;
//             public readonly ArrayBuffer data;
//             public readonly Exception exception;
//
//             public Message(EventType type) : this()
//             {
//                 this.type = type;
//             }
//
//             public Message(ArrayBuffer data) : this()
//             {
//                 type = EventType.Data;
//                 this.data = data;
//             }
//
//             public Message(Exception exception) : this()
//             {
//                 type = EventType.Error;
//                 this.exception = exception;
//             }
//
//             public Message(ushort opcodeId, EventType type) : this()
//             {
//                 this.opcodeId = opcodeId;
//                 this.type = type;
//             }
//
//             public Message(ushort opcodeId, ArrayBuffer data) : this()
//             {
//                 this.opcodeId = opcodeId;
//                 type = EventType.Data;
//                 this.data = data;
//             }
//
//             public Message(ushort opcodeId, Exception exception) : this()
//             {
//                 this.opcodeId = opcodeId;
//                 type = EventType.Error;
//                 this.exception = exception;
//             }
//         }
//         /// <summary>
//         /// Client used to control websockets
//         /// <para>Base class used by WebSocketClientWebGl and WebSocketClientStandAlone</para>
//         /// </summary>
//         public abstract class ShWebClient
//         {
//             public static ShWebClient Create(int maxMessageSize, int maxMessagesPerTick,
//                 TcpConfig tcpConfig)
//             {
// #if UNITY_WEBGL && !UNITY_EDITOR
//             return new SHWebSocketClientWebGl(maxMessageSize, maxMessagesPerTick);
// #else
//             return new SHWebSocketClientStandAlone(maxMessageSize, maxMessagesPerTick, tcpConfig);
// #endif
//             }
//
//             readonly int maxMessagesPerTick;
//             protected readonly int maxMessageSize;
//             public readonly ConcurrentQueue<Message> receiveQueue = new ConcurrentQueue<Message>();
//             protected readonly BufferPool bufferPool;
//
//             protected ClientState state;
//
//             protected ShWebClient(int maxMessageSize, int maxMessagesPerTick)
//             {
//                 this.maxMessageSize = maxMessageSize;
//                 this.maxMessagesPerTick = maxMessagesPerTick;
//                 bufferPool = new BufferPool(5, 20, maxMessageSize);
//             }
//
//             public ClientState ConnectionState => state;
//
//             public event Action onConnect;
//             public event Action onDisconnect;
//             public event Action<ushort, ArraySegment<byte>> onData;
//             public event Action<Exception> onError;
//
//             /// <summary>
//             /// Processes all new messages
//             /// </summary>
//             public void ProcessMessageQueue()
//             {
//                 ProcessMessageQueue(null);
//             }
//
//             /// <summary>
//             /// Processes all messages while <paramref name="behaviour"/> is enabled
//             /// </summary>
//             /// <param name="behaviour"></param>
//             public void ProcessMessageQueue(MonoBehaviour behaviour)
//             {
//                 int processedCount = 0;
//                 bool skipEnabled = behaviour == null;
//                 // check enabled every time in case behaviour was disabled after data
//                 while (
//                     (skipEnabled || behaviour.enabled) &&
//                     processedCount < maxMessagesPerTick &&
//                     // Dequeue last
//                     receiveQueue.TryDequeue(out Message next)
//                 )
//                 {
//                     processedCount++;
//
//                     switch (next.type)
//                     {
//                         case EventType.Connected:
//                             onConnect?.Invoke();
//                             break;
//                         case EventType.Data:
//                             onData?.Invoke(next.opcodeId, next.data.ToSegment());
//                             next.data.Release();
//                             break;
//                         case EventType.Disconnected:
//                             onDisconnect?.Invoke();
//                             break;
//                         case EventType.Error:
//                             onError?.Invoke(next.exception);
//                             break;
//                     }
//                 }
//
//                 if (receiveQueue.Count > 0)
//                     Debug.LogWarning($"SimpleWebClient ProcessMessageQueue has {receiveQueue.Count} remaining.");
//             }
//
//             public abstract void Connect(Uri serverAddress);
//             public abstract void Disconnect();
//             public abstract void Send(ArraySegment<byte> segment);
//         }
//
//
//         /// <summary>
//         /// 头定义
//         /// </summary>
//         public class Head
//         {
//             public ushort opcode { get; }
//             public ushort bodyLen { get; }
//
//             public Head(byte[] b)
//             {
//                 byte[] bodyB = new byte[2];
//                 Array.Copy(b, 0, bodyB, 0, 2);
//                 bodyLen = BitConverter.ToUInt16(bodyB.Reverse().ToArray(), 0);
//                 byte[] opcodeB = new byte[2];
//                 Array.Copy(b, 2, opcodeB, 0, 2);
//                 opcode = BitConverter.ToUInt16(opcodeB.Reverse().ToArray(), 0);
//             }
//         }
//
// }