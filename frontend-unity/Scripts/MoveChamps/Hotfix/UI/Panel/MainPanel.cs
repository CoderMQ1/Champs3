using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using SquareHero.Core;
using SquareHero.Core.Loading;
using SquareHero.Hotfix.Events;
using SquareHero.Hotfix.Generate;
using SquareHero.Hotfix.Main;
using SquareHero.Hotfix.Model;
using SquareHero.Hotfix.Net;
using SquareHero.Hotfix.Player;
using SquareHero.Hotfix.Toast;
using TMPro;
using UnityCommon;
using UnityCommon.Util;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

namespace SquareHero.Hotfix.UI
{
    public class MainPanelData : UIPanelData
    {
        public bool AutoStart;
        public int AutoStartMode;
    }

    public partial class MainPanel : UIPanel
    {
        private bool _isMatching;
        private MainModel _mainModel;
        private SpriteAtlas RoleAtlas;
        private BindableProperty<int> currentRoleIndex = new BindableProperty<int>(-1);
        private bool _showTalentInfo;
        private Sequence arrowSequence;
        private bool _approveStatus;
        private bool _pvpApproveStatus;
        private float _pvpPrice;
        private int _gameType = 1;
        
        private bool _unApproveing;

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as MainPanelData ?? new MainPanelData();
            // please add init code here
            PlayBtn.onClick.AddListener(StartMatch);
            
            PvpPlay.onClick.AddListener(StartMatch);
            
            UserCode.gameObject.SetActive(false);

            currentRoleIndex.Register(OnSelectRole);

            // UpgradeBtn.onClick.AddListener(OpenRoleUpgradePanel);

            // BackpackBtn.onClick.AddListener(OpenBackpack);

            // StoreBtn.onClick.AddListener(OpenStorePanel);
            
            // HistoryBtn.onClick.AddListener(OpenHistorePanel);
            
            // InvitationBtn.onClick.AddListener(OpenInvitationPanel);
            
            // UnApproveBtn.onClick.AddListener(() =>
            // {
            //     ApproveGame(_gameType, false);
            // });
            
            // LuckSpinBtn.onClick.AddListener(OpenLuckSpinPanel);
            // CoinBox.ExchangeBtn.onClick.AddListener(() =>
            // {
            //     AudioKit.PlaySound(SoundName.clickbutton.ToString());
            //     UIKit.OpenPanelAsync<RechargePanel>();
            // });
            
            LeftArrow.onClick.AddListener(() =>
            {
                var value = currentRoleIndex.Value;
                value--;
                value = Mathf.Max(0, value);
                currentRoleIndex.Value = value;
                // RolesScrollView.SmoothScrollTo(listIndex, 0.2f);
            });

            RightArrow.onClick.AddListener(() =>
            {
                var value = currentRoleIndex.Value;
                value++;
                value = Mathf.Min(value, _mainModel.RoleDatas.Count - 1);
                currentRoleIndex.Value = value;
                // RolesScrollView.SmoothScrollTo(listIndex, 0.2f);
            });


            SettingBtn.onClick.AddListener(() => { UIKit.OpenPanelAsync<SettingPanel>(); });

            Manager.RegisterEvent((int)UIEvents.ID.RefreshPlayerAssets, RefreshPlayerAssets);
            Manager.RegisterEvent((int)UIEvents.ID.RefreshSelectRoleAttributes, RefreshSelectRoleAttributes);

            _mainModel = MainController.Instance.GetArchitecture().GetModel<MainModel>();
            
            
            GameModeSwith.AddListener(index =>
            {
                _gameType = index + 1;

                if (_gameType == 1)
                {
                    Pve.transform.DOScale(Vector3.one, 0.2f);
                    Pvp.transform.DOScale(Vector3.one * 0.5f, 0.2f);
                }
                else
                {
                    Pve.transform.DOScale(Vector3.one * 0.5f, 0.2f);
                    Pvp.transform.DOScale(Vector3.one, 0.2f);
                }
                
                Consume.text = $"consume <color=#D2EF12>1</color>";
                PvpConsume.text = $"consume <color=#D2EF12>{_pvpPrice.ToString("F1")}</color>";
                RefreshGameModeCost(_gameType == 2);
            });
            
