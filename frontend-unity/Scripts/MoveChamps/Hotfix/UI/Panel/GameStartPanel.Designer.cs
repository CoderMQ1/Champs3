using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace SquareHero.Hotfix.UI
{
	// Generate Id:82dc3ee5-0411-4b04-ad82-556ed0faef88
	public partial class GameStartPanel
	{
		public const string Name = "GameStartPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button StartGame;
		[SerializeField]
		public UnityEngine.UI.Image BlackMask;
		[SerializeField]
		public SquareHero.Hotfix.LoginFailPanel LoginFailPanel;
		[SerializeField]
		public UnityEngine.CanvasGroup Views;
		[SerializeField]
		public SquareHero.Hotfix.DeviceNotSupportPanel DeviceNotSupportPanel;
		
		private GameStartPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			StartGame = null;
			BlackMask = null;
			LoginFailPanel = null;
			Views = null;
			DeviceNotSupportPanel = null;
			
			mData = null;
		}
		
		public GameStartPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		GameStartPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new GameStartPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
