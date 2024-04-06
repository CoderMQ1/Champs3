// 
// 2023/12/05

using System;
using Cinemachine;
using SquareHero.Core;
using SquareHero.Hotfix.Player;
using UnityEngine;

namespace SquareHero.Hotfix
{
    public class CameraTrigger : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        #endregion

        #region FIELDS

        #endregion

        #region PROPERTIES

        #endregion

        #region EVENT FUNCTION

        private void Awake()
        {



        }

        private void OnTriggerEnter(Collider other)
        {
            // this.Log($"Client OnTriggerEnter {other.transform.parent.name}.{other.name}-{other.tag}");
            if (other.CompareTag("Player"))
            {
                var playerController = other.gameObject.GetComponent<SHPlayerController>();

                if (playerController && !playerController.Data.IsRobot)
                {
                    var child = transform.GetChild(0);
                    child.gameObject.SetActive(true);
                    if (!transform.parent.name.Contains("Start"))
                    {
                        if (GameStart.isCreated && GameStart.Instance.IsMobilePlatform())
                        {
                            var virtualCamera = child.GetComponent<CinemachineVirtualCamera>();
                            virtualCamera.m_Lens.FieldOfView = 60;
                            var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
                            transposer.m_FollowOffset = new Vector3(6, 6.76f, 4.5f);
                        }
                    }

                    var cameraTargets = other.gameObject.GetComponentsInChildren<CameraTarget>();
                    for (int i = 0; i < cameraTargets.Length; i++)
                    {
                        cameraTargets[i].transform.parent = null;
                        cameraTargets[i].gameObject.SetActive(false);
                    }
                
                    var find = transform.Find("CameraTarget");
                    if (find)
                    {
                        find.parent = other.gameObject.transform;
                        find.localPosition = Vector3.zero;
                    }
                }
            }
        }
        
        // private void OnTriggerStay(Collider other)
        // {
        //     this.Log($"Client OnTriggerEnter {other.transform.parent.name}.{other.name}-{other.tag}");
        //     if (other.CompareTag("Player"))
        //     {
        //         var playerController = other.gameObject.GetComponent<SHPlayerController>();
        //         var child = transform.GetChild(0);
        //         if (playerController.isLocalPlayer)
        //         {
        //             
        //             var find = transform.Find("CameraTarget");
        //             find.parent = other.gameObject.transform;
        //             find.localPosition = Vector3.zero;
        //             // child.GetComponent<CinemachineVirtualCamera>().Follow = other.transform;
        //             // child.GetComponent<CinemachineVirtualCamera>().LookAt = other.transform;
        //         }
        //     }
        // }

        #endregion

        #region METHODS

        #endregion

        
    }

}