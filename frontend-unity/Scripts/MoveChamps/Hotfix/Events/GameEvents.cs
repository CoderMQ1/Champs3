using System.Collections.Generic;
using UnityEngine;

namespace SquareHero.Hotfix.Events
{
    public class GameEvents
    {
        //当所有玩家同步完数据后
        public struct OnAllPlayerInitialized
        {
            
        }
        
        public struct OnAllPlayerReadyed
        {
            
        }
        
        public struct FinishCreateMap
        {
            
        }

        public struct OnServerStartGame
        {
            
        }
        
        public struct StartCountDown
        {
            public float CountDown;
        }
        
        public struct OnClientStartGame
        {
            
        }
        
        public struct OnClientLoadedScene
        {
            public string SceneName;
        }
        
        public struct  CreateRobot
        {
            
        }

        public struct OnServerGameOver
        {
            
        }
        
        public struct OnClientGameOver
        {
            public List<GameObject> Rank;
        }
        
        public struct OnCoinRankChange
        {
            
        }
        
        public struct GameReadyedOnClient
        {
            
        }
    }
}