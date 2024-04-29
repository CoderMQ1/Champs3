using System;
using System.Collections.Generic;
using QFramework;
using champs3.Hotfix.Events;
using champs3.Hotfix.GameLogic;
using champs3.Hotfix.Map;
using champs3.Hotfix.Model;
using champs3.Hotfix.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace champs3.Hotfix
{
    public class PlayerManager : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS
        [Tooltip("玩家预制体")]
        public GameObject PlayerPrefab;
        public GameObject RobotPrefab;
        public List<SHPlayerController> PlayerList = new List<SHPlayerController>();
        
        public int AllPlayerCount;
        public int RealPlayerCount;

        private int _connectedPlayer;
        private int _initializedPlayerCount;
        private int _readyedPlayerCount;
        #endregion
        
        #region FIELDS

        public static PlayerManager Instance
        {
            get { return _instance; }
        }

        private static PlayerManager _instance;
        private List<Loading> _loadings;

        private string[] RobotName = new string[]
        {
            "Aidan",
            "Edwin",
            "Ian",
            "Jim",
            "Keith",
        };

        private List<int> _trackId = new List<int>() { 0, 1, 2, 3, 4 };
        #endregion

        #region PROPERTIES

        #endregion
        #region EVENT FUNCTION

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {

            
        }

        private void AllPlayerCountChange(int oldVal, int newVal)
        {
            LogKit.I($"AllPlayerCount Change: {oldVal} ----{newVal}");
            AllPlayerCount = newVal;
        }
        
        private void RelPlayerCountChange(int oldVal, int newVal)
        {
            LogKit.I($"RelPlayerCount Change: {oldVal} ----{newVal}");
            RealPlayerCount = newVal;
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        private void Update()
        {
            
        }

        #endregion
        #region METHODS
        
        #region Server
        
        // private void RegisteServerEvents()
        // {
        //     TypeEventSystem.Global.Register<GameEvents.CreateRobot>(CreateRobot);
        //     TypeEventSystem.Global.Register<PlayerEvents.JoinGame>(OnPlayerJoin);
        // }

        // private void UnRegisteServerEvents()
        // {
        //     TypeEventSystem.Global.UnRegister<GameEvents.CreateRobot>(CreateRobot);
        //     TypeEventSystem.Global.UnRegister<PlayerEvents.JoinGame>(OnPlayerJoin);
        // }

        #endregion

        #region Client




        



        #endregion    


        public GameObject GetPlayerPrefab()
        {
            return PlayerPrefab;
        }

        public void CreatePlayer(PlayerGameData data)
        {
            var playerObj = Instantiate(GetPlayerPrefab(), transform);
            
            var shPlayerController = playerObj.GetComponent<SHPlayerController>();
            shPlayerController.SetGameDate(data);
            var startPosition = MapCreater.Instance.StartPositions[data.Track - 1];
            playerObj.transform.position = startPosition.position;
            
            PlayerList.Add(shPlayerController);
        }

        // public void CreateRobot(GameEvents.CreateRobot evt)
        // {
        //     for (int i = 0; i < PlayerList.Count; i++)
        //     {
        //         var shPlayerController = PlayerList[i].GetComponent<SHPlayerController>();
        //         _trackId.Remove(shPlayerController.Data.Track);
        //     }
        //     int trackId = 0;
        //     for (int i = 0; i < GameGlobalConfig.Instance.RobotCount; i++)
        //     {
        //         var startPosition = NetworkManager.singleton.GetStartPosition();
        //         startPosition.NullCheck();
        //         var robot = Instantiate(RobotPrefab, startPosition.position, startPosition.rotation);
        //         var r = robot.GetComponent<Robot>();
        //         PlayerGameData data = new PlayerGameData();
        //         data.UserId = 100 + i;
        //         data.UserName = RobotName[i];
        //         data.IsRobot = true;
        //         data.RunSpeed = Random.Range(3.5f, 4.5f);
        //         data.SwimSpeed = Random.Range(2.5f, 3.5f);
        //         data.ClimbSpeed = Random.Range(1.5f, 2.5f);
        //         data.Flypeed = Random.Range(2.5f, 3.5f);
        //         data.PropIds = new[] {3 + i };
        //         data.SkinId = i + 2;
        //         data.Track = _trackId[trackId];
        //         trackId++;
        //         NetworkServer.Spawn(robot);
        //     }
        // }

        // public void SimulateCreateRobot()
        // {
        //     for (int i = 0; i < PlayerList.Count; i++)
        //     {
        //         var shPlayerController = PlayerList[i].GetComponent<SHPlayerController>();
        //         _trackId.Remove(shPlayerController.Data.Track);
        //     }
        //     int trackId = 0;
        //     for (int i = 0; i < GameGlobalConfig.Instance.RobotCount; i++)
        //     {
        //         var startPosition = NetworkManager.singleton.GetStartPosition();
        //         startPosition.NullCheck();
        //         var robot = Instantiate(RobotPrefab, startPosition.position, startPosition.rotation);
        //         var r = robot.GetComponent<Robot>();
        //         PlayerGameData data= new PlayerGameData();
        //         data.UserId = 100 + i;
        //         data.UserName = RobotName[i];
        //         data.IsRobot = true;
        //         data.RunSpeed = Random.Range(3.5f, 4.5f);
        //         data.SwimSpeed = Random.Range(2.5f, 3.5f);
        //         data.ClimbSpeed = Random.Range(1.5f, 2.5f);
        //         data.Flypeed = Random.Range(2.5f, 3.5f);
        //         data.PropIds = new[] {3 + i };
        //         data.SkinId = i + 2;
        //         data.Track = _trackId[trackId];
        //         trackId++;
        //         NetworkServer.Spawn(robot);
        //         ActionKit.Delay(2f, () =>
        //         {
        //             r.InitializedRobot();
        //         }).Start(this);
        //     }
        // }

        // public void CreateRobot()
        // {
        //     var roomInfo = RoomManager.Instance.RoomInfo;
        //     for (int i = 0; i < roomInfo.player_info_list.Count; i++)
        //     {
        //         var playerInfo = roomInfo.player_info_list[i];
        //
        //         if (playerInfo.is_robot)
        //         {
        //             var startPosition = NetworkManager.singleton.GetStartPosition();
        //             startPosition.NullCheck();
        //             var robot = Instantiate(RobotPrefab, startPosition.position, startPosition.rotation);
        //             var robotData = robot.GetComponent<Robot>();
        //             PlayerGameData data = new PlayerGameData();
        //             data.UserId = playerInfo.user_id;
        //             data.UserName = playerInfo.name;
        //             data.IsRobot = true;
        //             data.RunSpeed = playerInfo.role_info.Attributes[(int)AttributesType.RunSpeed].Speed;
        //             data.SwimSpeed = playerInfo.role_info.Attributes[(int)AttributesType.SwimSpeed].Speed;
        //             data.ClimbSpeed = playerInfo.role_info.Attributes[(int)AttributesType.ClimbSpeed].Speed;
        //             data.Flypeed = playerInfo.role_info.Attributes[(int)AttributesType.FlySpeed].Speed;
        //             // data.PropIds = new[] {2 + i };
        //             data.PropIds = new[] {playerInfo.item_id};
        //             data.SkinName = playerInfo.role_info.Character;
        //             data.Track = playerInfo.track_id;
        //             NetworkServer.Spawn(robot);
        //         }
        //     }
        //     
        // }

        public void OnPlayerJoin(PlayerEvents.JoinGame evt)
        {
            var shPlayerController = evt.PlayerObj.GetComponent<SHPlayerController>();
            PlayerList.Add(shPlayerController);
            evt.PlayerObj.transform.parent = transform;
            //
            // if (isServer)
            // {
            //     if (!shPlayerController.Data.IsRobot)
            //     {
            //         _connectedPlayer++;
            //         if (_connectedPlayer == RealPlayerCount)
            //         {
            //             CreateRobot();
            //         }
            //     }
            // }
            
            // var controller = evt.PlayerObj.GetComponent<SHPlayerController>();
            //
            //
            // if (controller && isServer)
            // {
            //     CreatePlayerLoading(controller);
            // }
        }
        private void CreatePlayerLoading(SHPlayerController controller)
        {
            long id = controller.Data.UserId;
            LogKit.I($"server to Client   {id}");
            Loading loading = new Loading(id);
            WaitSyncDataNode syncDataNode = new WaitSyncDataNode(id);
            syncDataNode.CompletHandler += () =>
            {
                OnPlayerInitialized(id);
            };
            loading.AddNode(syncDataNode);
            if (!controller.Data.IsRobot)
            {
                WaitLoadedSceneNode  waitLoadedSceneNode = new WaitLoadedSceneNode(GameGlobalConfig.Instance.Level1, id);
                loading.AddNode(waitLoadedSceneNode);
                WaitCreateMapNode waitCreateMapNode = new WaitCreateMapNode(id);
                loading.AddNode(waitCreateMapNode);
            }
           
            _loadings.Add(loading);
            loading.OnCompleted += () =>
            {
   
                OnPlayerReady(id);
            };
            loading.Start();
        }

        private void OnPlayerInitialized(long userId)
        {
            _initializedPlayerCount++;
        }


        private void OnPlayerReady(long userId)
        {
            LogKit.I($"OnPlayerReady   {userId}");
            _readyedPlayerCount++;
            if (_readyedPlayerCount == AllPlayerCount)
            {
                TypeEventSystem.Global.Send(new GameEvents.OnAllPlayerReadyed()
                {
                    
                });
            }
        }
        
        #endregion

        
        
        
    }
    
}