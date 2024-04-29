// // 
// // 2023/12/04
//
// using Mirror;
// using UnityEngine;
//
// namespace champs3.Hotfix.Mirror
// {
//     public class SHNetworkAuthenticator : NetworkAuthenticator
//     {
//         #region EDITOR EXPOSED FIELDS
//
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
//         #endregion
//
//         #region METHODS
//
//         public override void OnServerAuthenticate(NetworkConnectionToClient conn)
//         {
//             base.OnServerAuthenticate(conn);conn.isAuthenticated = true;
//
//             // proceed with the login handshake by calling OnServerConnect
//             if (NetworkManager.singleton.onlineScene != "" && NetworkManager.networkSceneName != NetworkManager.singleton.offlineScene)
//             {
//                 SceneMessage msg = new SceneMessage()
//                 {
//                     sceneName = NetworkManager.singleton.onlineScene,
//                     customHandling = true
//                 };
//                 conn.Send(msg);
//             }
//             NetworkManager.singleton.OnServerConnect(conn);
//         }
//
//
//         public override void OnClientAuthenticate()
//         {
//             base.OnClientAuthenticate();
//             NetworkClient.connection.isAuthenticated = true;
//             bool needLoadScene;
//             // Set flag to wait for scene change?
//             if (string.IsNullOrWhiteSpace(NetworkManager.singleton.onlineScene) || NetworkManager.singleton.onlineScene == NetworkManager.singleton.offlineScene || Utils.IsSceneActive(NetworkManager.singleton.onlineScene))
//             {
//                 needLoadScene = false;
//             }
//             else
//             {
//                 // Scene message expected from server.
//                 needLoadScene = true;
//                 // NetworkManager.singleton.clientReadyConnection = NetworkClient.connection;
//             }
//
//             if (!needLoadScene)
//             {
//                 if (!NetworkClient.ready)
//                     NetworkClient.Ready();
//
//                 if ( NetworkManager.singleton.autoCreatePlayer)
//                     NetworkClient.AddPlayer();
//             }
//             // Call virtual method regardless of whether a scene change is expected or not.
//
//             
//         }
//
//         #endregion
//
//         
//     }
//
// }