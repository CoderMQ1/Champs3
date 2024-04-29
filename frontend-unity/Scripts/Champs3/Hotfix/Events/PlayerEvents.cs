using champs3.Hotfix.Generate;
using UnityEngine;

namespace champs3.Hotfix.Events
{
    public class PlayerEvents
    {
        public struct OnPlayerInitialized
        {
            public long UserId;
        }
        
        public struct OnPlayerReadyed
        {
            public long UserId;
            
        }
        
        public struct PlayerArrived
        {
            public GameObject GameObject;
            public float UsedTime;
        }
        
        public struct OnPlayerTriggerMoveNode
        {
            public GameObject Player;

            public MoveNode MoveNode;
        }
        
        public struct OnPropUse
        {
            public PropsConfig Config;
            public GameObject User;
            public GameObject Affecter;
        }
        
        public struct OnPropAffectEnd
        {
            public PropsConfig Config;
            public GameObject User;
            public GameObject Affecter;
        }
        
        public struct JoinGame
        {
            public GameObject PlayerObj;
        }
        
        public struct GetCoin
        {
            public long UserId;
            public int Coin;
        }
        
    }

    public struct PlayerJoin
    {
        public GameObject PlayerObj;
        public int CurrentPlayerCount;
    }

    public struct PlayerLeave
    {
        public GameObject PlayerObj;
        public int CurrentPlayerCount;
    }



}