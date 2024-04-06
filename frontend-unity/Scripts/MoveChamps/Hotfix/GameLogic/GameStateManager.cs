using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;

using QFramework;
using SquareHero.Core;
using SquareHero.Hotfix.Events;
using SquareHero.Hotfix.Map;
using SquareHero.Hotfix.Model;
using SquareHero.Hotfix.Player;
using SquareHero.Hotfix.UI;
using UnityCommon.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SquareHero.Hotfix.GameLogic
{
    public class GameStateManager : MonoSingleton<GameStateManager>
    {
        
        #region EDITOR EXPOSED FIELDS

        public List<GameObject> ArrivedPlayers = new List<GameObject>();


        #endregion

        #region FIELDS

        private bool _createdRobot;
        private bool isReadyed;
        [SerializeField]
        private GameState _state;
        #endregion

        #region PROPERTIES

        public bool IsGameing {
            get
            {
                return _state == GameState.Gameing;
            }
        }
        
        public bool IsWaitPlayer {
            get
            {
                return _state == GameState.None;
            }
        }

        #endregion


        #region EVENT FUNCTION

        private void Awake()
        {
            mInstance = this;
        }
            
        
        
        
        private void Start()
        {

        }

        private void OnDestroy()
        {

        }





        #endregion
        
        #region METHODS

        public void StartGame(long roomId, int mapId, long localPlayerId)
        {
            var startGameAsync = StartGameAsync(roomId, mapId, localPlayerId);

            StartCoroutine(startGameAsync);
            LogKit.I($"start game : {roomId}, {mapId}, {localPlayerId}");
        }

        public IEnumerator StartGameAsync(long roomId, int mapId, long localPlayerId)
        {
            LogKit.I("Start");
            // yield return HttpHelper.Get($"https://info.squarehero.io/api/roommgr/racing_info?room_id=17606635080950784", result =>
            yield return HttpHelper.Get($"{GameUrlConstValue.RoomInfo.Url()}?room_id={roomId}", result =>
            {
                
                RoomInfoResponse roomInfoResponse = JsonUtil.FromJson<RoomInfoResponse>(result);
                LogKit.I($"Test ---- {roomInfoResponse.ToJson()}");
                RoomManager.Instance.RoomInfo = roomInfoResponse.data;
                RoomManager.Instance.LocalPlayerId = localPlayerId;
                RoomManager.Instance.GameType = roomInfoResponse.data.game_type;
                MapCreater.Instance.StartCreateMap(mapId);
                
            });
            
            
            yield break;
        }


        




        #endregion

        
    }

    public enum GameState
    {
        None,
        Ready,
        Gameing,
        GameOver
    }

    public class SettleData
    {
        public long game_id;
        public int game_type;
        public long settle_time;
        public List<RankInfo> rank_list;
    }

    public class RankInfo
    {
        public long user_id;
        public bool is_Robot;
        public int rank;
    }
}