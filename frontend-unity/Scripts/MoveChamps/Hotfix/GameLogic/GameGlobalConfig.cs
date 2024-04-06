// 
// 2023/12/04

using System;

using SquareHero.Hotfix.Model;
using UnityEngine;

namespace SquareHero.Hotfix
{
    public class GameGlobalConfig : MonoBehaviour
    {

        
        #region EDITOR EXPOSED FIELDS
        [Tooltip("玩家数量，包括机器人")]
        public int PlayerCount;
        public float RunSpeed = 4;
        public float SwimSpeed = 3;
        public float ClimbSpeed = 2;
        public float FlySpeed = 3;
        public int MaxConnectionCount = 1;

        public PlayerGameData LocalPlayerData;
        public string Level1;
        #endregion

        #region FIELDS
        private static GameGlobalConfig _instance;

        #endregion

        #region PROPERTIES

        public static GameGlobalConfig Instance {
            get
            {
                return _instance;
            }
        }

        public int RobotCount
        {
            get
            {
                return PlayerCount - MaxConnectionCount;
            }
        }

        #endregion

        #region EVENT FUNCTION

        private void Awake()
        {
            _instance = this;

        }

        private void OnDestroy()
        {
            _instance = null;
        }

        #endregion

        #region METHODS

        #endregion

    }
}