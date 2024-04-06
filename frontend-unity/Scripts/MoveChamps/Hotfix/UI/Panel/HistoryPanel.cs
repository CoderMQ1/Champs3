using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using SquareHero.Core.Loading;
using UnityCommon.Util;
using UnityEngine.U2D;

namespace SquareHero.Hotfix.UI
{
	public class HistoryPanelData : UIPanelData
	{
	}
	public partial class HistoryPanel : UIPanel
	{
		private GetHistoryResponse _getHistoryResponse;
		private SpriteAtlas propAtlas;
		private SpriteAtlas roleAtlas;
		private SpriteAtlas propGradeAtlas;
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as HistoryPanelData ?? new HistoryPanelData();
			// please add init code here
			
			BackBtn.onClick.AddListener(() =>
			{
				AudioKit.PlaySound(SoundName.clickbutton.ToString());

				var maskPanel = UIKit.GetPanel<MaskPanel>();
				maskPanel.FadeIn(() =>
				{
					UIKit.ClosePanel<HistoryPanel>();
					
					maskPanel.FadeOut();
				});


			});
			HistoryList.OnCreate += OnCreateHistoruViewsHolder;
			HistoryList.OnUpdate += (viewsHolder => { OnUpdateHistoryItem(viewsHolder as HistoryViewsHolder); });
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}

		
		protected SHItemViewsHolder OnCreateHistoruViewsHolder(int index, GameObject prefab)
		{
			HistoryViewsHolder viewsHolder = new HistoryViewsHolder();

			return viewsHolder;
		}
		protected void OnUpdateHistoryItem(HistoryViewsHolder viewsHolder)
		{
			var index = viewsHolder.ItemIndex;

			var historyData = _getHistoryResponse.Data.Records[index];

			viewsHolder.HistoryItem.Pve.gameObject.SetActive(historyData.GameMode == 1);
			viewsHolder.HistoryItem.Pvp.gameObject.SetActive(historyData.GameMode == 2);
			viewsHolder.HistoryItem.Rank.text = historyData.Rank + "st";
			//viewsHolder.HistoryItem.TimeLabel = 
			string skinId = historyData.Role;
			LogKit.I($"use role {skinId}");
			int propId = historyData.ItemId;
				
			var skinConfig = ExcelConfig.SkinConfigTable.Data.Find(config =>
			{
				return config.Name == skinId;
			});
			
			var skinAssetConfig = ExcelConfig.AssetConfigTable.Data.Find(config =>
			{
				return config.Id == skinConfig.AssetId;
			});
			var skinSprite = roleAtlas.GetSprite(skinAssetConfig.SmallIcon);
			viewsHolder.HistoryItem.Role.sprite = skinSprite;
			// viewsHolder.HistoryItem.PropsBg = 
				
			var propConfig = ExcelConfig.PropConfigTable.Data.Find(config =>
			{
				return config.Id == propId;
			});
			
			if (propConfig)
			{
				var propAssetConfig = ExcelConfig.AssetConfigTable.Data.Find(config =>
				{
					return config.Id == propConfig.AssetId;
				});
				var propSprite = propAtlas.GetSprite(propAssetConfig.BigIcon);
				var grade = propGradeAtlas.GetSprite($"Prop_Grade_{propConfig.Grade}");
				viewsHolder.HistoryItem.PropsBg.sprite = grade;
				viewsHolder.HistoryItem.PropsIcon.sprite = propSprite;
				
				viewsHolder.HistoryItem.PropsBg.gameObject.SetActive(true);
				viewsHolder.HistoryItem.PropsIcon.gameObject.SetActive(true);
			}
			else
			{
				viewsHolder.HistoryItem.PropsBg.gameObject.SetActive(false);
				viewsHolder.HistoryItem.PropsIcon.gameObject.SetActive(false);
			}

			if (historyData.Rewards != null && historyData.Rewards.Count > 0)
			{
				viewsHolder.HistoryItem.RewardNum.text = historyData.Rewards[0].quantity.ParseToken().ToString("F1");
				viewsHolder.HistoryItem.Reward.gameObject.SetActive(true);
			}
			else
			{
				viewsHolder.HistoryItem.Reward.gameObject.SetActive(false);
			}

			var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
			startTime = startTime.AddSeconds(historyData.GameStartTime);

			viewsHolder.HistoryItem.TimeLabel.text = startTime.ToString("yyyy-M-d HH:mm:ss");

		}

		protected override void OnShow()
		{

			LoadAssetNode<SpriteAtlas> roleAtlasLoad = new LoadAssetNode<SpriteAtlas>(ConstValue.AtlasLocation.Character);
			LoadAssetNode<SpriteAtlas> propsAtlasLoad = new LoadAssetNode<SpriteAtlas>(ConstValue.AtlasLocation.Props);
			LoadAssetNode<SpriteAtlas> propsGradeAtlasLoad = new LoadAssetNode<SpriteAtlas>(ConstValue.AtlasLocation.PropsGrade);
			Loading loading = new Loading();
			HttpGetNode getNode = new HttpGetNode(this, GameUrlConstValue.History.Url());
			
			loading.AddNode(getNode);
			loading.AddNode(roleAtlasLoad);
			loading.AddNode(propsAtlasLoad);
			loading.AddNode(propsGradeAtlasLoad);


			loading.OnCompleted += () =>
			{
				var result = getNode.GetResult();
				_getHistoryResponse = JsonUtil.FromJson<GetHistoryResponse>(result);
				if (_getHistoryResponse.Data.Records == null)
				{
					_getHistoryResponse.Data.Records = new List<RecordData>();
				}
				_getHistoryResponse.Data.Records.Sort((a, b) =>
				{
					if (a.GameStartTime > b.GameStartTime)
					{
						return -1;
					}
					
					if (a.GameStartTime < b.GameStartTime)
					{
						return 1;
					}

					return 0;
				});
				propAtlas = propsAtlasLoad.GetAsset();
				propGradeAtlas = propsGradeAtlasLoad.GetAsset();
				roleAtlas = roleAtlasLoad.GetAsset();
				FillDate();
				
			};
			
			loading.Start();
			
		}

		protected void FillDate()
		{
			
			HistoryList.ResetItems(_getHistoryResponse.Data.Records.Count);
			var maskPanel = UIKit.GetPanel<MaskPanel>();
			maskPanel.FadeOut(null);
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}
	}
	
	public class HistoryViewsHolder : SHItemViewsHolder
	{
		public HistoryItem HistoryItem;
    
		public override void CollectViews()
		{
			HistoryItem = root.GetComponent<HistoryItem>();
		}
	}
}
