using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using champs3.Core.Loading;
using champs3.Hotfix.Generate;
using champs3.Hotfix.Main;
using champs3.Hotfix.Model;
using champs3.Hotfix.Net;
using UnityCommon.Util;
using UnityEngine.U2D;

namespace champs3.Hotfix.UI
{
    public class LuckSpinPanelData : UIPanelData
    {
    }

    public partial class LuckSpinPanel : UIPanel
    {
        private SpriteAtlas _luckSpinAtlas;
        private SpriteAtlas _propAtlas;
        private LuckSpinRewardSettingTable _config;
        private ItemConfigTable _itemConfig;
        private int _spinTimes;
        private bool _receiveResult;
        private int _spinTargetIndex;
        private SPinResultMessage _spinResult;

        private MainModel _mainModel;

        protected override void OnInit(IUIData uiData = null)
        {
            mData = uiData as LuckSpinPanelData ?? new LuckSpinPanelData();
            // please add init code here
            // Views.alpha = 0;
            
            
            SpinButton.onClick.AddListener(Spin);
            ExchangeBtn.onClick.AddListener(Exchange);
            BackBtn.onClick.AddListener(() =>
            {
                var maskPanel = UIKit.GetPanel<MaskPanel>();
                maskPanel.FadeIn(() =>
                {
                    UIKit.ClosePanel<LuckSpinPanel>();
					
                    maskPanel.FadeOut();
                });
            });
            
            champs3NetManager.RegisterHandler<SPinResultMessage>(KeepOpCode.OP_S2C_SPinResultMessage, OnReceiveSpinResult);
            _mainModel = MainController.Instance.GetModel<MainModel>();
            
            ReceiveRewardSubPanel.ShareBtn.onClick.AddListener(() =>
            {
                // Global.Instance.ShareToTwitter.TweetWithScreenshot("I am so lucky! I drew the card I wanted on champs3! @champs3xyz");
                Global.Instance.ShareToTwitter.TweetWithScreenshot("Free Charge @champs3xyz");
            });
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            var mainModel = MainController.Instance.GetArchitecture().GetModel<MainModel>();

            _spinTimes = mainModel.Bag.LotteryKey;
        }

        protected override void OnShow()
        {
            Jackpot.gameObject.SetActive(false);
            Load();
            CoinBox.Refresh();

            Web3.Instance.RotateSpinHandler += OnRotateSpinHandler;

        }

        private void OnRotateSpinHandler(bool result)
        {
            UIKit.HidePanel<WaitingPanel>();
            if (result)
            {
                StartCoroutine(DoRotate());
            }
        }

