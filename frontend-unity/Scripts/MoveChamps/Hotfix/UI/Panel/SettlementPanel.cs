using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using SquareHero.Hotfix.GameLogic;
using SquareHero.Hotfix.Generate;
using SquareHero.Hotfix.Model;
using SquareHero.Hotfix.Net;
using SquareHero.Hotfix.Player;
using TMPro;
using UnityEngine.Experimental.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.Video;
using SquareHero.Hotfix.Main;

namespace SquareHero.Hotfix.UI
{
	public class SettlementPanelData : UIPanelData
	{
		public SpriteAtlas RoleAtlas;
		public SpriteAtlas PropsAtlas;
		public SpriteAtlas PropsGradeAtlas;
	}
	public partial class SettlementPanel : UIPanel
	{
		private SkinConfigTable skinConfigTable;
		private AssetConfigTable assetConfigTable;
		private PropsConfigTable propConfigTable;
		private SpriteAtlas spriteAtlas;
		private SpriteAtlas propAtlas;
		private bool _countDowning;
		private int _time = 10;
		private IEnumerator _countDown;
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as SettlementPanelData ?? new SettlementPanelData();
			// please add init code here
			
			ShareBtn.onClick.AddListener(() =>
			{
				Global.Instance.ShareToTwitter.TweetWithScreenshot("Free Charge @runxxyz");
			});
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			skinConfigTable = ExcelConfig.SkinConfigTable;
			assetConfigTable = ExcelConfig.AssetConfigTable;
			propConfigTable = ExcelConfig.PropConfigTable;


			spriteAtlas = mData.RoleAtlas;
			propAtlas = mData.PropsAtlas;
		

			ContinueBtn.onClick.AddListener(() =>
			{
				StopCoroutine(_countDown);
				ReturnMainAndPlayAgain();
			});
			
			ReturnBtn.onClick.AddListener(() =>
			{
				StopCoroutine(_countDown);
				ReturnMain();
			});
			
			LuckDrawBtn.onClick.AddListener(() =>
			{
				StopCoroutine(_countDown);
				UIKit.OpenPanelAsync<LuckSpinPanel>(panel =>
				{
					panel.BackBtn.onClick.AddListener(() =>
					{
						ReturnMain();
					});
				});
			});
		}
		
		protected override void OnShow()
		{

			Views.gameObject.SetActive(false);
			
			// FadeIn(sequence, Rank1.transform as RectTransform, 1);
			// FadeIn(sequence, Rank2.GetComponent<RectTransform>(), 2);
			// FadeIn(sequence, Rank3.GetComponent<RectTransform>(), 3);
			// FadeIn(sequence, Rank4.GetComponent<RectTransform>(), 4);
			// FadeIn(sequence, Rank5.GetComponent<RectTransform>(), 5);
			
			// SHNetManager.RegisterHandler<RoomUserSettleResult>(KeepOpCode.Op_S2C_RoomUserSettleResult, OnSettle);
		}

		public void OnSettle(List<GameObject> Rank)
		{
			// var settleList = evt.UserSettleList;
			// settleList.Sort((a, b) =>
			// {
			// 	if (a.rank > b.rank)
			// 	{
			// 		return 1;
			// 	}
			//
			// 	if (a.rank < b.rank)
			// 	{
			// 		return -1;
			// 	}
			//
			// 	return 0;
			// });
	
			var playerManager = RoomManager.Instance.GetComponent<PlayerManager>();


			for (int i = 0; i < Rank.Count; i++)
			{
				// var userSettleInfo = settleList[i];
				var shPlayerController = Rank[i].GetComponent<SHPlayerController>();

				var animatorController = shPlayerController.GetComponent<CharacterAnimatorController>();
				if (i == 0)
				{
					animatorController.PlayAnimation("Rank_1");
				}
				
				if (i == 1 && i == 2)
				{
					animatorController.PlayAnimation("Rank_2");
				}
				if (i > 2)
				{
					animatorController.PlayAnimation("Rank_3");
				}
			}
			
			AudioKit.PlaySound(SoundName.Win.ToString());
			ActionKit.Delay(1, () =>
			{
				ShowSettle(Rank, RoomManager.Instance.GameType);
			}).Start(this);
		}


