using System.Collections;
using System.Collections.Generic;
using Com.ForbiddenByte.OSA.CustomAdapters.GridView;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using SquareHero.Core.Loading;
using SquareHero.Hotfix.Generate;
using SquareHero.Hotfix.Main;
using SquareHero.Hotfix.Model;
using UnityCommon.Util;
using UnityEngine.U2D;

namespace SquareHero.Hotfix.UI
{
	public class StorePanelData : UIPanelData
	{
	}
	public partial class StorePanel : UIPanel
	{
		// protected List<PropsData> PropsDatas;
		protected List<PropsConfig> PropsConfigs;
		private SpriteAtlas _propsAtlas;
		private SpriteAtlas _propsGradeAtlas;
		private int _selectedIndex = -1;

		private ShopGoodsTable _shopGoodsTable;

		private MainModel _mainModel;

		private int _maxPayAmount;
		private float _selectdPropsPrice;
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as StorePanelData ?? new StorePanelData();
			// please add init code here
			PropsCommodityList.Adapter = new PropsCommodityGridViewAdapter();
			PropsCommodityList.Adapter.UpdateHandler += holder =>
			{
				UpdatePropsCommodityItem(holder as PropsCommodityViewsHolder);
			};
			BackBtn.onClick.AddListener(() =>
			{
				AudioKit.PlaySound(SoundName.clickbutton.ToString());

				var maskPanel = UIKit.GetPanel<MaskPanel>();
				maskPanel.FadeIn(() =>
				{
					UIKit.ClosePanel<StorePanel>();
					UIKit.ShowPanel<MainPanel>();
					maskPanel.FadeOut();
				});


			});
			StorePaySubPanel.gameObject.SetActive(false);
			BuyBtn.onClick.AddListener(OpenPayPanel);
			
			CoinBox.Refresh();
			_mainModel = MainController.Instance.GetModel<MainModel>();
			
			StorePaySubPanel.AddBtn.onClick.AddListener(() =>
			{
				int amount = int.Parse(StorePaySubPanel.AmountInput.text);
				amount++;
				if (amount <= _maxPayAmount)
				{
					StorePaySubPanel.AmountInput.text = (amount).ToString();

					StorePaySubPanel.Price.text = (amount * _selectdPropsPrice).ToString();
				}
			});
			
			StorePaySubPanel.ReduceBtn.onClick.AddListener(() =>
			{
				int amount = int.Parse(StorePaySubPanel.AmountInput.text);
				amount--;
				if (amount >= 1)
				{
					StorePaySubPanel.AmountInput.text = (amount).ToString();
					StorePaySubPanel.Price.text = (amount * _selectdPropsPrice).ToString();
				}
			});
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			Web3.Instance.BuyGoodsHandler += OnBuyGoods;
			_selectedIndex = -1;
			// PropsDatas = MainController.Instance.GetArchitecture().GetModel<MainModel>().AllPropDatas;
			
			Load();	
			
		}

		protected void Load()
		{
			Loading loading = new Loading();


			LoadAssetNode<SpriteAtlas> loadPropsAtalsNode =
				new LoadAssetNode<SpriteAtlas>(ConstValue.AtlasLocation.Props);
			LoadAssetNode<SpriteAtlas> loadPropsGradeAtalsNode =
				new LoadAssetNode<SpriteAtlas>(ConstValue.AtlasLocation.PropsGrade);
			LoadAssetNode<ShopGoodsTable> shopGoodsTableNode =
				new LoadAssetNode<ShopGoodsTable>("ShopGoods");
			loading.OnCompleted += () =>
			{
				_propsAtlas = loadPropsAtalsNode.GetAsset();
				_propsGradeAtlas = loadPropsGradeAtalsNode.GetAsset();
				_shopGoodsTable = shopGoodsTableNode.GetAsset();
				FillData();
				var maskPanel = UIKit.GetPanel<MaskPanel>();
				maskPanel.FadeOut(null);
			};
			
			loading.AddNode(loadPropsAtalsNode);
			loading.AddNode(loadPropsGradeAtalsNode);
			loading.AddNode(shopGoodsTableNode);
			
			loading.Start();
		}

		protected void FillData()
		{
			PropsConfigs = ExcelConfig.PropConfigTable.Data;
			PropsCommodityList.ResetItems(_shopGoodsTable.Data.Count);
			SelectPropsCommodity(0);
			var child = PropsIcon.transform.GetChild(0);
			child.gameObject.SetActive(true);
			child.localScale = Vector3.one;
			child.DOScale(Vector3.one * 150, 0.4f);
		}

		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
			Web3.Instance.BuyGoodsHandler -= OnBuyGoods;

