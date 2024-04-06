using System;
using UnityEngine;

namespace SquareHero.Hotfix
{
    public class CameraTarget : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        public Vector3 TargetPosition;

        #endregion

        #region FIELDS

        #endregion

        #region PROPERTIES

        #endregion

        #region EVENT FUNCTION

        private void Update()
        {
            // if (gameObject.activeSelf)
            // {
            //     var playerList = PlayerManager.Instance.PlayerList;
            //     float xSum = 0;
            //     float ySum = 0;
            //     float zSum = 0;
            //     for (int i = 0; i < playerList.Count; i++)
            //     {
            //         var player = playerList[i];
            //
            //         xSum += player.transform.position.x;
            //         ySum += player.transform.position.y;
            //         zSum += player.transform.position.z;
            //     }
            //
            //     TargetPosition = new Vector3(xSum / playerList.Count, ySum / playerList.Count, zSum / playerList.Count);
            //
            //
            //     var position = transform.position;
            //     
            //     transform.position = Vector3.Lerp(position, TargetPosition, Time.deltaTime * 15f);
            // }
        }

        #endregion

        #region METHODS

        #endregion


    }
}