		private void ShowSettle(List<GameObject> Rank, int gameType)
		{
			

			_countDown = StartCountDown();
			StartCoroutine(_countDown);
			Views.gameObject.SetActive(true);
			var rectTransform = RewardsLabel.GetComponent<RectTransform>();
			var rect = rectTransform.parent.GetComponent<RectTransform>().rect;
			var position = rectTransform.anchoredPosition;

			rectTransform.anchoredPosition = position + Vector2.up * 500;
			
			var sequence = DOTween.Sequence();
			sequence.Append(rectTransform.DOAnchoredMove(position, 0.5f));
			sequence.AppendInterval(5f);
			
			var left = LeftRank.GetComponent<RectTransform>();
			var right = RightRank.GetComponent<RectTransform>();

			var leftAnchoredPosition = left.anchoredPosition;
			left.anchoredPosition =  Vector2.left * left.rect.width;
			

			sequence.Insert(0.2f, left.DOAnchoredMove(leftAnchoredPosition, 0.5f));
			
			
			var rightAnchoredPosition = right.anchoredPosition;
			right.anchoredPosition = new Vector2(right.sizeDelta.x, rightAnchoredPosition.y);

			sequence.Insert(0.2f, right.DOAnchoredMove(rightAnchoredPosition, 0.5f));

			MainModel mainModel = MainController.Instance.GetArchitecture().GetModel<MainModel>();

			for (int i = 0; i < Rank.Count; i++)
			{
				// var userSettleInfo = settleList[i];
				var playerInfo = Rank[i].GetComponent<SHPlayerController>();
				
				var child = PointRanks.transform.GetChild(i);
				string playerName = playerInfo.Data.UserName;
				string skinId = playerInfo.Data.SkinName;
				int propId = playerInfo.Data.PropIds[0];
				
				var skinConfig = skinConfigTable.Data.Find(config =>
				{
					return config.Name == skinId;
				});
				
				var skinAssetConfig = assetConfigTable.Data.Find(config =>
				{
					return config.Id == skinConfig.AssetId;
				});

				bool isLocal = playerInfo.Data.UserId == mainModel.Player.UserId;
				child.Find("Other").gameObject.SetActive(!isLocal);
				child.Find("Self").gameObject.SetActive(isLocal);
				var nameText = child.Find("PlayerName").GetComponent<TextMeshProUGUI>();
				nameText.text = playerName;
				if (isLocal)
				{
					nameText.color = new Color(0.9333333f, 0.8334295f, 0.1647059f, 1);
				}
				var skinSprite = spriteAtlas.GetSprite(skinAssetConfig.SmallIcon);
				child.Find("Avator").GetComponent<Image>().sprite = skinSprite;
				
				var propConfig = propConfigTable.Data.Find(config =>
				{
					return config.Id == propId;
				});
			
				if (propConfig)
				{
					var propAssetConfig = assetConfigTable.Data.Find(config =>
					{
						return config.Id == propConfig.AssetId;
					});
			
					var propSprite = propAtlas.GetSprite(propAssetConfig.BigIcon);
					child.Find("Prop").GetComponent<Image>().sprite = propSprite;
					child.Find("PropGrade").GetComponent<Image>().sprite = mData.PropsGradeAtlas.GetSprite($"Prop_Grade_{propConfig.Grade}");
				}
				else
				{
					child.Find("Prop").gameObject.SetActive(false);
					child.Find("PropGrade").gameObject.SetActive(false);
				}


				if (i == 0 && gameType == 2)
				{
					child.Find("LuckDrawBtn").gameObject.SetActive(false);
					child.Find("DiamondNum").gameObject.SetActive(false);
					child.Find("DiamondIcon").gameObject.SetActive(false);
					child.Find("CoinIcon").gameObject.SetActive(true);
					var coinNum = child.Find("CoinNum");
					coinNum.gameObject.SetActive(true);
					coinNum.GetComponent<TextMeshProUGUI>().text = "+" + 1;
					
				}
			}
			
			
			
			
			for (int i = 0; i < Rank.Count; i++)
			{

				var playerInfo = Rank[i].GetComponent<SHPlayerController>();
				
				var child = TokensRanks.transform.GetChild(i);
				string playerName = playerInfo.Data.UserName;
				string skinId = playerInfo.Data.SkinName;
				int propId = playerInfo.Data.PropIds[0];
				
				var skinConfig = skinConfigTable.Data.Find(config =>
				{
					return config.Name == skinId;
				});
				
				var skinAssetConfig = assetConfigTable.Data.Find(config =>
				{
					return config.Id == skinConfig.AssetId;
				});
				var skinSprite = spriteAtlas.GetSprite(skinAssetConfig.SmallIcon);
				child.Find("Avator").GetComponent<Image>().sprite = skinSprite;

				
				bool isLocal = playerInfo.Data.UserId == mainModel.Player.UserId;
				child.Find("Other").gameObject.SetActive(!isLocal);
				child.Find("Self").gameObject.SetActive(isLocal);
				// if (playerName == mainModel.Player.PlayerName)
				// {
				// 	child.GetComponent<Image>().sprite = propAtlas.GetSprite("settlement_bg_own");
				// }
				// else
				// {
				// 	child.GetComponent<Image>().sprite = propAtlas.GetSprite("settlement_bg_other");
				// }

				var propConfig = propConfigTable.Data.Find(config =>
				{
					return config.Id == propId;
				});

				if (propConfig)
				{
					var propAssetConfig = assetConfigTable.Data.Find(config =>
					{
						return config.Id == propConfig.AssetId;
					});
					var propSprite = propAtlas.GetSprite(propAssetConfig.BigIcon);

					child.Find("Prop").GetComponent<Image>().sprite = propSprite;
					child.Find("PropGrade").GetComponent<Image>().sprite = mData.PropsGradeAtlas.GetSprite($"Prop_Grade_{propConfig.Grade}");
				}else
				{
					child.Find("Prop").gameObject.SetActive(false);
					child.Find("PropGrade").gameObject.SetActive(false);
				}

				// child.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = playerName;
				var nameText = child.Find("PlayerName").GetComponent<TextMeshProUGUI>();
				nameText.text = playerName;
				if (isLocal)
				{
					nameText.color = new Color(0.9333333f, 0.8334295f, 0.1647059f, 1);
				}
				child.Find("CoinNum").GetComponent<TextMeshProUGUI>().text = "+" + (2 - (i * 0.4f));
			}
			
		}