            // Twitter.onClick.AddListener(() =>
            // {
            //     Application.OpenURL("https://twitter.com/runxxyz");
            // });
            //
            // Telegram.onClick.AddListener(() =>
            // {
            //     Application.OpenURL("https://t.me/runxxyz");
            // });
            //
            // ElectricityBtn.onClick.AddListener(() =>
            // {
            //     LogKit.I("ElectricityBtn Click");
            //     // Global.Instance.VideoPlayer.Play();
            //     
            //     UIKit.OpenPanelAsync<ExchangeEnergyPanel>(null, UILevel.Common, new ExchangeEnergyPanelData(){RoleToken = _mainModel.RoleDatas[currentRoleIndex.Value].NftID});
            // });
            
            //
            // UserCodeCopyBtn.onClick.AddListener(() =>
            // {
            //     SHWebGLPlugin.CopyToClipboard(_mainModel.Player.PlayerName);
            // });
            // Web3.Instance.MetaMaskHandler.RegisterHandler(MetaMaskMethodName.wallet_addEthereumChain, OnAddEthereumChain);
            // Web3.Instance.MetaMaskHandler.RegisterHandler(MetaMaskMethodName.wallet_switchEthereumChain, OnAddSwitchChain);
            
            // SHNetManager.RegisterHandler<R2C_StartMatchRacing>(KeepOpCode.OP_R2C_StartMatchRacing, OnStartMatch);
            // SHNetManager.RegisterHandler<MatchProgressMessage>(KeepOpCode.OP_MatchProgressMessage, ToMatch);
            // SHNetManager.RegisterHandler<RoomMatchSuccessMessage>(KeepOpCode.OP_RoomMatchSuccessMessage, ToReady);
        }
        
        protected override void OnOpen(IUIData uiData = null)
        {

            PetraAdapter.Instance.OnMintHander += OnMint;
            PetraAdapter.Instance.OnMatchHander += OnPetraMatch;
            
            Loading();

            arrowSequence = DOTween.Sequence();


            arrowSequence.Append(RightArrow.GetComponent<RectTransform>()
                .DOAnchoredMove(new Vector3(1, 0, 0), 0.8f, true));
            arrowSequence.Insert(0,
                LeftArrow.GetComponent<RectTransform>().DOAnchoredMove(new Vector3(-1, 0, 0), 0.8f, true));

            arrowSequence.SetLoops(-1, LoopType.Yoyo);
            arrowSequence.Play();
        }

        private void OnMint(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return;
            }
            
