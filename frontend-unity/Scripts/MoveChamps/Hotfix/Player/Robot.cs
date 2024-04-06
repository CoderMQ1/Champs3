// // 
// // 2023/12/07
//
// using System;
// using Mirror;
// using QFramework;
// using SquareHero.Hotfix.Events;
// using SquareHero.Hotfix.Model;
// using UnityEngine;
//
// namespace SquareHero.Hotfix.Player
// {
//     public class Robot : SHPlayerController
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
//         
//         #endregion
//
//         #region METHODS
//         
//         #endregion
//
//
//
//         public void InitializedRobot()
//         {
//             LogKit.I("Robot Init");
//
//             
//             var startPosition = NetworkManager.startPositions[Data.Track];
//
//             transform.position = startPosition.position;
//             transform.rotation = startPosition.rotation;
//             // RpcSyncData(Data);
//         }
//     }
//
// }