		protected IEnumerator StartCountDown()
		{
			_time = 10;
			for (int i = 0; i < 10; i++)
			{
				CountDown.text = $"({_time}s)";
				yield return new WaitForSeconds(1);
				_time--;
			}
			ReturnMainAndPlayAgain();
		}


		public void ReturnMain()
		{
			UIKit.ShowPanel<LoadingPanel>();
			// Hotfix.Global.Instance.LoadingScreen.ShowLoading();
			
				
			var playerModel = LevelManager.Instance.CurrentLevelController.GetArchitecture().GetModel<PlayerModel>();
			playerModel.PlayerModes.Clear();
			
			// var videoPlayerGameObject = Global.Instance.VideoPlayer.gameObject;
			// var instantiate = Instantiate(videoPlayerGameObject, videoPlayerGameObject.transform.parent);
			// var videoPlayer = instantiate.GetComponent<VideoPlayer>();
			// Destroy(videoPlayerGameObject);
			// Global.Instance.VideoPlayer = videoPlayer;
			// Global.Instance.VideoPlayer.url = "https://resource.squarehero.io/HotfixAsset/WebGL/Video/Main.mp4";
			// Global.Instance.VideoPlayer.targetTexture = new RenderTexture(960, 540, 16);
			// videoPlayer.Play();
			// UIKit.HidePanel<LevelPanel>();
			ResourceManager.Instance.LoadSceneAsync("Scene_Main", () =>
			{
				// UIKit.OpenPanelAsync<VideoPanel>(panel =>
				// {
					// Destroy(videoPlayerGameObject);
					UIKit.OpenPanelAsync<MainPanel>();
					UIKit.ClosePanel<LevelPanel>();
					UIKit.ClosePanel<SettlementPanel>();
				// });
			});
		}

