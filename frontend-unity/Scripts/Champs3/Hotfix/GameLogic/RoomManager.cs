// 
// 2023/12/22

using System;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using champs3.Hotfix.Events;
using champs3.Hotfix.Main;
using champs3.Hotfix.Map;
using champs3.Hotfix.Model;
using champs3.Hotfix.Player;
using UnityEngine;

namespace champs3.Hotfix.GameLogic
{
    public class RoomManager : MonoSingleton<RoomManager>
    {
        #region EDITOR EXPOSED FIELDS

        #endregion

        #region FIELDS

        public static RoomManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private static RoomManager _instance;
        public long LocalPlayerId;
        public RoomInfo RoomInfo;
        public int GameType;
        public RoomState State = RoomState.WaitMapCreated;
        private PlayerManager _playerManager;
        private int _arrivedPlayerNum;

        public int MapId;
        #endregion

        #region PROPERTIES
        
        #endregion

        #region EVENT FUNCTION

        private void Awake()
        {
            LogKit.I("Room manager awake");
            _instance = this;
            _playerManager = GetComponent<PlayerManager>();
            TypeEventSystem.Global.Register<MapEvents.OnClientFinishCreateMap>(OnCreatedMap);
            TypeEventSystem.Global.Register<PlayerEvents.PlayerArrived>(OnPlayerArrived);
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            TypeEventSystem.Global.UnRegister<MapEvents.OnClientFinishCreateMap>(OnCreatedMap);
            TypeEventSystem.Global.UnRegister<PlayerEvents.PlayerArrived>(OnPlayerArrived);
        }

        private void Update()
        {
            switch (State)
            {
                case RoomState.WaitPlayerReadyed:
                    bool isReadyed = true;
                    for (int i = 0; i < _playerManager.PlayerList.Count; i++)
                    {
                        if (!_playerManager.PlayerList[i].State.Readyed)
                        {
                            isReadyed = false;
                            break;
                        }
                    }

                    if (isReadyed)
                    {
                        SwitchState(RoomState.CountDown);
                       
                    }
                    break;
            }
        }

        #endregion

        #region METHODS


        public void OnCreatedMap(MapEvents.OnClientFinishCreateMap evt)
        {
            SwitchState(RoomState.WaitPlayerReadyed);
            for (int i = 0; i < RoomInfo.player_info_list.Count; i++)
            {
                var roomInfoPlayerInfo = RoomInfo.player_info_list[i];

                var playerGameData = GetPlayerData(roomInfoPlayerInfo);
                
                _playerManager.CreatePlayer(playerGameData);
            }
        }

        private List<GameObject> ArrivedPlayer = new List<GameObject>();
        public void OnPlayerArrived(PlayerEvents.PlayerArrived evt)
        {
            _arrivedPlayerNum++;
            ArrivedPlayer.Add(evt.GameObject);
            if (_arrivedPlayerNum == RoomInfo.player_info_list.Count)
            {
                LogKit.I("Game Over");
                TypeEventSystem.Global.Send(new GameEvents.OnClientGameOver(){Rank = ArrivedPlayer});
            }
        }

        public PlayerGameData GetPlayerData(PlayerInfo playerInfo)
        {
            PlayerGameData data = new PlayerGameData();
            data.UserId = playerInfo.user_id;
            data.UserName = playerInfo.name;
            data.IsRobot = playerInfo.is_robot;
            data.RunSpeed = playerInfo.role_info.GetRunSpeed();
            data.SwimSpeed = playerInfo.role_info.GetSwimSpeed();
            data.ClimbSpeed = playerInfo.role_info.GetClimbSpeed();
            data.Flypeed = playerInfo.role_info.GetFlySpeed();
            data.PropIds = new[] {playerInfo.item_id};
            // data.PropIds = new[] {1};
            data.SkinName = playerInfo.role_info.Character;
            data.Track = playerInfo.track_id;
            
            return data;
        }
        
        public PlayerGameData GetPlayerData(long userId)
        {
            PlayerGameData data = new PlayerGameData();
            var playerInfo = RoomInfo.player_info_list.Find(info =>
            {
                return info.user_id == userId;
            });

            if (playerInfo == null)
            {
                LogKit.E($"player [{userId} not find data]");
            }
            else
            {
                data.UserId = playerInfo.user_id;
                data.UserName = playerInfo.name;
                data.IsRobot = playerInfo.is_robot;
                data.RunSpeed = playerInfo.role_info.Attributes[(int)AttributesType.RunSpeed].Speed;
                data.SwimSpeed = playerInfo.role_info.Attributes[(int)AttributesType.SwimSpeed].Speed;
                data.ClimbSpeed = playerInfo.role_info.Attributes[(int)AttributesType.ClimbSpeed].Speed;
                data.Flypeed = playerInfo.role_info.Attributes[(int)AttributesType.FlySpeed].Speed;
                data.PropIds = new[] {playerInfo.item_id};
                // data.PropIds = new[] {1};
                data.SkinName = playerInfo.role_info.Character;
                data.Track = playerInfo.track_id;
            }
            return data;
        }

        private void SwitchState(RoomState state)
        {
            if (State == state)
            {
                return;
            }

            State = state;

            switch (state)
            {
                case RoomState.Gameing:
                    // SettlementHelper.Settlement(MapCreater.Instance._mapData.MapId, RoomInfo.player_info_list);
                    TypeEventSystem.Global.Send(new GameEvents.OnClientStartGame());
                    break;
                case RoomState.CountDown:
                    ActionKit.Sequence().Callback(() =>
                    {
                        TypeEventSystem.Global.Send(new GameEvents.StartCountDown() { CountDown = 3 });
                    }).Delay(3).Callback(() =>
                    {
                        SwitchState(RoomState.Gameing);
                    }).Start(this);
                    break;
            }
        }

        #endregion



    }

    public enum RoomState
    {
        WaitMapCreated,
        WaitPlayerReadyed,
        Gameing,
        CountDown,
        GameOver
    }



}