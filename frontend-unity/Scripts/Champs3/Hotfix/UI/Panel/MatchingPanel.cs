using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using champs3.Hotfix.Events;
using champs3.Hotfix.GameLogic;
using champs3.Hotfix.Generate;
using champs3.Hotfix.Main;
using champs3.Hotfix.Map;
using champs3.Hotfix.Model;
using champs3.Hotfix.Net;
using Random = UnityEngine.Random;

namespace champs3.Hotfix.UI
{
	public class MatchingPanelData : UIPanelData
	{
		public int GameMode = 2;
		public float PvPCost;

	}
	public partial class MatchingPanel : UIPanel
	{
		private bool _isMatching;
		private int _matched;
		private int _maxMatch = 5;
		private float _matchedTime;
		private int _mapId;
		private long _roomId;
		public long _localPlayerId;
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as MatchingPanelData ?? new MatchingPanelData();
			// please add init code here
			
			CancelBtn.onClick.AddListener(() =>
			{
				// SHNetManager.Instance.Send(KeepOpCode.OP_C2R_ExitRacingMatch, new C2R_ExitRacingMatch());
			});
			_localPlayerId = MainController.Instance.GetArchitecture().GetModel<MainModel>().Player.UserId;
			_roomId = 111233;
			// _mapId = 1;
			_mapId = Random.Range(1, 16);
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			
		}
		
		protected override void OnShow()
		{
			_isMatching = true;
			_matched = 1;
			RandomTime();
			MatchedNum.text = _matched.ToString();
			MaxNum.text = _maxMatch.ToString();
			var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			LogKit.I($"Start Match : {now}");


			StartCoroutine(SimulateMatch());

			// champs3NetManager.RegisterHandler<R2C_StartMatchRacing>(KeepOpCode.OP_R2C_StartMatchRacing, OpenMatch);
			// champs3NetManager.RegisterHandler<MatchProgressMessage>(KeepOpCode.OP_MatchProgressMessage, MatchProgressMessage);
			// champs3NetManager.RegisterHandler<RoomMatchSuccessMessage>(KeepOpCode.OP_RoomMatchSuccessMessage, MactchSuccess);
			// champs3NetManager.RegisterHandler<R2C_ExitRacingMatch>(KeepOpCode.OP_R2C_ExitRacingMatch, ExitMatch);
			// champs3NetManager.RegisterHandler<RacingGameStartMessage>(KeepOpCode.OP_RacingGameStart, EnterGameScene);
		}

		private IEnumerator SimulateMatch()
		{
			float waitTime = 2f;
			while (_matched < 5)
			{
				yield return new WaitForSeconds(waitTime);

				waitTime = Random.Range(1, 3);
				_matched++;
				MatchedNum.text = _matched.ToString();
				MaxNum.text = _maxMatch.ToString();
			}

			
			UIKit.OpenPanelAsync<ReadyPanel>(panel =>
			{
				UIKit.ClosePanel<MatchingPanel>();
			}, UILevel.Common, new ReadyPanelData()
			{
				GameMode = 2,
				MapId = _mapId,
				RoomId = _roomId,
				PvPCost = mData.PvPCost
			});
		}

		private void OpenMatch(R2C_StartMatchRacing evt)
		{
			LogKit.I($"Receive Msg {evt} duplicate");
		}

		private void ExitMatch(R2C_ExitRacingMatch evt)
		{
			_isMatching = false;
			_matched = 0;
			AudioKit.PlaySound(SoundName.clickbutton.ToString());
			UIKit.ShowPanel<MainPanel>();
			UIKit.ClosePanel<MatchingPanel>();
		}

		protected void MatchProgressMessage(MatchProgressMessage evt)
		{
			if (evt.current > _matched)
			{
				_matched = (int)evt.current;
			}
			MatchedNum.text = _matched.ToString();
			MaxNum.text = evt.target.ToString();
		}

		protected void MactchSuccess(RoomMatchSuccessMessage evt)
		{
			_mapId = (int)evt.MapId;
			_roomId = evt.RoomId;
			UIKit.OpenPanelAsync<ReadyPanel>(panel =>
			{
				UIKit.ClosePanel<MatchingPanel>();
			}, UILevel.Common, new ReadyPanelData()
			{
				GameMode = mData.GameMode,
				MapId = _mapId,
				RoomId = _roomId,
				PvPCost = mData.PvPCost
			});
		}

		protected override void OnHide()
		{
			// champs3NetManager.UnRegisterHandler<R2C_StartMatchRacing>(KeepOpCode.OP_R2C_StartMatchRacing, OpenMatch);
			// champs3NetManager.UnRegisterHandler<MatchProgressMessage>(KeepOpCode.OP_MatchProgressMessage, MatchProgressMessage);
			// champs3NetManager.UnRegisterHandler<RoomMatchSuccessMessage>(KeepOpCode.OP_RoomMatchSuccessMessage, MactchSuccess);
			// champs3NetManager.UnRegisterHandler<R2C_ExitRacingMatch>(KeepOpCode.OP_R2C_ExitRacingMatch, ExitMatch);
			// champs3NetManager.UnRegisterHandler<RacingGameStartMessage>(KeepOpCode.OP_RacingGameStart, EnterGameScene);
		}
		
		protected override void OnClose()
		{
		}

		private void Update()
		{
			// if (_matched < _maxMatch)
			// {
			// 	if (_matchedTime > 0)
			// 	{
			// 		_matchedTime -= Time.deltaTime;
			// 		if (_matchedTime <= 0)
			// 		{
			// 			_matched++;
			// 			MatchedNum.text = _matched.ToString();
			// 			RandomTime();
			//
			// 			if (_matched == _maxMatch)
			// 			{
			// 				ActionKit.Delay(0.5f, () =>
			// 				{
			// 					EnterGame(123456);
			// 				}).Start(this);
			// 			}
			// 		}
			// 	}
			// }
		}

		protected void RandomTime()
		{
			_matchedTime = Random.Range(0.5f, 1.5f);
		}

		protected void ToReadyPanel(long roomId, int readyTime, int mapId)
		{

			// UIKit.OpenPanelAsync<ReadyPanel>(panel =>
			// {
			// 	UIKit.HidePanel<MainPanel>();
			// 	UIKit.ClosePanel<MatchingPanel>();
			// },UILevel.Common,new ReadyPanelData()
			// 	{
			// 		RoomId = roomId,
			// 		ReadyTime =  readyTime,
			// 		MapId = mapId
			// 	}
			// );

		}
		
		private void EnterGameScene(RacingGameStartMessage evt)
		{
			LogKit.I("EnterGameScene");
			// AudioKit.StopMusic();
            
			var loadingPanel = UIKit.GetPanel<LoadingPanel>();
			if (loadingPanel.State == PanelState.Hide)
			{
				UIKit.ShowPanel<LoadingPanel>();
			}
			UIKit.ClosePanel<MainPanel>();
			UIKit.ClosePanel<ReadyPanel>();
			UIKit.ClosePanel<MatchingPanel>();
			UIKit.ClosePanel<VideoPanel>();
            Global.Instance.VideoPlayer.Pause();
			ResourceManager.Instance.LoadSceneAsync("Scene_Level1", () =>
			{
				LogKit.I("EnterGameScene Scene_Level1Start loaded");
				UIKit.OpenPanelAsync<LevelPanel>(panel =>
				{

					UIKit.ClosePanel<ReadyPanel>();
				}, UILevel.Common, new LevelPanelData()
				{
					RoomId = _roomId,
					UserId = _localPlayerId,
					MapId = _mapId
				});
			});

			
		}
		

	}
}