		protected void ReturnMainAndPlayAgain()
		{
			// Hotfix.Global.Instance.LoadingScreen.ShowLoading();
			UIKit.OpenPanelAsync<LoadingPanel>(null);

			int gameMode = RoomManager.Instance.RoomInfo.game_type;
			var playerModel = LevelManager.Instance.CurrentLevelController.GetArchitecture().GetModel<PlayerModel>();
			playerModel.PlayerModes.Clear();
				
			// var videoPlayerGameObject = Global.Instance.VideoPlayer.gameObject;
			// var instantiate = Instantiate(videoPlayerGameObject, videoPlayerGameObject.transform.parent);
			// var videoPlayer = instantiate.GetComponent<VideoPlayer>();
			// Destroy(videoPlayerGameObject);
			// Global.Instance.VideoPlayer = videoPlayer;
			// Global.Instance.VideoPlayer.url = "https://resource.squarehero.io/HotfixAsset/WebGL/Video/Main.mp4";
			// Global.Instance.VideoPlayer.targetTexture = new RenderTexture(1920, 1080, 16);
			// videoPlayer.Play();
			UIKit.HidePanel<LevelPanel>();
			ResourceManager.Instance.LoadSceneAsync("Scene_Main", () =>
			{

				// UIKit.OpenPanelAsync<VideoPanel>(panel =>
				// {
					UIKit.OpenPanelAsync<MainPanel>(panel =>
					{
						UIKit.ClosePanel<LevelPanel>();
						UIKit.ClosePanel<SettlementPanel>();
					}, UILevel.Common, new MainPanelData(){AutoStart = true, AutoStartMode = gameMode});
					UIKit.ClosePanel<SettlementPanel>();
				// });
				

			});
		}

		
		protected void FadeIn(Sequence sequence, RectTransform rectTransform, int i)
		{
			var position = rectTransform.anchoredPosition;
			rectTransform.anchoredPosition = position + Vector2.right * 1000;

			sequence.Insert(0.2f * i ,rectTransform.DOAnchoredMove(position, 0.5f));
		}


		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{

		}

