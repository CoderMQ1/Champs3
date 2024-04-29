using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace champs3.Hotfix.UI
{
	// Generate Id:8e9bc5f0-0691-4ac7-bdc2-81ae7193131c
	public partial class MainPanel
	{
		public const string Name = "MainPanel";
		
		[SerializeField]
		public TMPro.TextMeshProUGUI RunSpeed;
		[SerializeField]
		public UnityEngine.UI.Image RunIcon;
		[SerializeField]
		public TMPro.TextMeshProUGUI SwimSpeed;
		[SerializeField]
		public UnityEngine.UI.Image SwimIcon;
		[SerializeField]
		public TMPro.TextMeshProUGUI ClimbSpeed;
		[SerializeField]
		public UnityEngine.UI.Image ClimbIcon;
		[SerializeField]
		public TMPro.TextMeshProUGUI FlySpeed;
		[SerializeField]
		public UnityEngine.UI.Image FlyIcon;
		[SerializeField]
		public TMPro.TextMeshProUGUI LevelNum;
		[SerializeField]
		public UnityEngine.UI.Button UpgradeBtn;
		[SerializeField]
		public UnityEngine.UI.Image TalentInfo;
		[SerializeField]
		public UnityEngine.UI.Image AddtitionAttributeIcon;
		[SerializeField]
		public TMPro.TextMeshProUGUI TalentAddition;
		[SerializeField]
		public UnityEngine.UI.Image MultiplierAttributeIcon;
		[SerializeField]
		public TMPro.TextMeshProUGUI TalentMultiplier;
		[SerializeField]
		public UnityEngine.UI.Button ElectricityBtn;
		[SerializeField]
		public TMPro.TextMeshProUGUI ElectricityNum;
		[SerializeField]
		public UnityEngine.UI.Button LeftArrow;
		[SerializeField]
		public UnityEngine.UI.Button RightArrow;
		[SerializeField]
		public UnityEngine.UI.Button StoreBtn;
		[SerializeField]
		public UnityEngine.UI.Button LuckSpinBtn;
		[SerializeField]
		public UnityEngine.UI.Button HistoryBtn;
		[SerializeField]
		public UnityEngine.UI.Button InvitationBtn;
		[SerializeField]
		public UnityEngine.UI.Button BackpackBtn;
		[SerializeField]
		public UnityEngine.UI.Button SettingBtn;
		[SerializeField]
		public TMPro.TextMeshProUGUI PlayerName;
		[SerializeField]
		public UnityEngine.UI.Button UserCodeCopyBtn;
		[SerializeField]
		public TMPro.TextMeshProUGUI UserCode;
		[SerializeField]
		public UnityEngine.UI.Button PlayBtn;
		[SerializeField]
		public TMPro.TextMeshProUGUI PlayBtnTxt;
		[SerializeField]
		public TMPro.TextMeshProUGUI Consume;
		[SerializeField]
		public UnityEngine.UI.Image EnergyIcon;
		[SerializeField]
		public champs3.Hotfix.CoinBox CoinBox;
		[SerializeField]
		public UnityEngine.UI.Button Twitter;
		[SerializeField]
		public UnityEngine.UI.Button Telegram;
		[SerializeField]
		public UnityEngine.UI.ToggleGroup GameModeSwith;
		[SerializeField]
		public UnityEngine.UI.Toggle Pve;
		[SerializeField]
		public UnityEngine.UI.Toggle Pvp;
		[SerializeField]
		public UnityEngine.UI.Image CoinNotEnough;
		[SerializeField]
		public UnityEngine.UI.Button UnApproveBtn;
		[SerializeField]
		public TMPro.TextMeshProUGUI UnApproveState;
		[SerializeField]
		public UnityEngine.UI.Button PvpPlay;
		[SerializeField]
		public TMPro.TextMeshProUGUI PvpConsume;
		[SerializeField]
		public UnityEngine.UI.Image CoinIcon;
		[SerializeField]
		public champs3.Hotfix.NotNftPanel NotNftPanel;
		[SerializeField]
		public UnityEngine.CanvasGroup BlackMask;
		
		private MainPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			RunSpeed = null;
			RunIcon = null;
			SwimSpeed = null;
			SwimIcon = null;
			ClimbSpeed = null;
			ClimbIcon = null;
			FlySpeed = null;
			FlyIcon = null;
			LevelNum = null;
			UpgradeBtn = null;
			TalentInfo = null;
			AddtitionAttributeIcon = null;
			TalentAddition = null;
			MultiplierAttributeIcon = null;
			TalentMultiplier = null;
			ElectricityBtn = null;
			ElectricityNum = null;
			LeftArrow = null;
			RightArrow = null;
			StoreBtn = null;
			LuckSpinBtn = null;
			HistoryBtn = null;
			InvitationBtn = null;
			BackpackBtn = null;
			SettingBtn = null;
			PlayerName = null;
			UserCodeCopyBtn = null;
			UserCode = null;
			PlayBtn = null;
			PlayBtnTxt = null;
			Consume = null;
			EnergyIcon = null;
			CoinBox = null;
			Twitter = null;
			Telegram = null;
			GameModeSwith = null;
			Pve = null;
			Pvp = null;
			CoinNotEnough = null;
			UnApproveBtn = null;
			UnApproveState = null;
			PvpPlay = null;
			PvpConsume = null;
			CoinIcon = null;
			NotNftPanel = null;
			BlackMask = null;
			
			mData = null;
		}
		
		public MainPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		MainPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new MainPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
