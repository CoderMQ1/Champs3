using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using SquareHero.Core.Loading;
using SquareHero.Hotfix.Events;
using SquareHero.Hotfix.GameLogic;
using SquareHero.Hotfix.Generate;
using SquareHero.Hotfix.Model;
using SquareHero.Hotfix.Net;
using SquareHero.Hotfix.Player;
using TMPro;
using UnityEngine.U2D;
using UnityEngine.Video;
using Random = UnityEngine.Random;

namespace SquareHero.Hotfix.UI
{
	public class LevelPanelData : UIPanelData
	{
		public long RoomId;
		public long UserId;
		public int MapId;
	}
	public partial class LevelPanel : UIPanel
	{
		private SpriteAtlas propAtlas;
		private SpriteAtlas roleAtlas;
		private SpriteAtlas propGradeAtlas;
		private Dictionary<long, GameObject> PlayerHuds = new Dictionary<long, GameObject>();
		private bool _isGameOver;
		private RoomUserSettleResult _settleResult;
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as LevelPanelData ?? new LevelPanelData();
			// please add init code here
			
			BackBtn.onClick.AddListener(ReturnMain);
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{

		}
		
		protected override void OnShow()
		{
			
			UIKit.HidePanel<MatchingPanel>();
			Loading loading = new Loading();
			LoadAssetNode<SpriteAtlas> roleAtlasLoad = new LoadAssetNode<SpriteAtlas>(ConstValue.AtlasLocation.Character);
			LoadAssetNode<SpriteAtlas> propsAtlasLoad = new LoadAssetNode<SpriteAtlas>(ConstValue.AtlasLocation.Props);
			LoadAssetNode<SpriteAtlas> propsGradeAtlasLoad = new LoadAssetNode<SpriteAtlas>(ConstValue.AtlasLocation.PropsGrade);
			loading.AddNode(roleAtlasLoad);
			loading.AddNode(propsAtlasLoad);
			loading.AddNode(propsGradeAtlasLoad);

			loading.OnCompleted += () =>
			{
				propAtlas = propsAtlasLoad.GetAsset();
				propGradeAtlas = propsGradeAtlasLoad.GetAsset();
				roleAtlas = roleAtlasLoad.GetAsset();
				
				// var shPlayerController = NetworkClient.localPlayer.GetComponent<SHPlayerController>();
			
				// var abstractProps = shPlayerController.Props;

				// GameStateManager.Instance.StartGame(mData.RoomId, mData.MapId, mData.UserId);
			};
			
			loading.Start();

			Num1.gameObject.SetActive(false);
			Num2.gameObject.SetActive(false);
			Num3.gameObject.SetActive(false);
			StartLabel.gameObject.SetActive(false);
			PropBox.gameObject.SetActive(false);
			CoinRank.gameObject.SetActive(false);
			TypeEventSystem.Global.Register<GameEvents.StartCountDown>(OnStartGame);
			TypeEventSystem.Global.Register<GameEvents.OnCoinRankChange>(OnCoinRankChange);
			TypeEventSystem.Global.Register<PlayerEvents.GetCoin>(OnPlayerGetCoin);
			TypeEventSystem.Global.Register<GameEvents.OnClientGameOver>(OnGameOver);
			// RunxNetManager.RegisterHandler<RoomUserSettleResult>(KeepOpCode.Op_S2C_RoomUserSettleResult, OnSettle);
			
			// StartCountDown();
		}
		
		protected override void OnHide()
		{
			_startGame = false;
			TypeEventSystem.Global.UnRegister<GameEvents.StartCountDown>(OnStartGame);
			TypeEventSystem.Global.UnRegister<GameEvents.OnCoinRankChange>(OnCoinRankChange);
			TypeEventSystem.Global.UnRegister<PlayerEvents.GetCoin>(OnPlayerGetCoin);
			TypeEventSystem.Global.UnRegister<GameEvents.OnClientGameOver>(OnGameOver);

			// RunxNetManager.UnRegisterHandler<RoomUserSettleResult>(KeepOpCode.Op_S2C_RoomUserSettleResult, OnSettle);
		}
		
		private void OnGameOver(GameEvents.OnClientGameOver evt)
		{
			AudioKit.StopMusic();
			_isGameOver = true;
			// if (_settleResult != null)
			// {
				OnSettle(evt.Rank);
			// }
			// else
			// {
			// 	UIKit.OpenPanelAsync<WaitingPanel>();
			// }
		}