		public void SetPointRank(long[] ids)
		{

			var playerModel = LevelManager.Instance.CurrentLevelController.GetArchitecture().GetModel<PlayerModel>();

			
			for (int i = 0; i < PointRanks.transform.childCount; i++)
			{
				var child = PointRanks.transform.GetChild(i);
				string playerName = playerModel.PlayerModes[ids[i]].UserName;
				int skinId = playerModel.PlayerModes[ids[i]].SkinIndex;
				int propId = playerModel.PlayerModes[ids[i]].PropId;
				
				var skinConfig = skinConfigTable.Data.Find(config =>
				{
					return config.Id == skinId;
				});
				
				var skinAssetConfig = assetConfigTable.Data.Find(config =>
				{
					return config.Id == skinConfig.AssetId;
				});
			
							
				child.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = playerName;
				var skinSprite = spriteAtlas.GetSprite(skinAssetConfig.SmallIcon);
				child.Find("Avator").GetComponent<Image>().sprite = skinSprite;
				
				var propConfig = propConfigTable.Data.Find(config =>
				{
					return config.Id == propId;
				});

				if (propConfig)
				{
					var propAssetConfig = assetConfigTable.Data.Find(config =>
					{
						return config.Id == propConfig.AssetId;
					});
			
					var propSprite = propAtlas.GetSprite(propAssetConfig.BigIcon);
					child.Find("Prop").GetComponent<Image>().sprite = propSprite;
					child.Find("PropGrade").GetComponent<Image>().sprite = mData.PropsGradeAtlas.GetSprite($"Prop_Grade_{propConfig.Grade}");
				}
				else
				{
					child.Find("Prop").gameObject.SetActive(false);
					child.Find("PropGrade").gameObject.SetActive(false);
				}
			}
			
			
		}
		
		
		public void SetTokensRank()
		{
			
			var playerModel = LevelManager.Instance.CurrentLevelController.GetArchitecture().GetModel<PlayerModel>();
			var ids = playerModel.CoinRank.ToArray();


			for (int i = 0; i < TokensRanks.transform.childCount; i++)
			{
				var child = TokensRanks.transform.GetChild(i);
				string playerName = playerModel.PlayerModes[ids[i]].UserName;
				int skinId = playerModel.PlayerModes[ids[i]].SkinIndex;
				int propId = playerModel.PlayerModes[ids[i]].PropId;
				
				var skinConfig = skinConfigTable.Data.Find(config =>
				{
					return config.Id == skinId;
				});
				
				var skinAssetConfig = assetConfigTable.Data.Find(config =>
				{
					return config.Id == skinConfig.AssetId;
				});
				var skinSprite = spriteAtlas.GetSprite(skinAssetConfig.SmallIcon);
				child.Find("Avator").GetComponent<Image>().sprite = skinSprite;
				
				
				var propConfig = propConfigTable.Data.Find(config =>
				{
					return config.Id == propId;
				});

				if (propConfig)
				{
					var propAssetConfig = assetConfigTable.Data.Find(config =>
					{
						return config.Id == propConfig.AssetId;
					});
					var propSprite = propAtlas.GetSprite(propAssetConfig.BigIcon);

					child.Find("Prop").GetComponent<Image>().sprite = propSprite;
					child.Find("PropGrade").GetComponent<Image>().sprite = mData.PropsGradeAtlas.GetSprite($"Prop_Grade_{propConfig.Grade}");
				}else
				{
					child.Find("Prop").gameObject.SetActive(false);
					child.Find("PropGrade").gameObject.SetActive(false);
				}

			
			
				child.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = playerName;


				child.Find("CoinNum").GetComponent<TextMeshProUGUI>().text = "+" +
					playerModel.PlayerModes[ids[i]].Coins.ToString();

			}
		}

		public void SetRank(Transform RankRoot, string playerName, int skinId, int propId)
		{
			var skinConfig = skinConfigTable.Data.Find(config =>
			{
				return config.Id == skinId;
			});
			
			var propConfig = propConfigTable.Data.Find(config =>
			{
				return config.Id == propId;
			});

			var skinAssetConfig = assetConfigTable.Data.Find(config =>
			{
				return config.Id == skinConfig.AssetId;
			});
			
			var propAssetConfig = assetConfigTable.Data.Find(config =>
			{
				return config.Id == propConfig.AssetId;
			});
			
			LogKit.I($"SetRank prop small icon{propAssetConfig.SmallIcon}");
			
			RankRoot.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = playerName;
			var skinSprite = spriteAtlas.GetSprite(skinAssetConfig.SmallIcon);
			var propSprite = propAtlas.GetSprite(propAssetConfig.SmallIcon);
			RankRoot.Find("Avator").GetComponent<Image>().sprite = skinSprite;
			RankRoot.Find("Prop").GetComponent<Image>().sprite = propSprite;
		}

		private void Update()
		{
			
		}
	}
}
