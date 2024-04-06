using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using SquareHero.Hotfix.Events;
using SquareHero.Hotfix.GameLogic;
using SquareHero.Hotfix.Model;
using SquareHero.Hotfix.Player;
using UnityEngine;
using Random = UnityEngine.Random;
using SysRandom = System.Random;

namespace SquareHero.Hotfix.Map
{
    public class MapCreater : MonoSingleton<MapCreater>
    {
        public TileGroup tileGroup;
        // public List<TileData> TileDatas;
        public GameObject GiftPrefab;
        private List<List<int>> _giftPos;
        private List<GiftModel> _giftTypes;
        private Transform _creatTransform;
        private List<List<GiftModel>> _trackGiftModes;
        
        [SerializeField]
        private List<GiftData> _giftDatas;
        public MapData _mapData;
        public int MapId = 1;

        public List<Transform> StartPositions = new List<Transform>();
        public List<MovePath> MovePaths = new List<MovePath>();
        public float WaterDepth = 2f;
        #region METHOD

        
        
        #region Server

//         public override void OnStartServer()
//         {
//             base.OnStartServer();
//             MapId = RoomManager.Instance.MapId;
//             
// #if !UNITY_EDITOR && UNITY_SERVER
//             _giftDatas = RoomManager.Instance.RoomInfo.track_rewards;
//             MapId = GameServerManager.Instance.ServerConfig.MapId;
// #endif
//             ResourceManager.Instance.GetAssetAsync<MapData>($"Map_{MapId}", data =>
//             {
//                 _mapData = data;
//                 CreateGiftData();
//                 _mapData.GiftModels = _trackGiftModes;
//                 CreateMap();
//             });
//         }
        
        // [Command(requiresAuthority = false)]
        // public void CmdRequestMap(NetworkConnectionToClient sender = null)
        // {
        //     RpcOnClientCreateMap(sender, _mapData);
        // }
        
        // [Server]
        // private void ServerCreateMap()
        // {
        //     CreateMap();
        //     TypeEventSystem.Global.Send(new MapEvents.OnServerFinishCreateMap());
        //
        //     if (!isServerOnly)
        //     {
        //         TypeEventSystem.Global.Send(new MapEvents.OnClientFinishCreateMap());
        //     }
        // }

      


        #endregion

        #region Client
        
        // public override void OnStartClient()
        // {
        //     base.OnStartClient();
        //     // RegistClientEvent();
        //     // OnClientCreateMap();
        //     StartCoroutine(RequestMapData());
        // }

        // public IEnumerator RequestMapData()
        // {
        //     yield return new WaitForSeconds(0.5f);
        //     CmdRequestMap();
        // }

        // public override void OnStopClient()
        // {
        //     base.OnStopClient();
        //     
        //     
        //     this.gameObject.SetActive(true);
        // }

        // [TargetRpc]
        // public void RpcOnClientCreateMap(NetworkConnectionToClient target, MapData mapData)
        // {
        //     if (isClientOnly)
        //     {
        //         _mapData = mapData;
        //         // TileDatas = _mapData.TileDatas;
        //         CreateMap();
        //         TypeEventSystem.Global.Send(new MapEvents.OnClientFinishCreateMap());
        //     }
        //     else
        //     {
        //         FinishCreateMap(NetworkClient.localPlayer.GetComponent<SHPlayerController>().Data.UserId);
        //     }
        // }

        private void RegistClientEvent()
        {
            // TypeEventSystem.Global.Register<MapEvents.OnClientCreateMap>(OnClientCreateMap);
        }

        private void UnRegistClientEvents()
        {
            // TypeEventSystem.Global.UnRegister<MapEvents.OnClientCreateMap>(OnClientCreateMap);
        }

