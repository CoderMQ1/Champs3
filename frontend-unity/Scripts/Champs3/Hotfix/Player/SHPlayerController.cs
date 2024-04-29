// 
// 2023/12/04

using System;
using System.Collections.Generic;
using LitJson;
using QFramework;
using Sirenix.Serialization;
using champs3.Hotfix.Events;
using champs3.Hotfix.GameLogic;
using champs3.Hotfix.Generate;
using champs3.Hotfix.Map;
using champs3.Hotfix.Model;
using UnityCommon.Util;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace champs3.Hotfix.Player
{
    public class SHPlayerController : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS
        public PlayerGameData Data;
        public AbstractProp Props;
        public PlayerState State = new PlayerState();
        public List<MoveData> MoveDatas = new List<MoveData>();

        public MoveData _currentMoveData;
        private int _currentMoveDataIndex;
        private bool _started;
        public float _moveDistance;

        private CharacterAnimatorController _animatorController;

        
        public float EnterWaterSpeedY = 2f;
        private float _enterWaterSpeedY;
        public float ExitWaterSpeedY = 2f;
        public float ExitWaterPosZ = 2f;
        public float ClimbingOnTopDepth = 2f;
        private float _beforeEnterWaterPosY = 0;
        private bool _enteringWater;
        private bool _exitingWater;
        private float _waterDepth;
        private float _useTime;

        #endregion

        #region FIELDS

        #endregion

        #region PROPERTIES

        #endregion

        #region EVENT FUNCTION

        private void Awake()
        {
            _waterDepth = MapCreater.Instance.WaterDepth;
            TypeEventSystem.Global.Register<GameEvents.OnClientStartGame>(OnStartGame);
        }

        private void OnDestroy()
        {
            LogKit.I($"Destory {name}");
            TypeEventSystem.Global.UnRegister<GameEvents.OnClientStartGame>(OnStartGame);
        }

        private void Update()
        {
            // if (UsingProps != null)
            // {
            //     UsingProps.Update(Time.deltaTime);
            // }
            UpdateMove(Time.deltaTime);
        }
        
        private void FixedUpdate()
        {

        }

        #endregion

        #region METHODS
        

        #region Server

        
        #endregion


        private void OnStartGame(GameEvents.OnClientStartGame evt)
        {
            State.Start = true;
            _currentMoveData = MoveDatas[_currentMoveDataIndex];
        }

        private void ApplySkin()
        {
            var skinConfigTable = ExcelConfig.SkinConfigTable;
            var assetConfigTable = ExcelConfig.AssetConfigTable;
            if (string.IsNullOrEmpty(Data.SkinName))
            {
                Data.SkinName = "C_1_1";
            }
            var skinConfig = skinConfigTable.Data.Find(config =>
            {
                return config.Name == Data.SkinName;
            });

            Data.SkinId = skinConfig.Id;

            var assetConfig = assetConfigTable.Data.Find(config =>
            {
                return config.Id == skinConfig.AssetId;
            });

            ResourceManager.Instance.GetAssetAsync<GameObject>($"Skin_{assetConfig.PrefabName}", go =>
            {
                var a = GetComponentInChildren<Animator>();
                Destroy(a.gameObject);
                var instantiate = Instantiate(go, transform);
        
                var animator = instantiate.GetComponent<Animator>();
                _animatorController = GetComponent<CharacterAnimatorController>();
                _animatorController.Animator = animator;
                _animatorController.Initialize();
                // instantiate.GetComponent<RootMotionAdapter>().Root = transform;

                ActionKit.DelayFrame(1, () =>
                {
                    Props = SkinHelper.LoadProps(this, Data.PropIds[0]);
                }).Start(this);
                
            });
            
        }

        public void LoadedProps()
        {
            ActionKit.Delay(0.5f, () =>
            {
                InitlizedMoveData();
                State.Readyed = true;
            }).Start(this);
        }

        public void SetGameDate(PlayerGameData newData)
        {
            Data = newData;

            ApplySkin();
        }

        public void InitlizedMoveData()
        {
            var movePaths = MapCreater.Instance.MovePaths;
            int usedTimes = 0;
            if (RoomManager.Instance.LocalPlayerId == Data.UserId)
            {
                LogKit.I("Local Player InitlizedMoveData");
            }
            for (int i = 0; i < movePaths.Count; i++)
            {
                var movePath = movePaths[i];
                
                bool useProp = false;
                bool canUse = false;
                
                if (Props != null && Props.IsValid())
                {
                    canUse = Props.CanUse(movePath.TileType);
                    if (canUse)
                    {
                        if (Props.GetUseTimes() == -1 || Props.GetUseTimes() > usedTimes)
                        {
                            usedTimes++;
                            useProp = true;
                        }
                    }
                }
                
                float speed = 1;

                switch (movePath.TileType)
                {
                    case TileType.Start:
                    case TileType.Ground:
                    case TileType.End:
                        speed = Data.RunSpeed;
                        break;
                    case TileType.Water:
                        speed = Data.SwimSpeed;
                        break;
                    case TileType.Cliff:
                        speed = Data.ClimbSpeed;
                        break;
                    case TileType.Flight:
                        speed = Data.Flypeed;
                        break;
                }
                
                MoveDatas.Add(new MoveData()
                {
                    MovePath = movePath,
                    Speed = speed,
                    UseProps = useProp,
                    Props = useProp ? Props : null
                });
            }
        }

        public void UpdateEnterWater(float dateTime)
        {
            if (_enteringWater)
            {
                _enterWaterSpeedY = _enterWaterSpeedY - (9.8f * dateTime);
                var position = transform.position;
                var tarPosition = new Vector3(position.x, position.y + _enterWaterSpeedY * dateTime, position.z);
                
                if (_beforeEnterWaterPosY - tarPosition.y >= _waterDepth)
                {
                    tarPosition = new Vector3(position.x, _beforeEnterWaterPosY - _waterDepth, position.z);
                    _enteringWater = false;
                }
                
                transform.position = tarPosition;
            }
        }
        
        public void UpdateExitWater(float dateTime)
        {
            if (_exitingWater)
            {
                var position = transform.position;
                var tarPosition = new Vector3(position.x, position.y + ExitWaterSpeedY * dateTime, position.z);
                
                if (tarPosition.y >= _beforeEnterWaterPosY)
                {
                    tarPosition = new Vector3(position.x, _beforeEnterWaterPosY, position.z);
                    _exitingWater = false;
                }
                
                transform.position = tarPosition;
            }
        }


        public void UpdateMove(float dateTime)
        {
            if (State.Start)
            {
                if (_currentMoveDataIndex < MoveDatas.Count - 1)
                {
                    _useTime += dateTime;
                }
                ActionType actionType = ActionType.Idle;
                switch (_currentMoveData.MovePath.TileType)
                {
                    case TileType.Start:
                    case TileType.End:
                    case TileType.Ground:
                        actionType = ActionType.Run;
                        break;
                    case TileType.Water:
                        actionType = ActionType.Swiming;
                        break;
                    case TileType.Cliff:
                        actionType = ActionType.ClimbStart;
                        break;
                    case TileType.Flight:
                        actionType = ActionType.FlyStart;
                        break;
                }
                
                float speed = _currentMoveData.Speed;
                Vector3 motion = speed * dateTime * _currentMoveData.MovePath.MoveDirection;
                if (_currentMoveData.UseProps && _currentMoveData.Props.IsUsing())
                {
                    float propsSpeed = speed + speed * _currentMoveData.Props.GetSpeedAddition();
                    motion = propsSpeed * dateTime * _currentMoveData.MovePath.MoveDirection;
                    var leftAffetDistance = _currentMoveData.Props.GetLeftAffetDistance();
                    if (leftAffetDistance > 0 && motion.magnitude > leftAffetDistance)
                    {
                        float t = leftAffetDistance / speed;
                        
                        motion = propsSpeed* t * _currentMoveData.MovePath.MoveDirection + (speed * (dateTime - t) * _currentMoveData.MovePath.MoveDirection) ;
                    }
                }
                
                _moveDistance += motion.magnitude;
                // if (Data.UserId == RoomManager.Instance.LocalPlayerId && _currentMoveData.MovePath.TileType == TileType.Flight)
                // {
                    
                    // LogKit.I($"Test ----- {motion} - {motion.magnitude}" + (_currentMoveData.MovePath.Length - _moveDistance));
                // }
                if (_currentMoveData.MovePath.TileType == TileType.Flight &&(_currentMoveData.MovePath.Length - _moveDistance <= 1f))
                {
                    // LogKit.I($"Test ----- end fly");
                    actionType = ActionType.FlyEnd;
                }

                if (_moveDistance >= _currentMoveData.MovePath.Length)
                {
                    float m = _moveDistance - _currentMoveData.MovePath.Length;

                    float t = m / _currentMoveData.Speed;

                    if (_currentMoveDataIndex == MoveDatas.Count - 2)
                    {
                        _useTime -= t;
                    }
                    
                    if (_currentMoveDataIndex != MoveDatas.Count - 1)
                    {
                        var nextMoveData = MoveDatas[_currentMoveDataIndex + 1];
                        float nextSpeed = nextMoveData.Speed;
                        
                        if (nextMoveData.UseProps)
                        {
                            nextSpeed += nextSpeed * nextMoveData.Props.GetSpeedAddition();
                        }
                        
                        var nextMotion = nextSpeed * t * nextMoveData.MovePath.MoveDirection;
                        motion = speed * (dateTime - t) * _currentMoveData.MovePath.MoveDirection + nextMotion;
                        _moveDistance = nextMotion.magnitude;
                        
                        if (nextMoveData.MovePath.TileType == TileType.Water)
                        {
                            _enteringWater = true;
                            _enterWaterSpeedY = EnterWaterSpeedY;
                            _beforeEnterWaterPosY = transform.position.y;
                        }
                        
                        if (_currentMoveData.UseProps)
                        {
                            _currentMoveData.Props.Finish();
                        }
                        
                        NextMoveData();
                    }
                    else
                    {
                        TypeEventSystem.Global.Send(new PlayerEvents.PlayerArrived()
                        {
                            GameObject = gameObject,
                            UsedTime = _useTime
                        }); 
                        actionType = ActionType.Idle;
                        State.Start = false;
                    }
                }

                if (_currentMoveData.UseProps)
                {
                    _currentMoveData.Props.Update(motion.magnitude);
                }
                
                if (_currentMoveData.MovePath.TileType == TileType.Water &&
                    _currentMoveData.MovePath.Length - _moveDistance <= ExitWaterPosZ)
                {
                    _exitingWater = true;
                    actionType = ActionType.ClimbingOnTop;
                }
                
                if (_currentMoveData.MovePath.TileType == TileType.Cliff &&
                    _currentMoveData.MovePath.Length - _moveDistance <= ClimbingOnTopDepth)
                {
                    actionType = ActionType.ClimbingOnTop;
                }
                
 
                
                _animatorController.SetMoveType(actionType);
                _animatorController.SetSpeed(_currentMoveData.Speed);
                var position = transform.position;

                transform.position = position + motion;
            }
            
            UpdateEnterWater(dateTime);
            UpdateExitWater(dateTime);
        }

        private void NextMoveData()
        {
            LogKit.I($"{Data.UserName} {_currentMoveData.MovePath.TileType.ToString()} time {_useTime} - {_currentMoveData.UseProps}");
            _currentMoveDataIndex++;
            _currentMoveData = MoveDatas[_currentMoveDataIndex];
                        
            if (_currentMoveData.UseProps && _currentMoveData.Props.CanUse())
            {
                LogKit.I($"{Data.UserName}  use props  {_currentMoveData.Props.Config.Id}");
                _currentMoveData.Props.Use(gameObject);
            }
            if (_currentMoveData.UseProps && !_currentMoveData.Props.CanUse())
            {
                LogKit.I($"{Data.UserName} can't use props  {_currentMoveData.Props.Config.Id}");
                
            }
            
        }

        #endregion
    }
    [Serializable]
    public class PlayerState
    {
        public bool Initlized;
        public bool LoadedGameScene = true;
        public bool Readyed;
        public bool Start;
    }
    [Serializable]
    public struct MoveData
    {
        public MovePath MovePath;
        public float Speed;
        public bool UseProps;
        public AbstractProp Props;
    }
}