            var radomNft = NFTHepler.RadomNft(0);
            _mainModel.RoleDatas = new List<RoleData>();
            _mainModel.RoleDatas.Add(radomNft);
            FillData();
        }
        

        protected void Loading()
        {

            Loading loading = new Loading();
            
            LoadAssetNode<SpriteAtlas> roleAtlasNode = new LoadAssetNode<SpriteAtlas>("Atlas_Character");

            roleAtlasNode.CompletHandler += () => { RoleAtlas = roleAtlasNode.GetAsset(); };
            
            loading.AddNode(roleAtlasNode);
            loading.OnCompleted += () =>
            {
                
                if (mData.AutoStart)
                {
                    StartMatch();
                }
                else
                {
                    UIKit.HidePanel<LoadingPanel>();
                }


            };
            _pvpPrice = 1;
            _approveStatus = true;
            _pvpApproveStatus = true;
            _gameType = 2;
            Pvp.isOn = true;
            _mainModel = MainController.Instance.GetArchitecture().GetModel<MainModel>();
            _mainModel.Player.UserId = 1;


            if (_mainModel.RoleDatas == null || _mainModel.RoleDatas.Count == 0)
            {
                
                _mainModel.Bag.AddAssetNumber(ConstValue.TokenId, 100 * TokenHelper.TokenAccuracy);
            
                _mainModel.Bag.AddAssetNumber(11001, 10);
                _mainModel.Bag.AddAssetNumber(11002, 10);
                _mainModel.Bag.AddAssetNumber(12001, 10);
                _mainModel.Bag.AddAssetNumber(12002, 10);
                _mainModel.Bag.AddAssetNumber(13001, 10);
                _mainModel.Bag.AddAssetNumber(13002, 10);
                _mainModel.Bag.AddAssetNumber(14001, 10);
                _mainModel.Bag.AddAssetNumber(14002, 10);
                
                NotNftPanel.gameObject.SetActive(true);
                
                NotNftPanel.Bg.transform.localScale = Vector3.zero;
                var sequence = DOTween.Sequence();
                sequence.Append(NotNftPanel.Bg.transform.DOScale(Vector3.one * 1.2f, 0.2f));
                sequence.Append(NotNftPanel.Bg.transform.DOScale(Vector3.one, 0.2f));
                sequence.Play();
                NotNftPanel.GoBtn.onClick.AddListener(() =>
                {
#if UNITY_EDITOR
                    var radomNft = NFTHepler.RadomNft(0);
                    _mainModel.RoleDatas = new List<RoleData>();
                    _mainModel.RoleDatas.Add(radomNft);
                    FillData();
#else
                    PetraAdapter.Instance.Mint();
#endif
                });
            }
            else
            {
                loading.OnCompleted += FillData;
            }

            loading.Start();
            
        }
        
        
        

       

        protected override void OnShow()
        {
            LogKit.I("Show MAIN");
            _mainModel = MainController.Instance.GetArchitecture().GetModel<MainModel>();
            
            _mainModel.Player.PlayerName = PetraAdapter.Instance.Account;
            
            // AudioKit.PlayMusic(SoundName.MainBGM.ToString());

            PlayerName.text = _mainModel.Player.PlayerName;
            // UserCode.text = _mainModel.Player.PlayerCode;


            // Web3.Instance.ApproveGameHandler += OnApproveGame;
            // RunxNetManager.RegisterHandler<R2C_StartMatchRacing>(KeepOpCode.OP_R2C_StartMatchRacing, OnStartMatch);
            // RunxNetManager.RegisterHandler<MatchProgressMessage>(KeepOpCode.OP_MatchProgressMessage, ToMatch);
            // RunxNetManager.RegisterHandler<RoomMatchSuccessMessage>(KeepOpCode.OP_RoomMatchSuccessMessage, MactchSuccess);
        }
        
        protected override void OnHide()
        {
            Web3.Instance.ApproveGameHandler -= OnApproveGame;  

        }

        protected override void OnClose()
        {
            Manager.UnRegisterEvent((int)UIEvents.ID.RefreshPlayerAssets, RefreshPlayerAssets);
            Manager.UnRegisterEvent((int)UIEvents.ID.RefreshSelectRoleAttributes, RefreshSelectRoleAttributes);
            // SHNetManager.UnRegisterHandler<R2C_StartMatchRacing>(KeepOpCode.OP_R2C_StartMatchRacing, OnStartMatch);
            // SHNetManager.UnRegisterHandler<MatchProgressMessage>(KeepOpCode.OP_MatchProgressMessage, ToMatch);
            // SHNetManager.UnRegisterHandler<RoomMatchSuccessMessage>(KeepOpCode.OP_RoomMatchSuccessMessage, ToReady);
            RunxNetManager.UnRegisterHandler<R2C_StartMatchRacing>(KeepOpCode.OP_R2C_StartMatchRacing, OnStartMatch);
            RunxNetManager.UnRegisterHandler<MatchProgressMessage>(KeepOpCode.OP_MatchProgressMessage, ToMatch);
            RunxNetManager.UnRegisterHandler<RoomMatchSuccessMessage>(KeepOpCode.OP_RoomMatchSuccessMessage, MactchSuccess);
            // Web3.Instance.MetaMaskHandler.UnRegisterHandler(MetaMaskMethodName.wallet_addEthereumChain, OnAddEthereumChain);
            // Web3.Instance.MetaMaskHandler.UnRegisterHandler(MetaMaskMethodName.wallet_switchEthereumChain, OnAddSwitchChain);
        }


        private void SetPointEvent(TextMeshProUGUI label)
        {
            var pointArea = label.transform.Find("PointArea");
            pointArea.OnPointerEnterEvent(data =>
            {
                var find = label.transform.Find("Image");
                find.gameObject.SetActive(true);
                find.localScale = Vector3.zero;

                find.DOScale(Vector3.one, 0.2f);
            });

            pointArea.OnPointerExitEvent(data =>
            {
                var find = label.transform.Find("Image");
                find.DOScale(Vector3.zero, 0.2f).onComplete += () =>
                {
                    find.gameObject.SetActive(false);
                };
            });

            
        }
        
        private void RemovePointEvent(TextMeshProUGUI label)
        {
            var pointArea = label.transform.Find("PointArea");
            var eventTrigger = pointArea.GetComponent<OnPointerEnterEventTrigger>();

            if (eventTrigger)
            {
                eventTrigger.OnPointerEnterEvent.UnRegisterAll();
            }
        }

        private void RefreshGameModeCost(bool isPvp)
        {
            EnergyIcon.rectTransform.anchoredPosition = new Vector2(Consume.preferredWidth + 2, 0);
            CoinIcon.rectTransform.anchoredPosition = new Vector2(PvpConsume.preferredWidth + 2, 0);
            // UnApproveBtn.gameObject.SetActive(_gameType == 1 ? _approveStatus : _pvpApproveStatus);
            
            PlayBtn.gameObject.SetActive(!isPvp);
            PvpPlay.gameObject.SetActive(isPvp);
        }

        private void RefreshPlayerAssets(int id, object[] objects)
        {
            var msg = objects[0] as UIEvents.RefreshPlayerAssets;
            FillPlayerAsset(msg.Response);
        }

        private void RefreshSelectRoleAttributes(int id, object[] objects)
        {
            var index = currentRoleIndex.Value;

            var roleData = _mainModel.RoleDatas[index];
            RefreshSelectRoleAttributes(roleData);
        }


        protected void OnAddEthereumChain(string result)
        {
            Web3.Instance.TestSwitchChain();
        }
        
        protected void OnAddSwitchChain(string result)
        {
            // Web3.Instance.StartMatch(_mainModel.RoleDatas[currentRoleIndex.Value].NftID.ToString());
        }

        private IEnumerator ShowTalentInfo()
        {
            _showTalentInfo = true;

            yield return new WaitForSeconds(0.4f);

            if (_showTalentInfo)
            {
                TalentInfo.gameObject.SetActive(true);
                TalentInfo.transform.localScale = Vector3.zero;

                TalentInfo.transform.DOScale(Vector3.one, 0.4f);
            }
        }



        protected void ToReady(RoomMatchSuccessMessage evt)
        {
            UIKit.OpenPanelAsync<ReadyPanel>(panel =>
                {
                    UIKit.HidePanel<MainPanel>();
                    UIKit.ClosePanel<MatchingPanel>();
                    UIKit.HidePanel<WaitingPanel>();
                },UILevel.Common,new ReadyPanelData()
                {
                    // RoomId = evt.RoomId,
                    // ReadyTime = (int)evt.CountDown,
                    MapId = (int)evt.MapId
                }
            );
        }
        
        protected void ToReady()
        {
            if (mData != null && mData.AutoStart)
            {
                _gameType = mData.AutoStartMode;
            }
            if (_gameType == 2)
            {
                if (_mainModel.Bag.Coin < _pvpPrice)
                {
                    UIKit.HidePanel<LoadingPanel>();
                    ShowCoinNotEnougn();
                    return;
                }
            }
            else
            {
                if (_mainModel.RoleDatas[currentRoleIndex.Value].Energy < 1)
                {
                    UIKit.HidePanel<LoadingPanel>();
                    ToastManager.Instance.ToastMsg("Dont have enougn enerhy");
                    return;
                }
            }
 
            var enumerator = HttpHelper.Get(GameUrlConstValue.MapInfo.Url(), result =>
            {
                var mapInfoResponse = JsonUtil.FromJson<MapInfoResponse>(result);

                if (mapInfoResponse.code == 0)
                {
                    UIKit.OpenPanelAsync<ReadyPanel>(panel =>
                        {
                            UIKit.HidePanel<MainPanel>();
                            UIKit.HidePanel<WaitingPanel>();
                        },UILevel.Common,new ReadyPanelData()
                        {
                            MapId = mapInfoResponse.data.map_id,
                            MapKey = mapInfoResponse.data.map_key,
                            RoleIndex = currentRoleIndex.Value,
                            GameMode = _gameType,
                            PvPCost = _pvpPrice
                        }
                    );
                }
                else
                {
                    ToastManager.Instance.ToastMsg("Network Error");
                }
            });
            UIKit.OpenPanelAsync<LoadingPanel>(panel =>
            {
                StartCoroutine(enumerator);
            });
        }

        // protected SHItemViewsHolder onCreateRoleViewsHolder(int index, GameObject prefab)
        // {
        //     RoleViewsHolder viewsHolder = new RoleViewsHolder();
        //
        //     return viewsHolder;
        // }

        protected void OnSelectRole(int index)
        {
            // if (RolesScrollView.GetItemsCount() == 0)
            // {
            //     TalentGradeBg.gameObject.SetActive(false);
            //     return;
            // }

            // for (int i = 0; i < RolesScrollView.GetItemsCount(); i++)
            // {
            //     var viewsHolder = RolesScrollView.GetItemViewsHolderIfVisible(i) as RoleViewsHolder;
            //     if (viewsHolder != null)
            //     {
            //         viewsHolder.RoleItem.Checkmark.gameObject.SetActive(index == i);
            //     }
            // }

            _mainModel = MainController.Instance.GetArchitecture().GetModel<MainModel>();
            var roleData = _mainModel.RoleDatas[index];
            RefreshSelectRoleAttributes(roleData);

            var skinConfig = ExcelConfig.SkinConfigTable.Data.Find(config =>
            {
                return config.Name == roleData.Character;
            });

            var assetConfig = ExcelConfig.AssetConfigTable.Data.Find(config =>
            {
                return config.Id == skinConfig.AssetId;
            });
            
            

            ResourceManager.Instance.GetAssetAsync<GameObject>($"Skin_{assetConfig.PrefabName}", asset =>
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


        public void RefreshSelectRoleAttributes(RoleData roleData)
        {
            LevelNum.text = roleData.Level.ToString();


            ElectricityNum.text = roleData.Energy.ToString();
            
            var roleTalent = ExcelConfig.RoleTalentTable.Data.Find(config =>
            {
                return config.Id == roleData.TalentId;
            });

            var skinConfig = ExcelConfig.SkinConfigTable.Data.Find(config =>
            {
                return config.Name == roleData.Character;
            });
            
            GetAttributesValue(roleData.Attributes, AttributesType.RunSpeed, RunSpeed);
            GetAttributesValue(roleData.Attributes, AttributesType.SwimSpeed, SwimSpeed);
            GetAttributesValue(roleData.Attributes, AttributesType.ClimbSpeed, ClimbSpeed);
            GetAttributesValue(roleData.Attributes, AttributesType.FlySpeed, FlySpeed);

            RunIcon.sprite = RoleAtlas.GetSprite($"1_1");
            SwimIcon.sprite = RoleAtlas.GetSprite($"2_1");
            ClimbIcon.sprite = RoleAtlas.GetSprite($"3_1");
            FlyIcon.sprite = RoleAtlas.GetSprite($"4_1");
            RunSpeed.color = TalentHelper.TalentColor[0];
            SwimSpeed.color = TalentHelper.TalentColor[0];
            ClimbSpeed.color = TalentHelper.TalentColor[0];
            FlySpeed.color = TalentHelper.TalentColor[0];
            if (roleTalent != null)
            {
                var sprite = RoleAtlas.GetSprite($"{roleTalent.AttriType}_{roleTalent.Grade}");
                RectTransform rectTransform = null;
                switch (roleTalent.AttriType)
                {
                    case 1:
                        RunIcon.sprite = sprite;
                        rectTransform = RunIcon.GetComponent<RectTransform>();
                        
                        RunSpeed.color = TalentHelper.TalentColor[roleTalent.Grade - 1];
                        break;
                    case 2:
                        SwimIcon.sprite = sprite;
                        rectTransform = SwimIcon.GetComponent<RectTransform>();
                        SwimSpeed.color = TalentHelper.TalentColor[roleTalent.Grade - 1];
                        break;
                    case 3:
                        ClimbIcon.sprite = sprite;
                        rectTransform = ClimbIcon.GetComponent<RectTransform>();
                        ClimbSpeed.color = TalentHelper.TalentColor[roleTalent.Grade - 1];
                        break;
                    case 4:
                        FlyIcon.sprite = sprite;
                        rectTransform = FlyIcon.GetComponent<RectTransform>();
                        FlySpeed.color = TalentHelper.TalentColor[roleTalent.Grade - 1];
                        break;
                }
                
                AddtitionAttributeIcon.sprite = sprite;
                MultiplierAttributeIcon.sprite = sprite;
                
                
                TalentAddition.text = $"+{roleTalent.BaseSpeedAdd}";
                TalentMultiplier.text = $"+{roleTalent.BaseSpeedAdd}%";
                var parentPos = rectTransform.parent.GetComponent<RectTransform>().anchoredPosition;
                TalentInfo.rectTransform.anchoredPosition = new Vector2(parentPos.x - rectTransform.anchoredPosition.x,
                    parentPos.y + rectTransform.anchoredPosition.y);
                
                TalentInfo.rectTransform.localScale = Vector3.zero;
                rectTransform.OnPointerEnterEvent(data =>
                {
                    TalentInfo.gameObject.SetActive(true);
                    TalentInfo.rectTransform.DOScale(Vector3.one, 0.2f);
                });
                
                rectTransform.OnPointerExitEvent(data =>
                {
                    TalentInfo.rectTransform.DOScale(Vector3.zero, 0.2f).onComplete += () =>
                    {
                        TalentInfo.gameObject.SetActive(false);
                    };
                });
            }
        }

        private void GetAttributesValue(List<RoleAttribute> attributes, AttributesType attributesType, TextMeshProUGUI label)
        {
            var roleAttribute = attributes.Find(attribute =>
            {
                return attribute.AttriType == (int)attributesType;
            });
            
            string value = (roleAttribute.AttriValue + roleAttribute.TalentValue).ToString();

            label.text = value;
            
            
            if (roleAttribute.TalentValue > 0)
            {
                value = roleAttribute.AttriValue + $"<color=#D2EF12>(+{roleAttribute.TalentValue})</color>";
                label.transform.Find("Image/Text (TMP)").GetComponent<TextMeshProUGUI>().text = value;
   
                SetPointEvent(label);
            }
            else
            {
                RemovePointEvent(label);
            }

        }

        private void ApproveGame(int gameType, bool approved)
        {
            UIKit.OpenPanelAsync<WaitingPanel>(panel =>
            {
                Web3.Instance.ApproveMatchGame(gameType, approved);
                UIKit.ClosePanel<ConfirmBoxPanel>();
            });
        }

        private void OnApproveGame(ApproveOrder order)
        {
            StartCoroutine(CheckApproveGameOrder(order));
        }

        private IEnumerator CheckApproveGameOrder(ApproveOrder order)
        {
            bool orderChecked = false;
            while(true)
            {
                yield return new WaitForSeconds(3);

                yield return HttpHelper.Get(GameUrlConstValue.ApproveStatus.Url(), result =>
                {
                    var approveStatusResponse = JsonUtil.FromJson<ApproveStatusResponse>(result);
                    _approveStatus = approveStatusResponse.data.is_nft_approve;
                    _pvpApproveStatus = approveStatusResponse.data.is_token_approve;
                    if (order.GameType == 1)
                    {
                        orderChecked = _approveStatus == order.Approve;
                    }
                    else
                    {
                        orderChecked = _pvpApproveStatus == order.Approve;
                    }
                });

                if (orderChecked)
                {
                    break;
                }
            }

            if (orderChecked)
            {
                RefreshGameModeCost(_gameType == 2);
                if (order.Approve)
                {
                    StartMatch();
                }
                else
                {
                    UIKit.HidePanel<WaitingPanel>();
                }
            }
            else
            {
                UIKit.HidePanel<WaitingPanel>();
            }
        }

        private IEnumerator UpdateApprovedCD(int leftSeconds)
        {
            PlayBtn.interactable = false;
            _unApproveing = true;
            UnApproveState.gameObject.SetActive(true);
            while (leftSeconds > 0)
            {
                PlayBtnTxt.text = leftSeconds + "S";
                yield return new WaitForSeconds(1f);
                if (gameObject == null || !gameObject.activeSelf)
                {
                    yield break;
                }
                leftSeconds--;
            }
            _unApproveing = false;
            RefreshGameModeCost(_gameType == 2);
            UnApproveState.gameObject.SetActive(false);
            PlayBtn.interactable = true;
        }

        private void OnPetraMatch(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return;
            }
            
            UIKit.OpenPanelAsync<MatchingPanel>(panel =>
            {
                
            });
        }

        public void StartMatch()
        {
            
#if UNITY_EDITOR
            UIKit.OpenPanelAsync<MatchingPanel>(panel =>
            {

                UIKit.HidePanel<LoadingPanel>();
            });
#else
            PetraAdapter.Instance.Match();  
#endif
            

            
            
            // if (!mData.AutoStart)
            // {
            //     AudioKit.PlaySound(SoundName.clickbutton.ToString());
            // }
            // if (_gameType == 1)
            // {
            //     bool canMatch = false;
            //     for (int i = 0; i < _mainModel.RoleDatas.Count; i++)
            //     {
            //         if (_mainModel.RoleDatas[i].Energy > 0)
            //         {
            //             canMatch = true;
            //             break;
            //         }
            //     }
            //     if (!canMatch)
            //     {
            //         UIKit.OpenPanelAsync<ConfirmBoxPanel>(panel =>
            //         {
            //             UIKit.HidePanel<LoadingPanel>();
            //             UIKit.HidePanel<WaitingPanel>();
            //             panel.Cancle.gameObject.SetActive(false);
            //             panel.Content.text = "NFT without enough energy";
            //             panel.Confirm.onClick.AddListener(() =>
            //             {
            //                 UIKit.ClosePanel<ConfirmBoxPanel>();
            //             });
            //         });
            //         return;
            //     }
            //     
            //     if (_approveStatus)
            //     {
            //         UIKit.OpenPanelAsync<WaitingPanel>();
            //         RunxNetManager.Instance.Send(KeepOpCode.OP_C2R_StartMatchRacing,new C2R_StartMatchRacing(){GameType = _gameType});
            //     }
            //     else
            //     {
            //         ApproveGame(_gameType, true);
            //     }
            //     
            // }
            // else
            // {
            //     if (_mainModel.Bag.Coin < _pvpPrice)
            //     {
            //         ShowCoinNotEnougn();
            //         return;
            //     }
            //
            //     if (_pvpApproveStatus)
            //     {
            //         UIKit.OpenPanelAsync<WaitingPanel>();
            //         RunxNetManager.Instance.Send(KeepOpCode.OP_C2R_StartMatchRacing,new C2R_StartMatchRacing(){GameType = _gameType});
            //     }
            //     else
            //     {
            //         ApproveGame(_gameType, true);
            //     }
            // }
            
        }
        protected void OnStartMatch(R2C_StartMatchRacing evt)
        {
            if (evt.error == ErrCode.Ok)
            {
                RunxNetManager.UnRegisterHandler<MatchProgressMessage>(KeepOpCode.OP_MatchProgressMessage, ToMatch);
                RunxNetManager.UnRegisterHandler<RoomMatchSuccessMessage>(KeepOpCode.OP_RoomMatchSuccessMessage, MactchSuccess);
                OpenMatchPanel();
            }
        }
        
        private void OpenMatchPanel()
        {
            UIKit.OpenPanelAsync<MatchingPanel>(panel =>
            {
                UIKit.HidePanel<WaitingPanel>();
                // Hotfix.Global.Instance.LoadingScreen.HideLoading();
                UIKit.HidePanel<MainPanel>();
                UIKit.HidePanel<LoadingPanel>();
            },UILevel.Common, new MatchingPanelData(){GameMode = _gameType, PvPCost = -_pvpPrice});
        }

        protected void ToMatch(MatchProgressMessage evt)
        {
            RunxNetManager.UnRegisterHandler<R2C_StartMatchRacing>(KeepOpCode.OP_R2C_StartMatchRacing, OnStartMatch);
            RunxNetManager.UnRegisterHandler<RoomMatchSuccessMessage>(KeepOpCode.OP_RoomMatchSuccessMessage, MactchSuccess);
            var matchingPanel = UIKit.GetPanel<MatchingPanel>();
            if (matchingPanel == null || matchingPanel.State == PanelState.Closed)
            {
                UIKit.OpenPanelAsync<MatchingPanel>(panel =>
                {
                    panel.MaxNum.text = evt.target.ToString();
                    panel.MatchedNum.text = evt.current.ToString();
                    // Hotfix.Global.Instance.LoadingScreen.HideLoading();
                    UIKit.HidePanel<MainPanel>();
                    UIKit.HidePanel<WaitingPanel>();
                    UIKit.HidePanel<LoadingPanel>();
                },UILevel.Common, new MatchingPanelData(){GameMode = _gameType, PvPCost = -_pvpPrice});
            }
            else
            {
                matchingPanel.MaxNum.text = evt.target.ToString();
                matchingPanel.MatchedNum.text = evt.current.ToString();
            }

        }
        
        protected void MactchSuccess(RoomMatchSuccessMessage evt)
        {
            RunxNetManager.UnRegisterHandler<R2C_StartMatchRacing>(KeepOpCode.OP_R2C_StartMatchRacing, OnStartMatch);
            RunxNetManager.UnRegisterHandler<MatchProgressMessage>(KeepOpCode.OP_MatchProgressMessage, ToMatch);
            UIKit.OpenPanelAsync<ReadyPanel>(panel =>
            {
                UIKit.HidePanel<MatchingPanel>();
                UIKit.HidePanel<MainPanel>();
                UIKit.HidePanel<WaitingPanel>();
            },UILevel.Common, new ReadyPanelData()
            {
                MapId = (int)evt.MapId,
                RoomId = evt.RoomId,
                GameMode = _gameType,
                PvPCost = _pvpPrice
            });
            
        }

        protected void StartMatchWithContract()
        {
            // Web3.Instance.StartMatch();
        }


        protected void ShowCoinNotEnougn()
        {
            UIKit.HidePanel<LoadingPanel>();
            UIKit.HidePanel<WaitingPanel>();
            CoinNotEnough.gameObject.SetActive(true);
            var position = CoinNotEnough.rectTransform.anchoredPosition;

            CoinNotEnough.rectTransform.anchoredPosition = new Vector2(position.x, -position.y);


            var sequence = DOTween.Sequence();
            sequence.Append(CoinNotEnough.rectTransform.DOAnchorPosY(position.y, 0.4f));
            sequence.AppendInterval(1);
            sequence.AppendCallback(() =>
            {
                CoinNotEnough.gameObject.SetActive(false);
            });
            sequence.Play();
        }

        protected void OpenLuckSpinPanel()
        {
            AudioKit.PlaySound(SoundName.clickbutton.ToString());
            var maskPanel = UIKit.GetPanel<MaskPanel>();

            maskPanel.FadeIn(() => { UIKit.OpenPanelAsync<LuckSpinPanel>(); });
        }

        protected void OpenStorePanel()
        {
            AudioKit.PlaySound(SoundName.clickbutton.ToString());

            var maskPanel = UIKit.GetPanel<MaskPanel>();

            maskPanel.FadeIn(() =>
            {
                UIKit.OpenPanelAsync<StorePanel>();
                UIKit.HidePanel<MainPanel>();
            });
        }
        
        protected void OpenHistorePanel()
        {
            AudioKit.PlaySound(SoundName.clickbutton.ToString());
            
            var maskPanel = UIKit.GetPanel<MaskPanel>();

            maskPanel.FadeIn(() => { UIKit.OpenPanelAsync<HistoryPanel>(); });
        }
        
        protected void OpenInvitationPanel()
        {
            AudioKit.PlaySound(SoundName.clickbutton.ToString());
            
            var maskPanel = UIKit.GetPanel<MaskPanel>();

            maskPanel.FadeIn(() => { UIKit.OpenPanelAsync<InvitationPanel>(); });
        }

        protected void OpenRoleUpgradePanel()
        {
            AudioKit.PlaySound(SoundName.clickbutton.ToString());
            var skinIndex = currentRoleIndex.Value;
            var roleData = _mainModel.RoleDatas[skinIndex];
            LogKit.I($"OpenRoleUpgradePanel {roleData.Character}");
            UIKit.OpenPanelAsync<RoleUpgradePanel>(null, UILevel.Common,
                new RoleUpgradePanelData() { RoleToken = roleData.NftID });
        }





        protected void FillPlayerAsset(PlayerassetsResponse response)
        {
            RefreshPlayerAssets();
        }

        protected void OpenBackpack()
        {
            AudioKit.PlaySound(SoundName.clickbutton.ToString());
            UIKit.OpenPanelAsync<BackpackPanel>();
        }

        protected void RefreshPlayerAssets()
        {
            CoinBox.Refresh();
        }


        protected void FillData()
        {
            
            RefreshPlayerAssets();
            
            if (_mainModel.RoleDatas.Count > 0)
            {
                NotNftPanel.gameObject.SetActive(false);
                currentRoleIndex.Value = 0;
            
                // Pvp.isOn = _mainModel.RoleDatas[currentRoleIndex.Value].Energy <= 0;
                //
                // if (Pvp.isOn)
                // {
                Consume.text = $"consume <color=#D2EF12>{_pvpPrice.ToString("F1")}</color>";
                // }
                RefreshGameModeCost(Pvp.isOn);
            }
            else
            {
                NotNftPanel.gameObject.SetActive(true);
                
                NotNftPanel.Bg.transform.localScale = Vector3.zero;
                var sequence = DOTween.Sequence();
                sequence.Append(NotNftPanel.Bg.transform.DOScale(Vector3.one * 1.2f, 0.2f));
                sequence.Append(NotNftPanel.Bg.transform.DOScale(Vector3.one, 0.2f));
                sequence.Play();
            }


        }



        private void OnApplicationFocus(bool hasFocus)
        {
            // if (hasFocus && NotNftPanel.gameObject.activeSelf)
            // {
            //     Loading loading = new Loading();
            //     loading.AddNode(new GetRoleNode(this));
            //     loading.OnCompleted += () =>
            //     {
            //         var mainModel = MainController.Instance.GetArchitecture().GetModel<MainModel>();
            //         if (mainModel.RoleDatas.Count > 0)
            //         {
            //             FillData();
            //             NotNftPanel.gameObject.SetActive(false);
            //         }
            //     };
            //     loading.Start();
            // }
            LogKit.I($"OnApplicationFocus : {hasFocus} {DateTime.Now.ToString("hh:mm:ss t z")}");
        }
        //
        // private void OnApplicationPause(bool pauseStatus)
        // {
        //     LogKit.I($"OnApplicationPause : {pauseStatus} {DateTime.Now.ToString("hh:mm:ss t z")}");
        // }

        public void ChangeRole(int index)
        {
            currentRoleIndex.Value = index;
        }



    }


    public class RoleViewsHolder : SHItemViewsHolder
    {
        public RoleBackpackItem RoleItem;
    
        public override void CollectViews()
        {
            RoleItem = root.GetComponent<RoleBackpackItem>();
        }
    }
}