        // private void OnClientCreateMap()
        // {
        //     if (isClientOnly)
        //     {
        //         CreateMap();
        //         TypeEventSystem.Global.Send(new MapEvents.OnClientFinishCreateMap());
        //         // NetworkClient.localPlayer.GetComponent<SHPlayerController>().CmdOnClientCreatedMap();
        //     }
        //     else
        //     {
        //         FinishCreateMap(NetworkClient.localPlayer.GetComponent<SHPlayerController>().Data.UserId);
        //     }
        // }
        
        #endregion
        
        private void Awake()
        {
            _creatTransform = transform;
            
            LogKit.I("Mapcreate awake");
            
            
        
                
            
        }

        private void Start()
        {
            // for (int i = 0; i < transform.childCount; i++)
            // {
            //     StaticBatchingUtility.Combine(transform.GetChild(i).gameObject);
            //     
            // }
            //
            Debug.Log("test");
        }


        // Update is called once per frame
        void Update()
        {
            
        }

        private void OnDestroy()
        {
            // if (isClient)
            // {
            //     // UnRegistClientEvents();
            // }
        }
        private void CreateGiftData()
        {

#if !UNITY_EDITOR && UNITY_SERVER
            if (_giftDatas == null)
            {
                return;
            }
            _giftDatas.Sort((data1, data2) =>
            {
                int result = 0;
                if (data1.TrackId > data2.TrackId)
                {
                    result = 1;
                }

                if (data1.TrackId < data2.TrackId)
                {
                    result = -1;
                }
                
                return result;
            });
            _trackGiftModes = new List<List<GiftModel>>();

            for (int i = 0; i < 5; i++)
            {

                List<GiftModel> giftModels = new List<GiftModel>();
                _trackGiftModes.Add(giftModels);
                for (int j = 0; j < _giftDatas[i].RewardList.Count; j++)
                {
                    var rewardType = _giftDatas[i].RewardList[j];

                    GiftModel model = new GiftModel();
                    model.Track = _giftDatas[i].TrackId - 1;
                    if (rewardType.RewardType == 0)
                    {
                        model.NeedCreate = false;
                    }
                    else
                    {
                        model.RewardType = (RewardType)rewardType.RewardType;
                        model.CoinCount = rewardType.TokenNum;
                        model.NeedCreate = true;
                    }
                    giftModels.Add(model);
                }
            }
#else
            _giftTypes = new List<GiftModel>();

            for (int i = 0; i < 18; i++)
            {
                _giftTypes.Add(new GiftModel()
                {
                    RewardType = RewardType.Lv1,
                    CoinCount = 10,
                    NeedCreate = true
                });
            }
            for (int i = 0; i < 6; i++)
            {
                _giftTypes.Add(new GiftModel()
                {
                    RewardType = RewardType.Lv2,
                    CoinCount = 20,
                    NeedCreate = true
                });
            }
            
            _giftTypes.Add(new GiftModel()
            {
                RewardType = RewardType.Lv3,
                CoinCount = 99,
                NeedCreate = true
            });


            List<GiftModel> tempGiftModels = new List<GiftModel>();

            // 打乱顺序
            int count = _giftTypes.Count;

            for (int i = 0; i < count; i++)
            {
                int index = Random.Range(0, _giftTypes.Count);
                tempGiftModels.Add(_giftTypes[index]);
                _giftTypes.RemoveAt(index);
            }

            _giftTypes = tempGiftModels;
            
            
            //
            List<int> giftTrackPos = new List<int>();

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    giftTrackPos.Add(i);
                }
            }

            // int track = Random.Range(0, 5);
            
            // giftTrackPos.RemoveAt(track * 3);


            List<int> tempTrackIds = new List<int>();

            count = giftTrackPos.Count;

            for (int i = 0; i < count; i++)
            {
                int index = Random.Range(0, giftTrackPos.Count);
                tempTrackIds.Add(giftTrackPos[index]);
                giftTrackPos.RemoveAt(index);
            }

            giftTrackPos = tempTrackIds;

