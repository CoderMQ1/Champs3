// using System;
// using System.Collections;
// using Mirror;
// using QFramework;
// using champs3.Hotfix.Events;
// using champs3.Hotfix.GameLogic;
// using champs3.Hotfix.Player;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.SceneManagement;
//
// namespace champs3.Hotfix.Mirror
// {
//     public class SHNetworkManager : NetworkManager
//     {
//         #region EDITOR EXPOSED FIELDS
//         public PlayerManager PlayerManager;
//
//         public bool AutoDestoryPlayer;
//         #endregion
//         #region FIELDS
//
//         #endregion
//         #region PROPERTIES
//         #endregion
//         #region METHODS
//         public override void OnStartClient()
//         {
//             base.OnStartClient();
//             LogKit.I($"Start Client address:{networkAddress}");
//             if (PlayerManager && playerPrefab == null)
//             {
//                 playerPrefab = PlayerManager.GetPlayerPrefab();
//                 NetworkClient.RegisterPrefab(playerPrefab, SpawnPlayerHandler, UnspawnPlayerHandler);
//                 NetworkClient.RegisterPrefab(PlayerManager.RobotPrefab);
//             }
//         }
//         
//         public override void OnStartServer()
//         {
//             base.OnStartServer();
//             if (PlayerManager && playerPrefab == null)
//             {
//                 playerPrefab = PlayerManager.GetPlayerPrefab();
//             }
//
//             StartCoroutine(CheckConnenction());
//         }
//
//
//         public IEnumerator CheckConnenction()
//         {
//
//             for (int i = 0; i < 20; i++)
//             {
//                 yield return new WaitForSeconds(1);
//             }
//
//             GameStateManager gameStateManager = GetComponentInChildren<GameStateManager>();
//             
//             
//             if (NetworkServer.connections.Count <= 0 && gameStateManager.IsWaitPlayer)
//             {
//                 LogKit.I("dont has player connected kill server");
//                 Application.Quit();
// #if UNITY_EDITOR
//                 EditorApplication.isPlaying = false;
// #endif
//             }
//
//             for (int i = 0; i < 20; i++)
//             {
//                 yield return new WaitForSeconds(1);
//             }
//
//             if (gameStateManager.IsWaitPlayer)
//             {
//                 LogKit.I("game has not start  kill server");
//                 Application.Quit();
// #if UNITY_EDITOR
//                 EditorApplication.isPlaying = false;
// #endif
//             }
//         }
//
//
//         private GameObject SpawnPlayerHandler(SpawnMessage msg)
//         {
//             GameObject playerObj = null;
//             if (mode == NetworkManagerMode.ClientOnly)
//             {
//                 playerObj = GameObject.Instantiate(playerPrefab, msg.position, msg.rotation);
//                 OnPlayerJoin(playerObj);
//             }
//             return playerObj;
//         }
//         
//         private void UnspawnPlayerHandler(GameObject spawned)
//         {
//             OnPlayerLeave(spawned);
//         }
//         
//         private void OnPlayerJoin(GameObject playerObj)
//         {
//             if (PlayerManager)
//             {
//                 // PlayerManager.OnPlayerJoin(playerObj);
//             }
//         }
//
//         private void OnPlayerLeave(GameObject playerObj)
//         {
//             if (PlayerManager)
//             {
//                 // PlayerManager.OnPLayerLeave(playerObj);
//             }
//         }
//
//         public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
//         {
//
//             if (mode == NetworkManagerMode.Host)
//             {
//                 TypeEventSystem.Global.Send(new GameEvents.OnClientLoadedScene()
//                 {
//                     SceneName = newSceneName
//                 });
//             }
//             if (mode == NetworkManagerMode.ClientOnly)
//             {
//                 LogKit.I($"OnClientChangeScene {newSceneName}");
//                 Action OnCompleted = () =>
//                 {
//                     OnClientLoadSceneCompleted(newSceneName, sceneOperation);
//                 };
//                 switch (sceneOperation)
//                 {
//                     case SceneOperation.Normal:
//                         ResourceManager.Instance.LoadSceneAsync(newSceneName, OnCompleted, LoadSceneMode.Single);
//                         break;
//                     case SceneOperation.LoadAdditive:
//                         ResourceManager.Instance.LoadSceneAsync(newSceneName, OnCompleted, LoadSceneMode.Additive);
//                         break;
//                     case SceneOperation.UnloadAdditive:
//                         ResourceManager.Instance.UnLoadSubScene(newSceneName, OnCompleted);
//                         break;
//                 }
//             }
//
//         }
//
//         private void OnClientLoadSceneCompleted(string newSceneName, SceneOperation sceneOperation)
//         {
//             LogKit.I($"OnClientLoadSceneCompleted : {newSceneName}");
//             FinishLoadScene();
//             TypeEventSystem.Global.Send(new GameEvents.OnClientLoadedScene()
//             {
//                 SceneName = newSceneName
//             });
//         }
//
//         public override void OnServerAddPlayer(NetworkConnectionToClient conn)
//         {
//             base.OnServerAddPlayer(conn);
//             OnPlayerJoin(conn.identity.gameObject);
//         }
//
//         public override void OnServerDisconnect(NetworkConnectionToClient conn)
//         {
//             OnPlayerLeave(conn.identity.gameObject);
//             if (AutoDestoryPlayer)
//             {
//                 base.OnServerDisconnect(conn);
//             }
//         }
//
//         public override void ServerChangeScene(string newSceneName)
//         {
//             if (string.IsNullOrWhiteSpace(newSceneName))
//             {
//                 Debug.LogError("ServerChangeScene empty scene name");
//                 return;
//             }
//
//             if (NetworkServer.isLoadingScene && newSceneName == networkSceneName)
//             {
//                 Debug.LogError($"Scene change is already in progress for {newSceneName}");
//                 return;
//             }
//
//             // Debug.Log($"ServerChangeScene {newSceneName}");
//             NetworkServer.SetAllClientsNotReady();
//             networkSceneName = newSceneName;
//
//             // Let server prepare for scene change
//             OnServerChangeScene(newSceneName);
//
//             // set server flag to stop processing messages while changing scenes
//             // it will be re-enabled in FinishLoadScene.
//             NetworkServer.isLoadingScene = true;
//
//
//             ResourceManager.Instance.LoadSceneAsync(newSceneName, () =>
//             {
//                 OnServerLoadedScene();
//             });
//             
//             // loadingSceneAsync = SceneManager.LoadSceneAsync(newSceneName);
//
//             // ServerChangeScene can be called when stopping the server
//             // when this happens the server is not active so does not need to tell clients about the change
//             if (NetworkServer.active)
//             {
//                 // notify all clients about the new scene
//                 NetworkServer.SendToAll(new SceneMessage
//                 {
//                     sceneName = newSceneName,
//                     customHandling = true
//                 });
//             }
//
//             // startPositionIndex = 0;
//             // startPositions.Clear();
//         }
//
//
//         private void OnServerLoadedScene()
//         {
//             try
//             {
//                 FinishLoadScene();
//             }
//             finally
//             {
//
//             }
//         }
//
//
//         #region EVENT FUNCTION
//         public override void Start()
//         {
//             base.Start();
//             MessageHelper.Initialize();
//         }
//         #endregion
//         #endregion
//     }
// }