		private bool _startGame;
		private void OnStartGame(GameEvents.StartCountDown evt)
		{
			
			var localPlayerId = RoomManager.Instance.LocalPlayerId;

			var playerManager = RoomManager.Instance.GetComponent<PlayerManager>();
			var shPlayerController = playerManager.PlayerList.Find(controller =>
			{
				return controller.Data.UserId == localPlayerId;
			});


			var abstractProp = shPlayerController.Props;
			PropBox.gameObject.SetActive(abstractProp != null && abstractProp.IsValid());
			if (abstractProp != null && abstractProp.IsValid())
			{
				var sprite = propAtlas.GetSprite(abstractProp.AssetConfig.SmallIcon);
				var gradeSprite = propGradeAtlas.GetSprite($"Prop_Grade_{abstractProp.Config.Grade}");
				PropIcon.sprite = sprite;
				// PropBox.sprite = gradeSprite;
				PropBox.color = new Color(1, 1, 1, 0);
				if (abstractProp.Config.UsageTimes < 0)
				{
					PropLeftUseTimes.text = "∞";
				}

				else
				{
					PropLeftUseTimes.text = abstractProp.Config.UsageTimes.ToString();
				}
				
				abstractProp.OnUseHandler += times =>
				{
					if (times > 0)
					{
						PropLeftUseTimes.text = times.ToString();
					}
					AudioKit.PlaySound(SoundName.UseProps.ToString());
				};
			}
			UIKit.HidePanel<LoadingPanel>();
			StartCountDown();
			_startGame = true;
		}

		protected void OnSettle(List<GameObject> Rank)
		{
			// if (_isGameOver)
			// {
				UIKit.OpenPanelAsync<SettlementPanel>(settlementPanel =>
				{
					// UIKit.ClosePanel<LevelPanel>();
					UIKit.ClosePanel<WaitingPanel>();
					//settlementPanel.SetPointRank(evt.Rank.ToArray());
					//settlementPanel.SetTokensRank();
					PropBox.gameObject.SetActive(false);
					// if (evt != null)
					// {
						settlementPanel.OnSettle(Rank);
					// }
					
				}, UILevel.Common, new SettlementPanelData()
				{
					PropsAtlas = propAtlas,
					RoleAtlas = roleAtlas,
					PropsGradeAtlas = propGradeAtlas
				});
			// }
			// else
			// {
			// 	_settleResult = evt;
			// }

		}

		protected override void OnClose()
		{

		}

		private void OnCoinRankChange(GameEvents.OnCoinRankChange evt)
		{
			var architecture = LevelManager.Instance.CurrentLevelController.GetArchitecture();
			var playerModel = architecture.GetModel<PlayerModel>();


			if (!CoinRank.gameObject.activeSelf)
			{
				CoinRank.gameObject.SetActive(true);	
			}
			
			foreach (var item in playerModel.PlayerModes)
			{if (item.Value.UserId == RoomManager.Instance.LocalPlayerId)
				{
					SetRankInfo(Rank1.transform, item.Value.UserName, item.Value.Coins.Value);
				}
			}
			// SetRankInfo(Rank1.transform,playerModel.PlayerModes[playerModel.CoinRank[0]].UserName, playerModel.PlayerModes[playerModel.CoinRank[0]].Coins.Value);
			// SetRankInfo(Rank2.transform,playerModel.PlayerModes[playerModel.CoinRank[1]].UserName, playerModel.PlayerModes[playerModel.CoinRank[1]].Coins.Value);
			// SetRankInfo(Rank3.transform,playerModel.PlayerModes[playerModel.CoinRank[2]].UserName, playerModel.PlayerModes[playerModel.CoinRank[2]].Coins.Value);
			Rank2.gameObject.SetActive(false);
			Rank3.gameObject.SetActive(false);	
			

			
		}

		private void SetRankInfo(Transform rank, string playerName, int coinNum)
		{
			rank.Find("PlayerName").GetComponent<TextMeshProUGUI>().text = playerName;
			rank.Find("CoinNum").GetComponent<TextMeshProUGUI>().text = coinNum.ToString();
		}

