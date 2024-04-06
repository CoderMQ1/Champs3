// 
// 2023/12/04

using System;
using Sirenix.Serialization;
using UnityEngine.Serialization;

namespace SquareHero.Hotfix.Model
{
    [Serializable]
    public class PlayerGameData
    {
        public long UserId = -10000;
        public string UserName;
        public int SkinId;
        public string SkinName;
        public int Track;
        public int ConnectionId;
        //玩家选择的道具
        public int[] PropIds = new []{1};
        
        public bool IsRobot = false;

        public float RunSpeed;
        public float SwimSpeed;
        public float ClimbSpeed;
        public float Flypeed;
        
    }
}