            for (int i = 0; i < _giftTypes.Count; i++)
            {
                var giftModel = _giftTypes[i];
                giftModel.Track = giftTrackPos[i];
                _giftTypes[i] = giftModel;
            }
            
            
            int offset = 3;

            int posCount = (_mapData.TileDatas.Count - 2) / 3;

            int t = posCount;

            List<int> giftCounts = new List<int>(posCount);
            for (int i = 0; i < posCount; i++)
            {
                giftCounts.Add(0);
            }

            _trackGiftModes = new List<List<GiftModel>>();
            for (int i = 0; i < 5; i++)
            {
                List<GiftModel> giftModels = new List<GiftModel>();
                _trackGiftModes.Add(giftModels);
            }

            for (int i = 0; i < _giftTypes.Count; i++)
            {
                _trackGiftModes[_giftTypes[i].Track].Add(_giftTypes[i]);
            }

            for (int i = 0; i < 5; i++)
            {
                var giftMode = _trackGiftModes[i];
                var l = posCount - giftMode.Count;
                for (int j = 0; j < l; j++)
                {
                    giftMode.Add(new GiftModel(){NeedCreate = false});
                }

                List<GiftModel> temp = new List<GiftModel>();
                int trackCount = giftMode.Count;
                for (int j = 0; j < trackCount; j++)
                {
                    int index = Random.Range(0, giftMode.Count);
                        
                    temp.Add(giftMode[index]);
                    giftMode.RemoveAt(index);
                }

                _trackGiftModes[i] = temp;

            }
#endif
        }

        void CreateTile(int tileIndex, Vector3 position, TileData tileData)
        {
            var tileCount = tileGroup.data[tileData.tileType][tileData.tilePosition];

            int index = Random.Range(0, tileCount) + 1;

            string location = $"{tileData.tileType.ToString()}_{(int)tileData.tilePosition}_{index}";

            ResourceManager.Instance.GetAssetAsync<GameObject>(location, asset =>
            {
                var instantiate = Instantiate(asset, _creatTransform);
                instantiate.transform.localPosition = position;
                if (tileData.tileType == TileType.Water && tileData.tilePosition == TilePosition.Start)
                {
                    var water = instantiate.transform.Find("Track/Water");
                    CreateWater(tileIndex, water);
                }

                if (tileData.tileType == TileType.Start)
                {
                    var find = instantiate.transform.Find("StartPosition");

                    for (int i = 0; i < find.childCount; i++)
                    {
                        var startPos = find.Find($"Pos{i + 1}");
                        StartPositions.Add(startPos);
                    }
                }

                if (tileIndex == _mapData.TileDatas.Count - 1)
                {
                    StaticBatchingUtility.Combine(gameObject);
                    TypeEventSystem.Global.Send(new MapEvents.OnClientFinishCreateMap());
                }
            });
        }

        void CreateMap()
        {
            float x = 0, y = 0, z = 0;
            
            for (int i = 0; i < _mapData.TileDatas.Count; i++)
            {
                var tileData = _mapData.TileDatas[i];
                CreateTile(i, new Vector3(x, y, z), tileData);

                switch (tileData.tileType)
                {
                    case TileType.Start:
                    case TileType.End:
                    case TileType.Ground:
                    case TileType.Water:
                        z += 2.5f;
                        break;
                    case TileType.Cliff:
                        y += 2.5f;
                        break;
                    case TileType.Flight:
                        z += 2.165f;
                        y -= 1.25f;
                        break;
                        
                }
            }

            for (int i = 0; i < _mapData.TileDatas.Count; i++)
            {
                var tileData = _mapData.TileDatas[i];
                int tileCount = 0;
                switch (tileData.tileType)
                {
                    case TileType.Start:
                    case TileType.End:
                        MovePaths.Add(new MovePath(){TileType = tileData.tileType, MoveDirection = Vector3.forward, Length = 2.5f});
                        break;
                    case TileType.Ground:
                    case TileType.Water:
                    case TileType.Cliff:
                    case TileType.Flight:
                        for (int j = i; j < _mapData.TileDatas.Count; j++)
                        {
                            if (_mapData.TileDatas[j].tileType == tileData.tileType)
                            {
                                tileCount++;
                            }
                            else
                            {
                                i = j - 1;
                                break;
                            }
                        }
                        LogKit.I($"{tileData.tileType} Count : {tileCount}");
                        Vector3 dir = Vector3.forward;

                        if (tileData.tileType == TileType.Cliff)
                        {
                            dir = Vector3.up;
                        }

                        if (tileData.tileType == TileType.Flight)
                        {
                            dir = new Vector3(0, -1, 1.732f).normalized;
                        }
                        MovePaths.Add(new MovePath(){TileType = tileData.tileType, MoveDirection = dir, Length = 5f * tileCount});
                        break;
                }
            }
            // if (_mapData != null && _mapData.TileDatas.Count > 0)
            // {
            //     int pos = 0;
            //     int createIndex = 0;
            //     for (int i = 0; i < _mapData.TileDatas.Count; i++)
            //     {
            //         // try
            //         // {
            //             var tileData = _mapData.TileDatas[i];
            //             Dictionary<TilePosition, List<TileAsset>> typeGroup;
            //             if (tileGroup.data.TryGetValue(tileData.tileType, out typeGroup))
            //             {
            //                 List<TileAsset> tileAssets;
            //                 if (typeGroup.TryGetValue(tileData.tilePosition, out tileAssets))
            //                 {
            //                     if (tileAssets.Count > 0)
            //                     {
            //                         var index = Random.Range(0, tileAssets.Count);
            //
            //                         var tileAsset = tileAssets[index];
            //                         
            //                         var go = Instantiate(tileAsset.prefab, transform);
            //                         go.transform.position = _creatTransform.position;
            //                         
            //                         var startTransform = go.transform.Find("StartPosition");
            //                         
            //                         // if (isClient)
            //                         // {
            //                         //     if (tileData.tileType == TileType.Water && tileData.tilePosition == TilePosition.Start)
            //                         //     {
            //                         //         var water = go.transform.Find("Track/Water");
            //                         //         CreateWater(i, water);
            //                         //     }
            //                         // }
            //                         
            //                         var endTransform = go.transform.Find("EndPosition");
            //                         var moveNodes = go.transform.Find("MoveNodes");
            //                         RegisteMoveNode(moveNodes);
            //                         
            //                         _creatTransform = endTransform ? endTransform : _creatTransform;
            //                         if (_mapData.GiftModels != null)
            //                         {
            //                             if ((i + 1)% 3 == 0)
            //                             {
            //                                 var gifts = go.transform.Find("Gifts");
            //                                 // var giftCount = giftCounts[pos];
            //                            
            //
            //                                 for (int j = 0; j < _mapData.GiftModels.Count; j++)
            //                                 {
            //                                     if (pos < _mapData.GiftModels[j].Count)
            //                                     {
            //                                         CreatGift(gifts, _mapData.GiftModels[j][pos]);
            //                                     }
            //                                 }
            //                                 pos++;
            //                                 // CreatGift(gifts, createIndex, giftCount);
            //                                 // createIndex += giftCount;
            //                             }
            //                         }
            //                         
            //                     }
            //                 
            //                 }
            //             }
            //         // }
            //         // catch (Exception e)
            //         // {
            //         //     LogKit.E($"Create Map {i} Error");
            //         //     // Console.WriteLine(e);
            //         //     throw e;
            //         // }
            //     }
            // }
        }


        void CreatGift(Transform transform, int startPos, int count)
        {
            int max = Mathf.Min(_giftTypes.Count, startPos + count);
            for (int i = startPos; i < max; i++)
            {
                var instantiate = Instantiate(GiftPrefab, transform);
                var gift = instantiate.GetComponent<Gift>();
                gift.GiftModel = _giftTypes[i];
            }
        }
        
        void CreatGift(Transform transform, GiftModel giftModel)
        {

            if (!giftModel.NeedCreate)
            {
                return;
            }
            
            var instantiate = Instantiate(GiftPrefab, transform);
            var gift = instantiate.GetComponent<Gift>();
            gift.GiftModel = giftModel;

        }
        
        void CreatGift(Transform transform, RewardInfo rewardInfo)
        {

            if (rewardInfo.RewardType == 0)
            {
                return;
            }
            
            var instantiate = Instantiate(GiftPrefab, transform);
            var gift = instantiate.GetComponent<Gift>();
            // gift.GiftModel = giftModel;

        }

        void CreateWater(int index, Transform water)
        {
            int waterLength = 0;
        
            for (int i = index; i < _mapData.TileDatas.Count; i++)
            {
                var tileData = _mapData.TileDatas[i];
                if (tileData.tileType == TileType.Water)
                {
                    waterLength++;
                }
                else
                {
                    break;
                }
            }

            water.localScale = new Vector3(0.05f, 1, waterLength * 0.025f);
            var position = water.localPosition;
            water.localPosition = new Vector3(position.x, position.y, position.z + waterLength * 1.25f);
        }
        public void RegisteMoveNode(Transform moveNodes)
        {
            if (!moveNodes)
            {
                return;
            }
            var nodes = moveNodes.GetComponentsInChildren<MoveNode>();

            for (int i = 0; i < nodes.Length; i++)
            {
                RegisteMoveNode(nodes[i]);
            }
        }
        
        public void RegisteMoveNode(MoveNode moveNode)
        {
            if (moveNode)
            {
                LogKit.I($"Rigiste MoveNode {moveNode.transform.parent.name}.{moveNode.name}");
                moveNode.GetComponent<Collider>().isTrigger = true;
                moveNode.tag = "MoveNode";
                var instanceCurrentLevelController = LevelManager.Instance.CurrentLevelController;
                var mapModel = instanceCurrentLevelController.GetArchitecture()
                    .GetModel<MapModel>();
                if (mapModel != null)
                {
                    var index = mapModel.MoveNodes.Count;
                    mapModel.MoveNodes.Add(moveNode);
                    moveNode.Index = index;
                }
            }
        }
        
        private void FinishCreateMap(long userId)
        {
            TypeEventSystem.Global.Send(new MapEvents.OnClientFinishCreateMap()
            {
                UserId = userId
            });
        }

        public void StartCreateMap(int mapId)
        {
            MapId = mapId;
            ResourceManager.Instance.GetAssetAsync<MapData>($"Map_{MapId}", mapdata =>
            {
                _mapData = mapdata;
                CreateMap();
            });
        }

        #endregion

        // protected override void OnValidate()
        // {
        //     base.OnValidate();
        //
        //     if (tileGroup == null)
        //     {
        //         LogKit.E("Tile is NUL");
        //
        //     }
        //     else
        //     {
        //         foreach (var item in tileGroup.data)
        //         {
        //             if (item.Value == null)
        //             {
        //                 LogKit.E($"Tile {item.Key} is nul");
        //             }
        //             else
        //             {
        //                 foreach (var tiles in item.Value)
        //                 {
        //                     if (tiles.Value == null)
        //                     {
        //                         LogKit.E($"Tile {item.Key} {tiles.Key} is nul");
        //                     }
        //                     else
        //                     {
        //                         for (int i = 0; i < tiles.Value.Count; i++)
        //                         {
        //                             if (tiles.Value[i] == null)
        //                             {
        //                                 LogKit.E($"Tile {item.Key} {tiles.Key} {i}is nul");
        //                             }
        //                         }
        //                     }
        //                 }
        //             }
        //         }                
        //     }
        // }
    }

    [Serializable]
    public struct MovePath
    {
        //路段类型
        public TileType TileType;
        public Vector3 MoveDirection;
        //路段长度
        public float Length;
    }

    
}