		public void StartCountDown()
		{
			
			var playerManager = RoomManager.Instance.GetComponent<PlayerManager>();
			for (int i = 0; i < playerManager.PlayerList.Count; i++)
			{
				var shPlayerController = playerManager.PlayerList[i];
				var playerName = GameObject.Instantiate(PlayerName.gameObject, PlayerName.transform.parent);
				playerName.gameObject.SetActive(true);
				playerName.GetComponent<TextMeshProUGUI>().text = shPlayerController.Data.UserName;
				PlayerHuds.Add(shPlayerController.Data.UserId, playerName);

				// if (shPlayerController.isLocalPlayer)
				// {
				// 	LocalPlayerIndicator.transform.parent = playerName.transform;
				// 	var rectTransform = LocalPlayerIndicator.GetComponent<RectTransform>();
				// 	rectTransform.localPosition = new Vector3(0, 45, 0);
				// 	LocalPlayerIndicator.gameObject.SetActive(true);
				// }
			}
			
			Num3.gameObject.SetActive(true);

			Vector3 startScale = Vector3.one * 0.1f;
			Num1.transform.localScale = startScale;

			var sequence = DOTween.Sequence();


			sequence.Append(Num3.transform.DOScale(Vector3.one, 0.25f));
			sequence.AppendCallback(() =>
			{
				AudioKit.PlaySound(SoundName.CountDown_1.ToString());
			});
			sequence.AppendInterval(0.5f);
			sequence.Append(Num3.transform.DOScale(Vector3.one* 0.1f,0.25f));
			sequence.AppendCallback(() =>
			{
				Num3.gameObject.SetActive(false);
				Num2.gameObject.SetActive(true);
				Num2.transform.localScale = startScale;
			});
			
			
			sequence.Append(Num2.transform.DOScale(Vector3.one, 0.25f));
			sequence.AppendCallback(() =>
			{
				AudioKit.PlaySound(SoundName.CountDown_2.ToString());
			});
			sequence.AppendInterval(0.5f);

			sequence.Append(Num2.transform.DOScale(Vector3.one* 0.1f,0.25f));
			sequence.AppendCallback(() =>
			{
				Num2.gameObject.SetActive(false);
				Num1.gameObject.SetActive(true);
				Num1.transform.localScale = startScale;
			});
			
			sequence.Append(Num1.transform.DOScale(Vector3.one, 0.25f));
			sequence.AppendCallback(() =>
			{
				AudioKit.PlaySound(SoundName.CountDown_3.ToString());
			});
			sequence.AppendInterval(0.5f);

			sequence.Append(Num1.transform.DOScale(Vector3.one* 0.1f,0.25f));
			sequence.AppendCallback(() =>
			{
				Num1.gameObject.SetActive(false);
				StartLabel.gameObject.SetActive(true);
				StartLabel.transform.localScale = startScale;
				AudioKit.PlayMusic(SoundName.GameBGM.ToString());
			});
			
			sequence.Append(StartLabel.transform.DOScale(Vector3.one, 0.25f));
			sequence.AppendInterval(0.2f);

			sequence.Append(StartLabel.transform.DOScale(Vector3.one* 0.1f,0.25f));
			sequence.AppendCallback(() =>
			{
				StartLabel.gameObject.SetActive(false);
	
			});

			sequence.onComplete = () =>
			{

			

			};

			sequence.Play();
		}

		private void OnPlayerGetCoin(PlayerEvents.GetCoin evt)
		{
			LogKit.I($"Player {evt.UserId} get coin {evt.Coin}");
			var rectTransform = Instantiate(CoinTip, CoinTip.transform.parent);
			
			rectTransform.Find("CoinNum").GetComponent<TextMeshProUGUI>().text = $"+{evt.Coin}";
			rectTransform.gameObject.SetActive(true);
			var find = PlayerManager.Instance.PlayerList.Find(obj =>
			{
				var controller = obj.GetComponent<SHPlayerController>();
				return controller.Data.UserId == evt.UserId;
			});

			if (find)
			{
				var position = find.transform.position + Vector3.up * 2;

				var popHud = rectTransform.gameObject.AddComponent<PopHud>();
				popHud.Pop(position, position + Vector3.up * 1.5f, 0.5f);
			}

			var shPlayerController = find.GetComponent<SHPlayerController>();
			// ToastGetCoin(shPlayerController.Data.UserName, evt.Coin);
			
			
			
		}