			if (StorePaySubPanel.gameObject.activeSelf)
			{
				StorePaySubPanel.gameObject.SetActive(false);
			}
		}
		
		protected override void OnClose()
		{
		}

		public void UpdatePropsCommodityItem(PropsCommodityViewsHolder viewsHolder)
		{
			var index = viewsHolder.ItemIndex;
			var shopGoods = _shopGoodsTable.Data[index];
			var propsConfig = PropsConfigs.Find(config =>
			{
				return config.Id == shopGoods.GoodsArg;
			});
			
			// var propsData = PropsDatas.Find(data =>
			// {
			// 	return data.item_id == shopGoods.GoodsArg;
			// });
			
			var itemConfig = ExcelConfig.ItemConfigTable.Data.Find(config =>
			{
				return config.Id == shopGoods.GoodsArg;
			});
			var assetConfig = ExcelConfig.AssetConfigTable.Data.Find(config =>
			{
				return config.Id == propsConfig.AssetId;
			});

			var propItem = viewsHolder.PropItem;


			propItem.Bg.sprite = _propsGradeAtlas.GetSprite($"Prop_Grade_{itemConfig.Grade}");

			propItem.Icon.sprite = _propsAtlas.GetSprite(assetConfig.BigIcon);

			propItem.Count.text = _mainModel.Bag.GetAssetNumber(propsConfig.Id).ToString();

			propItem.Price.text = shopGoods.ConsumeArg2.ToString("0.0");
			propItem.Views.onClick.RemoveAllListeners();
			
			propItem.Views.onClick.AddListener(() =>
			{
				AudioKit.PlaySound(SoundName.clickbutton.ToString());
				SelectPropsCommodity(index);
			});
		}

		public void SelectPropsCommodity(int index)
		{
			if (_selectedIndex == index)
			{
				return;
			}

			_selectedIndex = index;
			for (int i = 0; i < PropsCommodityList.GetItemsCount(); i++)
			{
				var shCellViewsHolder = PropsCommodityList.GetCellViewsHolder(i);
				
				
				PropsCommodityViewsHolder viewsHolder = shCellViewsHolder as PropsCommodityViewsHolder;


				if (_selectedIndex == i)
				{
					viewsHolder.PropItem.Selected.gameObject.SetActive(true);
					viewsHolder.PropItem.Selected.DOFade(0, 1, 0.2f);
				}
				else
				{
					if (viewsHolder.PropItem.Selected.gameObject.activeSelf)
					{
						viewsHolder.PropItem.Selected.DOFade(1, 0, 0.2f).onComplete += () => viewsHolder.PropItem.Selected.gameObject.SetActive(false);
					}
				}

			}
			var shopGoods = _shopGoodsTable.Data[index];
			var propsConfig = PropsConfigs.Find(config =>
			{
				return config.Id == shopGoods.GoodsArg;
			});

			var itemConfig = ExcelConfig.ItemConfigTable.Data.Find(config =>
			{
				return config.Id == shopGoods.GoodsArg;
			});

			var assetConfig = ExcelConfig.AssetConfigTable.Data.Find(config =>
			{
				return config.Id == propsConfig.AssetId;
			});

			PropDesc.text = itemConfig.Explain;
			PropsName.text = propsConfig.Name;
			PropsIcon.sprite = _propsAtlas.GetSprite(assetConfig.BigIcon);
			SpeedIncrease.text = propsConfig.SpeedIncrease.ToString() + "%";
			if (propsConfig.Distance == -1)
			{
				TimeIncrease.text = "<size=30>∞</size>";
			}
			else
			{
				TimeIncrease.text = propsConfig.Distance.ToString();
			}
			
			if (propsConfig.UsageTimes == -1)
			{
				UsesIncrease.text = "<size=30>∞</size>";
			}
			else
			{
				UsesIncrease.text = propsConfig.UsageTimes.ToString();
			}

			Price.text = shopGoods.ConsumeArg2.ToString("0.0");

			for (int i = 0; i < PropsGrades.childCount; i++)
			{
				PropsGrades.GetChild(i).gameObject.SetActive(false);
			}
			
			PropsGrades.Find($"Grade_{itemConfig.Grade}").gameObject.SetActive(true);
		}

		protected void OpenPayPanel()
		{
			AudioKit.PlaySound(SoundName.clickbutton.ToString());
			
			var shopGoods = _shopGoodsTable.Data[_selectedIndex];
			var propsConfig = PropsConfigs.Find(config =>
			{
				return config.Id == shopGoods.GoodsArg;
			});
			var itemConfig = ExcelConfig.ItemConfigTable.Data.Find(config =>
			{
				return config.Id == shopGoods.GoodsArg;
			});

			// var propsData = PropsDatas.Find(data =>
			// {
			// 	return data.item_id == propsConfig.Id;
			// });

			var assetConfig = ExcelConfig.AssetConfigTable.Data.Find(config =>
			{
				return config.Id == propsConfig.AssetId;
			});
			var balance = _mainModel.Bag.Coin;
			LogKit.I($"balance : {balance.ToString("0.000")} - {shopGoods.ConsumeArg2}");
			
			_maxPayAmount = (int) (_mainModel.Bag.Coin/ shopGoods.ConsumeArg2);

			_selectdPropsPrice = shopGoods.ConsumeArg2;
			StorePaySubPanel.Icon.sprite = _propsAtlas.GetSprite(assetConfig.BigIcon);

			
			StorePaySubPanel.Bg.sprite = _propsGradeAtlas.GetSprite($"Prop_Grade_{itemConfig.Grade}");

			StorePaySubPanel.Price.text = shopGoods.ConsumeArg2.ToString("0.0");
			
			StorePaySubPanel.BuyBtn.onClick.RemoveAllListeners();
			
			StorePaySubPanel.BuyBtn.onClick.AddListener(() =>
			{
				AudioKit.PlaySound(SoundName.clickbutton.ToString());
				Payfor(shopGoods, itemConfig);	
			});
			
			StorePaySubPanel.CancelBtn.onClick.RemoveAllListeners();

			StorePaySubPanel.CancelBtn.onClick.AddListener(() => ClosePayPanel());

			StorePaySubPanel.AmountInput.text = "1";
			
			StorePaySubPanel.gameObject.SetActive(true);
			StorePaySubPanel.Views.DOFade(0, 1, 0.4f);
			StorePaySubPanel.BuyBtn.interactable = _mainModel.Bag.Coin >= shopGoods.ConsumeArg2;
		}

		public void Payfor(ShopGoods shopGoods, ItemConfig itemConfig)
		{
			
			UIKit.OpenPanelAsync<WaitingPanel>(panel =>
			{
				int amount = int.Parse(StorePaySubPanel.AmountInput.text);
				Web3.Instance.BuyGoods(new OrderInfo(){GoodsId = itemConfig.Id, Amount = amount, Price = shopGoods.ConsumeArg2});
			});
		}

		public void ClosePayPanel()
		{
			AudioKit.PlaySound(SoundName.clickbutton.ToString());
			StorePaySubPanel.Views.DOFade(1, 0.2f, 0.4f).onComplete +=
				() => StorePaySubPanel.gameObject.SetActive(false);
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
			while(true)
			{
				yield return new WaitForSeconds(3);

				yield return HttpHelper.Get(GameUrlConstValue.GetAsset.Url(), result => {
					var playAssetResponse = JsonUtil.FromJson<PlayerassetsResponse>(result);
					TypeEventSystem.Global.Send(new SystemEvents.UpdatePlayerAssets()
					{
						Response = playAssetResponse
					});
					CoinBox.Refresh();
					
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
			ClosePayPanel();
			PropsCommodityList.Refresh();
			// Web3.Instance.GetBalance();
		}
	}
	
	
 
	
	public class PropsCommodityViewsHolder : SHCellViewsHolder
	{
		public PropsCommodityItem PropItem;
		
		public override void CollectViews()
		{
			base.CollectViews();
			PropItem = root.GetComponent<PropsCommodityItem>();
		}
	}
	
	public class PropsCommodityCellGroupViewsHolder : SHCellGroupViewsHolder
	{
		protected override SHCellViewsHolder CreateCellViewsHolder()
		{
			return new PropsCommodityViewsHolder();
		}
	}

	public class PropsCommodityGridViewAdapter : SHScrollGridViewAdapter
	{
		public override CellGroupViewsHolder<SHCellViewsHolder> GetNewCellGroupViewsHolder()
		{
			return new PropsCommodityCellGroupViewsHolder();
		}
	}
}
