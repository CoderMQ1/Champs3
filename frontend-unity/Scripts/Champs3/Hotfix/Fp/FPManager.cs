// // 
// // 2023/11/30
//
// using System;
// using Cinemachine;
// using DG.Tweening;
// using Mirror;
// using QFramework;
// using champs3.Hotfix.UI;
// using UnityEngine;
//
// namespace champs3.Hotfix
// {
//     public class FPManager : MonoBehaviour
//     {
//         public static FPManager Instance;
//         #region EDITOR EXPOSED FIELDS
//
//         public GameObject Door;
//         public CinemachineVirtualCamera Camera;
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
//         #region METHODS
//         
//         #region EVENT FUNCTION
//
//         private void Awake()
//         {
//             Instance = this;
//
//         }
//
//         private void OnDestroy()
//         {
//             Instance = null;
//         }
//
//         #endregion
//
//
//         public void EnterGame()
//         {
//
//             var gameStartPanel = UIKit.GetPanel<GameStartPanel>();
//
//             var animation = Door.GetComponent<Animation>();
//             animation.Play("OpenDoor");
//             CinemachineTrackedDolly cinemachineTrackedDolly = Camera.GetCinemachineComponent<CinemachineTrackedDolly>();
//             
//             Debug.Log(cinemachineTrackedDolly);
//             var sequence = DOTween.Sequence();
//             sequence.AppendInterval(2);
//             sequence.Append(cinemachineTrackedDolly.DODolly(cinemachineTrackedDolly.m_Path.MinPos, cinemachineTrackedDolly.m_Path.MaxPos, 2f));
//
//             if (gameStartPanel)
//             {
//                 gameStartPanel.BlackMask.gameObject.SetActive(true);
//                 sequence.Append(gameStartPanel.BlackMask.DOFade(1, 0.4f));
//             }
//
//             sequence.onComplete = () =>
//             {
//                 EnterMainScene();
//             };
//             sequence.Play();
//         }
//         
//         
//         
//
//         #endregion
//
//
//         public void StartGame()
//         {
//             // UIKit.OpenPanelAsync<LoadingPanel>(null, UILevel.PopUI);
//             UIKit.ClosePanel<GameStartPanel>();
//             ResourceManager.Instance.LoadSceneAsync("Scene_Level1Start", () =>
//             {
//                 NetworkManager.singleton.StartHost();
//             });
//             
//         }
//
//         public void EnterMainScene()
//         {
//             UIKit.OpenPanelAsync<LoadingPanel>(null, UILevel.PopUI);
//             UIKit.ClosePanel<GameStartPanel>();
//             ResourceManager.Instance.LoadSceneAsync("Scene_Main", () =>
//             {
//                 
//                 UIKit.OpenPanelAsync<MainPanel>(mainPanel =>
//                 {
//                     UIKit.HidePanel<LoadingPanel>();
//                 }, UILevel.PopUI);
//             });
//         }
//
//     }
//
// }