		private void ToastGetCoin(string playerName, int coinNum)
		{
			var instantiate = Instantiate(Toast.gameObject, Toast.transform.parent);
			
			var textMeshProUGUI = instantiate.GetComponent<TextMeshProUGUI>();

			string text = textMeshProUGUI.text;

			text = text.Replace("{1}", playerName);
			text = text.Replace("{2}", coinNum.ToString());

			textMeshProUGUI.text = text;

			instantiate.gameObject.SetActive(true);
			var rectTransform = textMeshProUGUI.GetComponent<RectTransform>();
			var rectTransformParent = rectTransform.parent as RectTransform;
			var rect = rectTransformParent.rect;

			var position = new Vector2(rect.width, Random.Range(200, rect.height - 200));
			rectTransform.anchoredPosition = position;

			rectTransform.DOAnchoredMove(new Vector3(-rect.width, position.y), 10f).onComplete += () =>
			{
				Destroy(rectTransform.gameObject);
			};
		}
		
		
		public void ReturnMain()
		{
			UIKit.ShowPanel<LoadingPanel>();
			UIKit.HidePanel<LevelPanel>();
			// Hotfix.Global.Instance.LoadingScreen.ShowLoading();
			
				
			var playerModel = LevelManager.Instance.CurrentLevelController.GetArchitecture().GetModel<PlayerModel>();
			playerModel.PlayerModes.Clear();
			
			var videoPlayerGameObject = Global.Instance.VideoPlayer.gameObject;
			var instantiate = Instantiate(videoPlayerGameObject, videoPlayerGameObject.transform.parent);
			var videoPlayer = instantiate.GetComponent<VideoPlayer>();
			Destroy(videoPlayerGameObject);
			Global.Instance.VideoPlayer = videoPlayer;
			Global.Instance.VideoPlayer.url = "https://resource.squarehero.io/HotfixAsset/WebGL/Video/Main.mp4";
			Global.Instance.VideoPlayer.targetTexture = new RenderTexture(1920, 1080, 16);
			videoPlayer.Play();
			ResourceManager.Instance.LoadSceneAsync("Scene_Main", () =>
			{
				UIKit.OpenPanelAsync<VideoPanel>(panel =>
				{
					// Destroy(videoPlayerGameObject);
					UIKit.OpenPanelAsync<MainPanel>();
					UIKit.ClosePanel<LevelPanel>();
				});
			});
		}

		private void Update()
		{
			// if (NetworkClient.active && NetworkClient.localPlayer != null)
			// {
			if (_startGame)
			{
				var playerManager = RoomManager.Instance.GetComponent<PlayerManager>();
				var playerController = playerManager.PlayerList.Find(controller =>
				{
					return controller.Data.UserId == RoomManager.Instance.LocalPlayerId;
				});

				var position = playerController.transform.position + Vector3.up * 3;
				
				var width = Screen.width;
				var height = Screen.height;
				
				if (CameraHelper.IsVisableInCamera(Global.Instance.MainCamera, position))
				{
					LocalPlayerIndicator.gameObject.SetActive(true);
					var screenPoint = Global.Instance.MainCamera.WorldToScreenPoint(position);
					
					var rectTransform = LocalPlayerIndicator.GetComponent<RectTransform>();
					
					var rect = rectTransform.parent.GetComponent<RectTransform>().rect;
				
					var rectWidth = rect.width;
					var rectHeight = rect.height;
				
					Vector3 pos = new Vector3(screenPoint.x / width * rectWidth, screenPoint.y * rectHeight / height);
					rectTransform.anchoredPosition = pos;
				}
				else
				{
					LocalPlayerIndicator.gameObject.SetActive(false);
				}

			
				for (int i = 0; i < playerManager.PlayerList.Count; i++)
				{
					var shPlayerController = playerManager.PlayerList[i];


					var playerHud = PlayerHuds[shPlayerController.Data.UserId];

					var rectTransform = playerHud.GetComponent<RectTransform>();
					
					var screenPoint = Global.Instance.MainCamera.WorldToScreenPoint(shPlayerController.transform.position + Vector3.up * 2);

					
					var rect = rectTransform.parent.GetComponent<RectTransform>().rect;

					var rectWidth = rect.width;
					var rectHeight = rect.height;

					Vector3 pos = new Vector3(screenPoint.x  / width * rectWidth, screenPoint.y * rectHeight / height);
					

					rectTransform.anchoredPosition = pos;
				}

			}
		}
	}
}
