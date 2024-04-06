using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using SquareHero.Core.Loading;
using SquareHero.Hotfix.Events;
using SquareHero.Hotfix.GameLogic;
using SquareHero.Hotfix.Generate;
using SquareHero.Hotfix.Main;
using SquareHero.Hotfix.Map;
using SquareHero.Hotfix.Model;
using SquareHero.Hotfix.Net;
using SquareHero.Hotfix.Player;
using SquareHero.Hotfix.Toast;
using UnityCommon.Util;
using UnityEngine.U2D;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace SquareHero.Hotfix.UI
{
    public class ReadyPanelData : UIPanelData
    {
        public int RoleIndex;
        public int MapId = 0;
        public long MapKey;
        public int GameMode = 2;
        public float PvPCost;
        public long RoomId;
    }

    public partial class ReadyPanel : UIPanel
    {
        private PropsConfigTable _propConfigTable;
        private AssetConfigTable _assetConfigTable;
        private SpriteAtlas _propsGrade;
        private SpriteAtlas _props;
        private SpriteAtlas roleAtlas;
        private SpriteAtlas _mapAtlas;
        private MainModel _mainModel;
        private int _selectedTrack = -1;
        private int _skinId;
        private int _propId;

        private float _timeOffset;

        // private int _leftTime = 20;
        private TweenerCore<Color, Color, ColorOptions> tweenerCore;
        private BindableProperty<int> currentRoleIndex = new BindableProperty<int>(-1);
        private BindableProperty<int> currentPropsIndex = new BindableProperty<int>(-1);
        private Sequence arrowSequence;
        private int roleListIndex;
        private int propsListIndex;
        private MapData _mapData;

        private List<ItemConfig> propsConfigs;

        private List<RoleData> _roleDatas;

        private int _leftTime;

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as ReadyPanelData ?? new ReadyPanelData();
            // please add init code here
            _mainModel = MainController.Instance.GetArchitecture().GetModel<MainModel>();
            _propConfigTable = ExcelConfig.PropConfigTable;
            _assetConfigTable = ExcelConfig.AssetConfigTable;


            RolesScrollView.OnCreate += onCreateRoleViewsHolder;
            RolesScrollView.OnUpdate += (viewsHolder => { OnUpdateRoleItem(viewsHolder as RoleViewsHolder); });

            PropsList.OnCreate += onCreatePropsViewsHolder;
            PropsList.OnUpdate += (viewsHolder => { OnUpdatePropsItem(viewsHolder as ReadyPropViewsHolder); });
            TrackAddListener();
            currentRoleIndex.Register(OnSelectRole);


            RoleLeftArrow.onClick.AddListener(() =>
            {
                roleListIndex--;
                roleListIndex = Mathf.Max(0, roleListIndex);
                RolesScrollView.SmoothScrollTo(roleListIndex, 0.2f);
            });

            RoleRightArrow.onClick.AddListener(() =>
            {
                roleListIndex++;
                roleListIndex = Mathf.Min(roleListIndex, RolesScrollView.GetItemsCount());
                RolesScrollView.SmoothScrollTo(roleListIndex, 0.2f);
            });


            PropsLeftArrow.onClick.AddListener(() =>
            {
                propsListIndex--;
                propsListIndex = Mathf.Max(0, propsListIndex);
                PropsList.SmoothScrollTo(propsListIndex, 0.2f);
            });

            PropsRightArrow.onClick.AddListener(() =>
            {
                propsListIndex++;
                propsListIndex = Mathf.Min(propsListIndex, PropsList.GetItemsCount());
                PropsList.SmoothScrollTo(propsListIndex, 0.2f);
            });

            // StartBtn.onClick.AddListener(StartMatch);

            BackBtn.onClick.AddListener(() =>
            {
                var maskPanel = UIKit.GetPanel<MaskPanel>();
                maskPanel.FadeIn(() =>
                {
                    UIKit.ShowPanel<MainPanel>();
                    UIKit.ClosePanel<ReadyPanel>();
                    maskPanel.FadeOut();
                });
            });

            LogKit.I($"Ready Panel, gamemode : {mData.GameMode}, MapId : {mData.MapId} ");
            _roleDatas = new List<RoleData>();
            if (mData.GameMode == 1)
            {
                for (int i = 0; i < _mainModel.RoleDatas.Count; i++)
                {
                    if (_mainModel.RoleDatas[i].Energy > 1)
                    {
                        _roleDatas.Add(_mainModel.RoleDatas[i]);
                    }
                }
            }
            else
            {
                _roleDatas.AddRange(_mainModel.RoleDatas);
            }
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            propsConfigs = new List<ItemConfig>();
            for (int i = 0; i < ExcelConfig.ShopGoodsTable.Data.Count; i++)
            {
                var shopGoods = ExcelConfig.ShopGoodsTable.Data[i];

                var assetNumber = _mainModel.Bag.GetAssetNumber(shopGoods.GoodsArg);

                if (assetNumber > 0)
                {
                    propsConfigs.Add(_mainModel.Bag.PlayerAssetsMap[shopGoods.GoodsArg].ItemConfig);
                }
            }


            Loading();
            CoinBox.Refresh();
            _timeOffset = 1f;

            arrowSequence = DOTween.Sequence();

            arrowSequence.Append(RoleRightArrow.GetComponent<RectTransform>()
                .DOAnchoredMove(new Vector3(16, 0, 0), 0.8f, true));
            arrowSequence.Insert(0,
                RoleLeftArrow.GetComponent<RectTransform>().DOAnchoredMove(new Vector3(-16, 0, 0), 0.8f, true));
            arrowSequence.Insert(0,
                PropsRightArrow.GetComponent<RectTransform>().DOAnchoredMove(new Vector3(16, 0, 0), 0.8f, true));
            arrowSequence.Insert(0,
                PropsLeftArrow.GetComponent<RectTransform>().DOAnchoredMove(new Vector3(-16, 0, 0), 0.8f, true));

            arrowSequence.SetLoops(-1, LoopType.Yoyo);
            arrowSequence.Play();


            // SHNetManager.RegisterHandler<RacingTrackSelectMessage>(KeepOpCode.OP_RacingTrackSelectMessage, OnTrackSelect);
        }


        protected void Loading()
        {
            // Hotfix.Global.Instance.LoadingScreen.ShowLoading();
            UIKit.ShowPanel<LoadingPanel>();
            Loading loading = new Loading();

            LoadAssetNode<SpriteAtlas> propGrade = new LoadAssetNode<SpriteAtlas>(ConstValue.AtlasLocation.PropsGrade);
            LoadAssetNode<SpriteAtlas> prop = new LoadAssetNode<SpriteAtlas>(ConstValue.AtlasLocation.Props);
            LoadAssetNode<SpriteAtlas> character = new LoadAssetNode<SpriteAtlas>(ConstValue.AtlasLocation.Character);
            LoadAssetNode<SpriteAtlas> mapAtlasNode = new LoadAssetNode<SpriteAtlas>(ConstValue.AtlasLocation.Map);
            LoadAssetNode<MapData> loadMapDataNode = new LoadAssetNode<MapData>($"Map_{mData.MapId}");
            propGrade.CompletHandler += () => { _propsGrade = propGrade.GetAsset(); };


            prop.CompletHandler += () => { _props = prop.GetAsset(); };

            loading.AddNode(propGrade);
            loading.AddNode(prop);
            loading.AddNode(character);
            loading.AddNode(loadMapDataNode);
            loading.AddNode(mapAtlasNode);

            loading.OnCompleted += () =>
            {
                roleAtlas = character.GetAsset();
                _props = prop.GetAsset();
                _propsGrade = propGrade.GetAsset();
                _mapAtlas = mapAtlasNode.GetAsset();
                _mapData = loadMapDataNode.GetAsset();

                // Hotfix.Global.Instance.LoadingScreen.HideLoading();
                FillData();
            };

            loading.Start();
        }

        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.B))
            // {
            //     mData.MapId++;
            //     Loading loading = new Loading();
            //     
            //     LoadAssetNode<MapData> loadMapDataNode = new LoadAssetNode<MapData>($"Map_{mData.MapId}");
            //     loading.AddNode(loadMapDataNode);
            //     loading.OnCompleted += () =>
            //     {
            //         _mapData = loadMapDataNode.GetAsset();
            //     
            //         // Hotfix.Global.Instance.LoadingScreen.HideLoading();
            //         for (int i = 0; i < Map.transform.childCount; i++)
            //         {
            //             var child = Map.GetChild(i);
            //
            //             if (child.name != "Tile")
            //             {
            //                 GameObject.Destroy(child.gameObject);
            //             }
            //         }
            //         CreateSmallMap();
            //         
            //     };
            //
            //     loading.Start();
            // }
        }

        protected void FillData()
        {
            CreateSmallMap();
            RolesScrollView.ResetItems(_roleDatas.Count);
            PropsList.ResetItems(propsConfigs.Count + 1);
            OnSelctProps(0);
            currentRoleIndex.Value = mData.RoleIndex;
            RolesScrollView.ScrollTo(mData.RoleIndex);
            roleListIndex = mData.RoleIndex;
            var itemViewsHolder = RolesScrollView.GetItemViewsHolder(0) as RoleViewsHolder;
            UIKit.HidePanel<LoadingPanel>();
            // itemViewsHolder.RoleItem.RoleItemToggle.isOn = true;
        }

        public void StartMatch()
        {
            if (mData.GameMode == 2)
            {
                if (_mainModel.Bag.Coin < 1)
                {
                    ToastManager.Instance.ToastMsg("Dont have enougn token");
                    return;
                }
            }
            else
            {
                if (_roleDatas[currentRoleIndex.Value].Energy < 1)
                {
                    ToastManager.Instance.ToastMsg("Dont have enougn enerhy");
                    return;
                }
            }

            int propsId = 0;
            if (currentPropsIndex > 0)
            {
                var shopGoods = propsConfigs[currentPropsIndex - 1];
                propsId = shopGoods.Id;
                var assetNumber = _mainModel.Bag.GetAssetNumber(propsId);
                if (assetNumber <= 0)
                {
                    propsId = 0;
                }
            }

            UIKit.OpenPanelAsync<WaitingPanel>();
            Web3.Instance.StartMatch(new MartchInfo()
                { GameMode = mData.GameMode, RaceMapKey = mData.MapKey, RoleToken = _roleDatas[currentRoleIndex.Value].NftID, PropsToken = propsId, PvPCost = mData.PvPCost });
        }


        protected void CreateSmallMap()
        {
            var tileObj = Map.Find("Tile");

            bool hasCliff = _mapData.TileDatas.Find(tileData => { return tileData.tileType == TileType.Cliff; }) !=
                            null;
            bool hasFlight = _mapData.TileDatas.Find(tileData => { return tileData.tileType == TileType.Flight; }) !=
                             null;

            float startX = 0;
            float startY = Map.rect.height / 2;


            if (hasCliff)
            {
                startY -= 39;
            }

            //计算整个小地图的长度
            float length = 0;
            for (int i = 0; i < _mapData.TileDatas.Count; i++)
            {
                var tileData = _mapData.TileDatas[i];
                var tileType = tileData.tileType;
                var tilePosition = (int)tileData.tilePosition;
                if (tileType == TileType.Cliff && tilePosition != 0)
                {
                    continue;
                }

                if (tileData.tileType == TileType.Flight && tileData.tilePosition == TilePosition.Middle)
                {
                    length += 80;
                    continue;
                }

                var spriteName = $"{tileType.ToString()}_{tilePosition}_1";
                var sprite = _mapAtlas.GetSprite(spriteName);
                length += sprite.rect.width / 2;
            }

            float scale = 1;
            if (length > (Map.rect.width - 160))
            {
                scale = (Map.rect.width - 160) / length;
            }

            startX += (Map.rect.width - length) / 2;

            float currentX = startX;
            float currentY = startY;

            for (int i = 0; i < _mapData.TileDatas.Count; i++)
            {
                var tileData = _mapData.TileDatas[i];
                if (tileData.tileType == TileType.Cliff)
                {
                    if (tileData.tilePosition == TilePosition.Start)
                    {
                        currentY += 47;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (tileData.tileType == TileType.Flight && tileData.tilePosition == TilePosition.Middle)
                {
                    currentX += 80;
                    currentY -= 40;
                    continue;
                }

                var instantiate = Instantiate(tileObj, Map).GetComponent<RectTransform>();
                CreateSmallMapTile(instantiate, tileData, currentX, currentY);
                instantiate.gameObject.SetActive(true);
                currentX += instantiate.sizeDelta.x;
                if (tileData.tileType == TileType.Cliff)
                {
                    currentY += 47;
                }
            }

            Map.localScale = Vector3.one * scale;
            tileObj.gameObject.SetActive(false);
        }


        protected void CreateSmallMapTile(RectTransform tileTransform, SquareHero.Hotfix.Map.TileData tileData, float x,
            float y)
        {
            var tileType = tileData.tileType;

            var tilePosition = (int)tileData.tilePosition;

            var spriteName = $"{tileType.ToString()}_{tilePosition}_1";
            var sprite = _mapAtlas.GetSprite(spriteName);
            if (sprite == null)
            {
                LogKit.E($"{spriteName} not found");
            }

            var image = tileTransform.GetComponent<Image>();

            image.sprite = sprite;

            tileTransform.sizeDelta = new Vector2(sprite.rect.width / 2, sprite.rect.height / 2);
            tileTransform.anchoredPosition = new Vector2(x, y);
        }

        protected void OnSelectRole(int index)
        {
            for (int i = 0; i < RolesScrollView.GetItemsCount(); i++)
            {
                var item = RolesScrollView.GetItemViewsHolderIfVisible(i) as RoleViewsHolder;
                if (item != null)
                {
                    item.RoleItem.Selected.gameObject.SetActive(index == i);
                }
            }

            var roleData = _roleDatas[index];
            // var enumerator = HttpHelper.PostJson(GameUrlConstValue.SelectRole.Url(),
            //     new SelectRole(){nft_id = roleData.NftID, room_id = mData.RoomId}.ToJson(), null);
            // StartCoroutine(enumerator);
            var viewsHolder = RolesScrollView.GetItemViewsHolder(index) as RoleViewsHolder;
            Role.sprite = viewsHolder.RoleItem.Icon.sprite;

            LevelNum.text = roleData.Level.ToString();
            RunSpeed.text = roleData.GetRunAttributeValue().ToString();
            SwimSpeed.text = roleData.GetSwimAttributeValue().ToString();
            ClimbSpeed.text = roleData.GetClimbAttributeValue().ToString();
            FlySpeed.text = roleData.GetFlyAttributeValue().ToString();

            var roleTalent = ExcelConfig.RoleTalentTable.Data.Find(config => { return config.Id == roleData.TalentId; });

            if (roleTalent == null)
            {
                TalentGradeBg.gameObject.SetActive(false);
            }
            else
            {
                TalentGradeBg.gameObject.SetActive(true);
                TalentGradeBg.sprite = roleAtlas.GetSprite($"Talent_Grade_{roleTalent.Grade}");
                TalentIcon.sprite = roleAtlas.GetSprite($"AttriType_{roleTalent.AttriType}");
            }

            var skinConfig = ExcelConfig.SkinConfigTable.Data.Find(config => { return config.Name == roleData.Character; });

            var assetConfig = ExcelConfig.AssetConfigTable.Data.Find(config => { return config.Id == skinConfig.AssetId; });
            _skinId = skinConfig.Id;
            // this.PostJson(GameUrlConstValue.SelectRole.Url(), new SelectRole() { nft_id = roleData.NftID }.ToJson(),
            //     result =>
            //     {
            //         var response = JsonUtil.FromJson<SelectPropsResponse>(result);
            //
            //         if (response.code == 0)
            //         {
            //
            //         }
            //     });
        }

        private string GetAttributesValue(List<RoleAttribute> attributes, AttributesType attributesType)
        {
            string value = (attributes[(int)attributesType].AttriValue +
                            attributes[(int)attributesType].TalentValue).ToString();

            // if (attributes[(int)attributesType].TalentValue > 0)
            // {
            //     value += $"<color=#D2EF12>(+{attributes[(int)attributesType].TalentValue})</color>";
            // }

            return value;
        }

        protected SHItemViewsHolder onCreateRoleViewsHolder(int index, GameObject prefab)
        {
            RoleViewsHolder viewsHolder = new RoleViewsHolder();

            return viewsHolder;
        }

        protected SHItemViewsHolder onCreatePropsViewsHolder(int index, GameObject prefab)
        {
            ReadyPropViewsHolder viewsHolder = new ReadyPropViewsHolder();

            return viewsHolder;
        }

        protected void OnUpdatePropsItem(ReadyPropViewsHolder viewsHolder)
        {
            var index = viewsHolder.ItemIndex;
            LogKit.I("OnUpdatePropsItem - " + index);
            if (index > 0)
            {
                var itemConfig = propsConfigs[index - 1];

                var shopGoods = ExcelConfig.ShopGoodsTable.Data.Find(config => { return config.GoodsArg == itemConfig.Id; });


                viewsHolder.PropsItem.Background.color = new Color(1, 1, 1, 0);
                viewsHolder.PropsItem.Icon.sprite = _props.GetSprite(itemConfig.IconSmall);
                viewsHolder.PropsItem.Icon.gameObject.SetActive(true);
                viewsHolder.PropsItem.Checkmark.gameObject.SetActive(index == currentPropsIndex.Value);

                viewsHolder.PropsItem.Views.onClick.RemoveAllListeners();

                viewsHolder.PropsItem.Views.onClick.AddListener(() =>
                {
                    AudioKit.PlaySound(SoundName.clickbutton.ToString());
                    OnSelctProps(index);
                });
                var amout = _mainModel.Bag.GetAssetNumber(itemConfig.Id);
                var has = amout > 0;
                viewsHolder.PropsItem.Mask.SetActive(false);
                viewsHolder.PropsItem.PropsCount.gameObject.SetActive(true);
                viewsHolder.PropsItem.Count.text = amout.ToString();
                viewsHolder.PropsItem.Price.text = shopGoods.ConsumeArg2.ToString("0.0");
                viewsHolder.PropsItem.Coin.gameObject.SetActive(false);
                viewsHolder.PropsItem.Buy.gameObject.SetActive(false);
                viewsHolder.PropsItem.Buy.onClick.RemoveAllListeners();
                viewsHolder.PropsItem.Buy.onClick.AddListener(() => { BuyProps(itemConfig, shopGoods); });
            }
            else
            {
                viewsHolder.PropsItem.Mask.SetActive(false);
                viewsHolder.PropsItem.Background.sprite = _props.GetSprite("Props_null");
                viewsHolder.PropsItem.Icon.gameObject.SetActive(false);
                viewsHolder.PropsItem.Checkmark.gameObject.SetActive(index == currentPropsIndex.Value);
                viewsHolder.PropsItem.Views.onClick.RemoveAllListeners();
                viewsHolder.PropsItem.Views.onClick.AddListener(() =>
                {
                    AudioKit.PlaySound(SoundName.clickbutton.ToString());
                    OnSelctProps(index);
                });

                viewsHolder.PropsItem.PropsCount.gameObject.SetActive(false);
                viewsHolder.PropsItem.Coin.gameObject.SetActive(false);
                viewsHolder.PropsItem.Buy.gameObject.SetActive(false);
            }
        }

        protected void BuyProps(ItemConfig itemConfig, ShopGoods shopGoods)
        {
            UIKit.OpenPanelAsync<WaitingPanel>(panel =>
            {
                int amount = 1;
                Web3.Instance.BuyGoods(new OrderInfo() { GoodsId = itemConfig.Id, Amount = amount, Price = shopGoods.ConsumeArg2 });
            });


            // PropsData propsData = PropsDatas[index];

            // this.PostJson(GameUrlConstValue.Buy.Url(),
            //     new BuyMessage() { goods_item_id = itemConfig.Id, amount = 1 }.ToJson(), result =>
            //     {
            //         var buyMessageResponse = JsonUtil.FromJson<BuyMessageResponse>(result);
            //         if (buyMessageResponse.code == 0)
            //         {
            //             propsData.amount += 1;
            //
            //
            //             // PropsList.Refresh();
            //
            //             var viewsHolder = PropsList.GetItemViewsHolderIfVisible(index) as ReadyPropViewsHolder;
            //
            //             if (viewsHolder != null)
            //             {
            //                 OnUpdatePropsItem(viewsHolder);
            //             }
            //
            //             _mainModel.Bag.AddAssetNumber(ConstValue.TokenId, propsData.price_amount * -1);
            //             CoinBox.Refresh();
            //         }
            //     });
        }

        public void OnBuyGoods(OrderInfo orderInfo)
        {
            var waitingPanel = UIKit.GetPanel<WaitingPanel>();

            if (waitingPanel == null)
            {
                UIKit.OpenPanelAsync<WaitingPanel>();
            }

            {
                UIKit.ShowPanel<WaitingPanel>();
            }
            StartCoroutine(CheckOrder(orderInfo));
        }

        private IEnumerator CheckOrder(OrderInfo orderInfo)
        {
            int currentNum = (int)_mainModel.Bag.GetAssetNumber((int)orderInfo.GoodsId);
            bool checkOrder = false;
            while (true)
            {
                yield return new WaitForSeconds(3);

                yield return HttpHelper.Get(GameUrlConstValue.GetAsset.Url(), result =>
                {
                    var playAssetResponse = JsonUtil.FromJson<PlayerassetsResponse>(result);
                    TypeEventSystem.Global.Send(new SystemEvents.UpdatePlayerAssets()
                    {
                        Response = playAssetResponse
                    });

                    int orderNum = (int)_mainModel.Bag.GetAssetNumber((int)orderInfo.GoodsId);

                    if (orderNum - currentNum == orderInfo.Amount)
                    {
                        checkOrder = true;
                    }
                });

                if (checkOrder)
                {
                    break;
                }
            }

            UIKit.HidePanel<WaitingPanel>();

            // Web3.Instance.GetBalance();

            PropsList.Refresh();
        }

        protected void OnSelctProps(int index)
        {
            if (index == currentPropsIndex.Value)
            {
                return;
            }

            currentPropsIndex.Value = index;

            for (int i = 0; i < PropsList.GetItemsCount(); i++)
            {
                var viewsHolder = PropsList.GetItemViewsHolderIfVisible(i) as ReadyPropViewsHolder;
                if (viewsHolder != null)
                {
                    viewsHolder.PropsItem.Checkmark.gameObject.SetActive(index == i);
                }
            }

            if (index > 0)
            {
                // var shopGoods = ExcelConfig.ShopGoodsTable.Data[index - 1];

                var itemConfig = propsConfigs[index - 1];
                // var enumerator = HttpHelper.PostJson(GameUrlConstValue.SelectProps.Url(),
                //     new SelectProps() { item_id = itemConfig.Id, room_id = mData.RoomId}.ToJson(), null);
                // StartCoroutine(enumerator);
                var propsConfig = _propConfigTable.Data.Find(config => { return config.Id == itemConfig.Id; });

                var assetConfig = _assetConfigTable.Data.Find(config => { return config.Id == propsConfig.AssetId; });

                var gradeSprite = _propsGrade.GetSprite($"Prop_Grade_{itemConfig.Grade}");
                var propSprite = _props.GetSprite(assetConfig.BigIcon);
                PropGrade.sprite = gradeSprite;
                PropIcon.sprite = propSprite;
                PropIcon.gameObject.SetActive(true);

                PropName.text = propsConfig.Name;
                PropDesc.text = itemConfig.Explain;
                SpeedIncrease.text = propsConfig.SpeedIncrease + "%";
                if (propsConfig.Distance == -1)
                {
                    AffectTime.text = "<size=30>∞</size>";
                }
                else
                {
                    AffectTime.text = propsConfig.Distance.ToString();
                }

                if (propsConfig.UsageTimes == -1)
                {
                    UseTimes.text = "<size=30>∞</size>";
                }
                else
                {
                    UseTimes.text = propsConfig.UsageTimes.ToString();
                }
            }
            else
            {
                var propSprite = _props.GetSprite("Props_null");
                PropGrade.sprite = propSprite;
                PropIcon.gameObject.SetActive(false);
                PropName.text = "";
                PropDesc.text = "";
                SpeedIncrease.text = "<size=30>∞</size>";
                UseTimes.text = "<size=30>∞</size>";
                AffectTime.text = "<size=30>∞</size>";
            }
        }

        protected void OnUpdateRoleItem(RoleViewsHolder viewsHolder)
        {
            var index = viewsHolder.ItemIndex;

            var roleData = _roleDatas[index];

            var skinConfig = ExcelConfig.SkinConfigTable.Data.Find(config => { return config.Name == roleData.Character; });

            var assetConfig = ExcelConfig.AssetConfigTable.Data.Find(config => { return config.Id == skinConfig.AssetId; });

            var sprite = roleAtlas.GetSprite(assetConfig.BigIcon);
            viewsHolder.RoleItem.Icon.sprite = sprite;
            bool isOn = index == currentRoleIndex.Value;
            viewsHolder.RoleItem.Selected.gameObject.SetActive(isOn);
            viewsHolder.RoleItem.Views.onClick.RemoveAllListeners();
            viewsHolder.RoleItem.Views.onClick.AddListener(() =>
            {
                AudioKit.PlaySound(SoundName.clickbutton.ToString());
                currentRoleIndex.Value = index;
            });
            viewsHolder.RoleItem.Energy.text = roleData.Energy.ToString();
            // viewsHolder.RoleItem.RoleItemToggle.isOn = isOn;
            // viewsHolder.RoleItem.RoleItemToggle.onValueChanged.RemoveAllListeners();
            // viewsHolder.RoleItem.RoleItemToggle.onValueChanged.AddListener(isOn =>
            // {
            //     if (isOn)
            //     {
            //         AudioKit.PlaySound(SoundName.clickbutton.ToString());
            //         OnSelectRole(index);
            //     }
            // });
        }

        protected override void OnShow()
        {
            Web3.Instance.BuyGoodsHandler += OnBuyGoods;

            _leftTime = 15;
            StartCoroutine(StartCountDown());
            // StartCountDown.text = $"({_leftTime}s)";
            tweenerCore = SelectTrackTip.DOFade(0, 1f).SetLoops(-1, LoopType.Yoyo).OnKill(() => { SelectTrackTip.alpha = 0f; });
            RunxNetManager.RegisterHandler<RacingGameStartMessage>(KeepOpCode.OP_RacingGameStart, EnterGameScene);

            RunxNetManager.RegisterHandler<R2C_StartMatchRacing>(KeepOpCode.OP_R2C_StartMatchRacing, OpenMatch);
            RunxNetManager.RegisterHandler<MatchProgressMessage>(KeepOpCode.OP_MatchProgressMessage, MatchProgressMessage);
        }


        protected IEnumerator StartCountDown()
        {
            while (_leftTime > 0)
            {
                CountDown.text = _leftTime + "S";
                yield return new WaitForSeconds(1);
                _leftTime -= 1;
                if (gameObject == null || !gameObject.activeSelf)
                {
                    yield break;
                }
            }

            UIKit.OpenPanelAsync<LoadingPanel>(panel => { EnterGameScene(); });
        }

        protected override void OnHide()
        {
            Web3.Instance.BuyGoodsHandler -= OnBuyGoods;
            // SHNetManager.UnRegisterHandler<R2C_StartMatchRacing>(KeepOpCode.OP_R2C_StartMatchRacing, OnStartMatch);
            // SHNetManager.UnRegisterHandler<MatchProgressMessage>(KeepOpCode.OP_MatchProgressMessage, ToMatch);
            // SHNetManager.UnRegisterHandler<RacingTrackSelectMessage>(KeepOpCode.OP_RacingTrackSelectMessage,
            //     OnTrackSelect);

            RunxNetManager.UnRegisterHandler<RacingGameStartMessage>(KeepOpCode.OP_RacingGameStart, EnterGameScene);
            RunxNetManager.UnRegisterHandler<R2C_StartMatchRacing>(KeepOpCode.OP_R2C_StartMatchRacing, OpenMatch);
            RunxNetManager.UnRegisterHandler<MatchProgressMessage>(KeepOpCode.OP_MatchProgressMessage, MatchProgressMessage);
            currentRoleIndex.UnRegister(OnSelectRole);
        }

        protected override void OnClose()
        {
            _roleDatas.Clear();
            _roleDatas = null;
        }


        private void OnSkinSelect(int index)
        {
            _skinId = 5 - index;
            ResourceManager.Instance.GetAssetAsync<GameObject>($"Skin_C1_0{_skinId}", asset =>
            {
                var parent = PlayerContainer.Instance.transform;

                if (parent.childCount != 0)
                {
                    for (int i = 0; i < parent.childCount; i++)
                    {
                        Destroy(parent.GetChild(i).gameObject);
                    }
                }

                Instantiate(asset, parent);
            });
        }


        private void TrackAddListener()
        {
            var buttons = Tracks.GetComponentsInChildren<Button>();

            for (int i = 0; i < buttons.Length; i++)
            {
                int temp = i;
                buttons[i].onClick.AddListener(() => { OnTrackSelect(temp); });
            }
        }

        private void OnTrackSelect(RacingTrackSelectMessage evt)
        {
            if (evt.UserId != _mainModel.Player.UserId)
            {
                var evtTrackId = evt.TrackId - 1;
                var child = Tracks.transform.GetChild((int)evtTrackId);
                child.GetComponent<Button>().interactable = false;
                child.transform.Find("Background/Checkmark").gameObject.SetActive(true);
            }
        }

        private void OnTrackSelect(int index)
        {
            AudioKit.PlaySound(SoundName.clickbutton.ToString());
            SelectTrack selectTrack = new SelectTrack()
            {
                // room_id = mData.RoomId,
                // track_id = index + 1
            };

            this.PostJson(GameUrlConstValue.SelectTrack.Url(), JsonUtil.ToJson(selectTrack), result =>
            {
                var baseResponse = JsonUtil.FromJson<BaseResponse>(result);
                if (baseResponse.code == 0)
                {
                    if (_selectedTrack != -1)
                    {
                        var preChild = Tracks.transform.GetChild(_selectedTrack);
                        preChild.GetComponent<Button>().interactable = true;
                        var checkmask = preChild.transform.Find("Background/Checkmark");
                        checkmask.gameObject.SetActive(false);
                    }

                    _selectedTrack = index;
                    var child = Tracks.transform.GetChild(index);
                    tweenerCore.Kill();
                    child.GetComponent<Button>().interactable = false;
                    child.transform.Find("Background/Checkmark").gameObject.SetActive(true);
                }
            });
        }


        private void EnterGame()
        {
            UIKit.ShowPanel<LoadingPanel>();
            // Hotfix.Global.Instance.LoadingScreen.ShowLoading();
            // UIKit.CloseAllOtherPanel<ReadyPanel>();
            LogKit.I("EnterGameScene");
        }


        private void OpenMatch(R2C_StartMatchRacing evt)
        {
            LogKit.I($"Receive Msg {evt} duplicate");
        }

        protected void MatchProgressMessage(MatchProgressMessage evt)
        {
            LogKit.I($"Receive Msg {evt} duplicate");
        }

        protected void ToMatch(MatchProgressMessage evt)
        {
            UIKit.OpenPanelAsync<MatchingPanel>(panel =>
            {
                panel.MaxNum.text = evt.target.ToString();
                panel.MatchedNum.text = evt.current.ToString();
                // Hotfix.Global.Instance.LoadingScreen.HideLoading();
                // UIKit.HidePanel<MainPanel>();
                UIKit.HidePanel<ReadyPanel>();
                UIKit.HidePanel<WaitingPanel>();
            });
        }

        private void EnterGameScene(RacingGameStartMessage evt)
        {
            LogKit.I("EnterGameScene");
            // AudioKit.StopMusic();

            long localPlayerId = MainController.Instance.GetArchitecture().GetModel<MainModel>().Player.UserId;
            var loadingPanel = UIKit.GetPanel<LoadingPanel>();
            if (loadingPanel.State == PanelState.Hide)
            {
                UIKit.ShowPanel<LoadingPanel>();
            }

            Global.Instance.VideoPlayer.Pause();
            ResourceManager.Instance.LoadSceneAsync("Scene_Level1", () =>
            {
                LogKit.I("EnterGameScene Scene_Level1Start loaded");

                UIKit.OpenPanelAsync<LevelPanel>(panel =>
                {
                    UIKit.ClosePanel<MainPanel>();
                    UIKit.ClosePanel<ReadyPanel>();
                    UIKit.ClosePanel<MatchingPanel>();
                    UIKit.ClosePanel<VideoPanel>();
                }, UILevel.Common, new LevelPanelData()
                {
                    RoomId = evt.RoomId,
                    UserId = localPlayerId,
                    MapId = mData.MapId
                });
            });
        }

        private void EnterGameScene()
        {
            LogKit.I("EnterGameScene");
            // AudioKit.StopMusic();
            long localPlayerId = MainController.Instance.GetArchitecture().GetModel<MainModel>().Player.UserId;
            var loadingPanel = UIKit.GetPanel<LoadingPanel>();
            if (loadingPanel.State == PanelState.Hide)
            {
                UIKit.ShowPanel<LoadingPanel>();
            }
            
            // Global.Instance.VideoPlayer.Pause();
            ResourceManager.Instance.LoadSceneAsync("Scene_Level1", () =>
            {
                LogKit.I("EnterGameScene Scene_Level1Start loaded");

                
                RandomRoom();
                
                UIKit.OpenPanelAsync<LevelPanel>(panel =>
                {
                    UIKit.ClosePanel<MainPanel>();
                    UIKit.ClosePanel<ReadyPanel>();
                    UIKit.ClosePanel<MatchingPanel>();
                    UIKit.ClosePanel<VideoPanel>();
                }, UILevel.Common, new LevelPanelData()
                {
                    RoomId = mData.RoomId,
                    UserId = localPlayerId,
                    MapId = mData.MapId
                });
            });
        }

        private void RandomRoom()
        {
            List<int> trackId = new List<int>() { 1, 2, 3, 4, 5 };
            List<int> itemId = new List<int>() { 11001, 11002, 12001, 12002, 13001,13002,14001,14002,0,0,0,0,0,0,0 };
            RoomManager.Instance.RoomInfo.game_type = 2;
            RoomManager.Instance.RoomInfo.room_id = mData.RoomId;
            RoomManager.Instance.RoomInfo.player_info_list = new List<PlayerInfo>();
            var mainModel = MainController.Instance.GetArchitecture().GetModel<MainModel>();
            var player = mainModel.Player;
            int trackIndex = Random.Range(0, trackId.Count);

            int selecteditemId = 0;
            _mainModel.Bag.AddAssetNumber(ConstValue.TokenId, -1 * TokenHelper.TokenAccuracy);
            if (currentPropsIndex.Value > 0)
            {
                // var shopGoods = ExcelConfig.ShopGoodsTable.Data[index - 1];

                var itemConfig = propsConfigs[currentPropsIndex.Value - 1];
                selecteditemId = itemConfig.Id;
                
                _mainModel.Bag.AddAssetNumber(selecteditemId, -1);
            }

            PlayerInfo local = new PlayerInfo()
            {
                user_id = player.UserId,
                name = player.PlayerName,
                role_info = mainModel.RoleDatas[0],
                track_id = trackId[trackIndex],
                item_id = selecteditemId,
            };
            trackId.RemoveAt(trackIndex);
            RoomManager.Instance.RoomInfo.player_info_list.Add(local);
            
            trackIndex = Random.Range(0, trackId.Count);
            RoomManager.Instance.RoomInfo.player_info_list.Add(new PlayerInfo()
            {
                user_id = 2,
                name = "0x2f7857EB836227e80c6D58ED0A71FC6345Ba90B6",
                role_info = NFTHepler.RadomNft(Random.Range(0, 10)),
                track_id = trackId[trackIndex],
                item_id = itemId[Random.Range(0, itemId.Count)],
            });
            trackId.RemoveAt(trackIndex);
            
            trackIndex = Random.Range(0, trackId.Count);
            RoomManager.Instance.RoomInfo.player_info_list.Add(new PlayerInfo()
            {
                user_id = 3,
                name = "0x163B10aBC2913c0b4b20d643C69349f9a0e0f711",
                role_info = NFTHepler.RadomNft(Random.Range(0, 10)),
                track_id = trackId[trackIndex],
                item_id = itemId[Random.Range(0, itemId.Count)],
            });
            trackId.RemoveAt(trackIndex);
            
            trackIndex = Random.Range(0, trackId.Count);
            RoomManager.Instance.RoomInfo.player_info_list.Add(new PlayerInfo()
            {
                user_id = 4,
                name = "0x849C746C69FfA34217E0F0865e6bBA9CdFffB9eF",
                role_info = NFTHepler.RadomNft(Random.Range(0, 10)),
                track_id = trackId[trackIndex],
                item_id = itemId[Random.Range(0, itemId.Count)],
            });
            trackId.RemoveAt(trackIndex);
            
            trackIndex = Random.Range(0, trackId.Count);
            RoomManager.Instance.RoomInfo.player_info_list.Add(new PlayerInfo()
            {
                user_id = 5,
                name = "0x595F9f7A572e49E8e6c748abBbfC558a19Eb915B",
                role_info = NFTHepler.RadomNft(Random.Range(0, 10)),
                track_id = trackId[trackIndex],
                item_id = itemId[Random.Range(0, itemId.Count)],
            });
            trackId.RemoveAt(trackIndex);
            
            
            MapCreater.Instance.StartCreateMap(mData.MapId);
            
        }
    }

    public class ReadyPropViewsHolder : SHItemViewsHolder
    {
        public PropsItem PropsItem;

        public override void CollectViews()
        {
            PropsItem = root.GetComponent<PropsItem>();
        }
    }
}