        private IEnumerator DoRotate()
        {
            // UIKit.DisableUIInput();
            Mask.gameObject.SetActive(true);
            int rotateTimes = 0;
            float waitTime = 0.24f;
            int index = _spinTargetIndex;
            bool end = false;
            int offset = Random.Range(1, 5);
            while (true)
            {
                var luckSpinReward = Rewards.Find($"Reward_{index + 1}").GetComponent<LuckSpinReward>();
                luckSpinReward.Select.gameObject.SetActive(true);

                if (end && index == _spinTargetIndex)
                {
                    break;
                }
                
                yield return new WaitForSeconds(waitTime);
                luckSpinReward.Select.gameObject.SetActive(false);
 


                if (!end && _receiveResult)
                {
                    int last = index + 4 + offset;
                    
                    
                    if (last >= Rewards.childCount)
                    {
                        last -= Rewards.childCount;
                    }
                    end = _spinTargetIndex == last;

                    if (end)
                    {
                        LogKit.I($"{index} - {last} - {offset}");
                    }
                }


                if (end)
                {
                    waitTime += 0.1f;

                    waitTime = Mathf.Min(0.5f, waitTime);
                }
                if (rotateTimes < 1)
                {
                    waitTime -= 0.02f;
                }
                
                index++;
                if (index >= Rewards.childCount)
                {
                    index = 0;
                    rotateTimes ++;
                }
            }
            var tartgetReward = Rewards.Find($"Reward_{_spinTargetIndex + 1}").GetComponent<LuckSpinReward>();
            for (int i = 0; i < 3; i++)
            {
                
                tartgetReward.Select.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.2f);
                tartgetReward.Select.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.2f);
            }
            tartgetReward.Select.gameObject.SetActive(true);
            _receiveResult = false;
            if (_spinResult != null)
            {
                ShowRewardPanel(_spinResult);
            }
            Mask.gameObject.SetActive(false);
        }

        protected void Load()
        {
            LoadAssetNode<LuckSpinRewardSettingTable> configLoadNode =
                new LoadAssetNode<LuckSpinRewardSettingTable>(ConstValue.ConfigLocation.LuckSpinRewardSetting);
            LoadAssetNode<ItemConfigTable> itemConfigLoadNode =
                new LoadAssetNode<ItemConfigTable>(ConstValue.ConfigLocation.ItemConfig);
            LoadAssetNode<SpriteAtlas> atlasLoadNode =
                new LoadAssetNode<SpriteAtlas>(ConstValue.AtlasLocation.LuckSpin);
            LoadAssetNode<SpriteAtlas> propsAtlas =
                new LoadAssetNode<SpriteAtlas>(ConstValue.AtlasLocation.Props);
            HttpGetNode spinInfoNode = new HttpGetNode(this, GameUrlConstValue.SpinInfo.Url());

            Loading loading = new Loading();

            loading.AddNode(configLoadNode);
            loading.AddNode(atlasLoadNode);
            loading.AddNode(itemConfigLoadNode);
            loading.AddNode(propsAtlas);
            loading.AddNode(spinInfoNode);

            loading.OnCompleted += () =>
            {
                _config = configLoadNode.GetAsset();
                _luckSpinAtlas = atlasLoadNode.GetAsset();
                _itemConfig = itemConfigLoadNode.GetAsset();
                _propAtlas = propsAtlas.GetAsset();

                SpinInfoResponse response = JsonUtil.FromJson<SpinInfoResponse>(spinInfoNode.GetResult());

                Jackpot.text = Mathf.RoundToInt(response.jackpot.ParseToken()).ToString();
                BonusPool.text = Mathf.RoundToInt(response.bonus_pool.ParseToken()).ToString();
                FillData();
                
                var maskPanel = UIKit.GetPanel<MaskPanel>();
                maskPanel.FadeOut(null);
            };

            loading.Start();
        }

        public void FillData()
        {
            // Views.DOFade(0, 1, 0.4f);
            RefreshSpinTimes();

            for (int i = 0; i < _config.Data.Count; i++)
            {
                var rewardSetting = _config.Data[i];

                var find = Rewards.Find($"Reward_{i + 1}");

                if (find)
                {
                    var itemConfig = _itemConfig.Data.Find(config => { return config.Id == int.Parse(rewardSetting.RewardID); });
                    var luckSpinReward = find.GetComponent<LuckSpinReward>();
                    if (itemConfig.Id == 2 && rewardSetting.RewardNum == 200)
                    {
                        luckSpinReward.RewardIcon.sprite = _luckSpinAtlas.GetSprite($"Pool");
                        JackPot.transform.parent = luckSpinReward.transform;
                        JackPot.GetComponent<RectTransform>().anchoredPosition = new Vector2(32, 46);
                        JackPot.gameObject.SetActive(true);

                        ItemSparkleSoftYellow.transform.parent = luckSpinReward.RewardIcon.transform;
                        ItemSparkleSoftYellow.transform.localPosition = Vector3.zero;
                        ItemSparkleSoftYellow.gameObject.SetActive(true);
                        var canvas = luckSpinReward.GetOrAddComponent<Canvas>();
                        canvas.overrideSorting = true;
                        canvas.sortingLayerName = "UI";
                        canvas.sortingOrder = 1002;
                    }
                    else if (itemConfig.Id == 2 && rewardSetting.RewardNum != 500)
                    {
                        int level = 1;

                        if (rewardSetting.RewardNum == 5)
                        {
                            level = 2;
                        }
                        
                        if (rewardSetting.RewardNum == 10)
                        {
                            level = 3;
                        }

                        var sprite = _luckSpinAtlas.GetSprite($"Coin_{level}");
                        luckSpinReward.RewardIcon.sprite = sprite;
                        luckSpinReward.RewardIcon.GetComponent<RectTransform>().sizeDelta =
                            new Vector2(sprite.rect.width, sprite.rect.height);
                    }
                    else
                    {
                        luckSpinReward.RewardIcon.sprite = _luckSpinAtlas.GetSprite(itemConfig.IconBig);
                    }

                    if (rewardSetting.RewardType == "res")
                    {
                        luckSpinReward.RewardCount.text = GetNumberTxt(rewardSetting.RewardNum.ToString());
                    }
                    else
                    {
                        float bonusPool = float.Parse(BonusPool.text);
                        float value = (bonusPool * rewardSetting.RewardNum * 0.001f);

                        string valueStr = value.ToString("0.0");
                        if (value >= 0 && value < 10)
                        {
                            valueStr = value.ToString("0.000");
                        }else if (value >= 10 && value < 100)
                        {
                            valueStr = value.ToString("0.00");
                        }
                        else if (value >= 100 && value < 1000)
                        {
                            valueStr = value.ToString("0.0");
                        }
                        {
                            valueStr = Mathf.FloorToInt(value).ToString();
                        }
                        luckSpinReward.RewardCount.text =  GetNumberTxt(valueStr);
                    }

                    luckSpinReward.RewardIcon.transform.rotation = Quaternion.identity;


                }
            }
        }

        protected string GetNumberTxt(string num)
        {
            var charArray = num.ToCharArray();
            string txt = "<sprite=10>";
            for (int j = 0; j < charArray.Length; j++)
            {
                if (charArray[j] != 'x')
                {
                    txt += $"<sprite={charArray[j]}>";
                }
            }
            return txt;
        }

        private void RefreshSpinTimes()
        {
            SpinTimes.text = _spinTimes + "/1";
            SpinButton.interactable = _spinTimes > 0;
        }

        protected void Exchange()
        {
            var enumerator = HttpHelper.PostJson(GameUrlConstValue.SpinExchange.Url(), "",result =>
            {
                var spinExchangeResponse = JsonUtil.FromJson<SpinExchangeResponse>(result);

                _spinTimes = spinExchangeResponse.key_num;
                RefreshSpinTimes();
                var mainModel = MainController.Instance.GetArchitecture().GetModel<MainModel>();
                mainModel.Bag.UpdateAssetNumber(ConstValue.SpinChipId, spinExchangeResponse.chip_num);
                mainModel.Bag.UpdateAssetNumber(ConstValue.SpinKeyId, spinExchangeResponse.key_num);
                CoinBox.Refresh();
            });


            StartCoroutine(enumerator);
        }

        protected void Spin()
        {
            // UIKit.DisableUIInput();
            // if (_spinTimes > 0)
            // {
                // var enumerator = HttpHelper.PostJson(GameUrlConstValue.Spin.Url(), "",result =>
                // {
                //     var spinResponse = JsonUtil.FromJson<SpinResponse>(result);
                //
                //     StartCoroutine(Rotate(spinResponse));
                //
                // });
                //
                //
                // StartCoroutine(enumerator);
                var waitingPanel = UIKit.GetPanel<WaitingPanel>();
                if (waitingPanel == null)
                {
                    UIKit.OpenPanelAsync<WaitingPanel>();
                }
                else
                {
                    UIKit.ShowPanel<WaitingPanel>();
                }
                Web3.Instance.RotateSpin();
            
            // }
            
        }

        protected void OnReceiveSpinResult(SPinResultMessage evt)
        {
            _receiveResult = true;
            _spinTargetIndex = (int)evt.TargetId - 1;
            _spinResult = evt;
            // StartCoroutine(Rotate(evt));
        }

        protected void ShowRewardPanel(SPinResultMessage response)
        {
            var itemConfig = _itemConfig.Data.Find(config =>
            {
                return config.Id == response.RewardID;
            });

            if (itemConfig == null)
            {
                LogKit.E($"Not Item config [{response.RewardID}]");
            }


            var sprite = _luckSpinAtlas.GetSprite(itemConfig.IconBig);

            if (sprite == null)
            {
                LogKit.E($"Not Item sprite [{response.RewardID}] ");
            }

            // _mainModel.Bag.AddAssetNumber((int)response.RewardID, response.RewardNum);
            
            ReceiveRewardSubPanel.RewardIcon.sprite = sprite;

            if (response.RewardID == 2)
            {
                ReceiveRewardSubPanel.RewardCount.text = response.RewardNum.ParseToken().ToString("R");
                
            }
            else
            { 
                ReceiveRewardSubPanel.RewardCount.text = response.RewardNum.ToString();
            }
            

            ReceiveRewardSubPanel.CloseBtn.onClick.RemoveAllListeners();
            ReceiveRewardSubPanel.CloseBtn.onClick.AddListener(() =>
            {
                BonusPool.text = Mathf.RoundToInt(response.BonusPool.ParseToken()).ToString();
                Jackpot.text = Mathf.RoundToInt(response.Jackpot.ParseToken()).ToString();
                _spinTimes--;
                RefreshSpinTimes();
                ReceiveRewardSubPanel.gameObject.SetActive(false);
                
            });
            
            ReceiveRewardSubPanel.gameObject.SetActive(true);
            
            
            Loading loading = new Loading();
            loading.AddNode(new GetPlayerAssetNode(this));
            // HttpGetNode spinInfoNode = new HttpGetNode(this, GameUrlConstValue.SpinInfo.Url());
            // loading.AddNode(spinInfoNode);
            loading.OnCompleted += () =>
            {
 
                // Web3.Instance.GetBalance();
                CoinBox.Refresh();
                // var result = spinInfoNode.GetResult();
                // SpinInfoResponse response = JsonUtil.FromJson<SpinInfoResponse>(result);
            };
            loading.Start();

        }

        protected override void OnHide()
        {
            Web3.Instance.RotateSpinHandler -= OnRotateSpinHandler;
        }

        protected override void OnClose()
        {
        }
    }
}