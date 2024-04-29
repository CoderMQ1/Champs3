// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using Mirror;
// using Mirror.SimpleWeb;
// using QFramework;
// using champs3.Core;
// using champs3.Hotfix;
// using champs3.Hotfix.GameLogic;
// using UnityCommon.Util;
// using UnityEngine;
//
// public class ServerInit : MonoBehaviour
// {
//
//     public bool _serverInitialized;
//     // Start is called before the first frame update
//     void Start()
//     {
//         StartCoroutine(GameServerManager.Instance.InitializeAsync(() =>
//         {
//             _serverInitialized = true;
//
//             ResourceManager.Instance.LoadSceneAsync("Scene_Level1", () =>
//             {
//                 ActionKit.Delay(0.5f, () =>
//                 {
//                     LogKit.I($"RoomManager - {RoomManager.Instance}");
//                     // LogKit.I($"GameServerManager - {GameServerManager.Instance}");
//                     // RoomManager.Instance.RoomInfo = ;
// #if !UNITY_EDITOR
//                     NetworkManager.singleton.networkAddress = $"{GameServerManager.Instance.ServerConfig.ServerAddress}";
//                     NetworkManager.singleton.GetComponent<SimpleWebTransport>().port = GameServerManager.Instance.ServerConfig.ServerPort; 
// #endif
//
//                     NetworkManager.singleton.StartServer();
//                 }).Start(ResourceManager.Instance);
//                 
//
//             });
//         }));
//     }
// }
