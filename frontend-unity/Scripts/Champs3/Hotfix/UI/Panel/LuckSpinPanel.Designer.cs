using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace champs3.Hotfix.UI
{
	// Generate Id:b869a239-081b-4917-b1e8-345db94799ec
	public partial class LuckSpinPanel
	{
		public const string Name = "LuckSpinPanel";
		
		[SerializeField]
		public UnityEngine.CanvasGroup Views;
		[SerializeField]
		public UnityEngine.UI.Button SpinButton;
		[SerializeField]
		public TMPro.TextMeshProUGUI SpinTimes;
		[SerializeField]
		public RectTransform Rewards;
		[SerializeField]
		public champs3.Hotfix.CoinBox CoinBox;
		[SerializeField]
		public UnityEngine.UI.Button ExchangeBtn;
		[SerializeField]
		public TMPro.TextMeshProUGUI LeftNum;
		[SerializeField]
		public UnityEngine.UI.Slider SeasonPointProgress;
		[SerializeField]
		public TMPro.TextMeshProUGUI SeasonPointProgressTxt;
		[SerializeField]
		public TMPro.TextMeshProUGUI BonusPool;
		[SerializeField]
		public TMPro.TextMeshProUGUI Jackpot;
		[SerializeField]
		public UnityEngine.UI.Button BackBtn;
		[SerializeField]
		public UnityEngine.ParticleSystem ItemSparkleSoftYellow;
		[SerializeField]
		public UnityEngine.UI.Image JackPot;
		[SerializeField]
		public champs3.Hotfix.ReceiveRewardSubPanel ReceiveRewardSubPanel;
		[SerializeField]
		public UnityEngine.UI.Image Mask;
		
		private LuckSpinPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Views = null;
			SpinButton = null;
			SpinTimes = null;
			Rewards = null;
			CoinBox = null;
			ExchangeBtn = null;
			LeftNum = null;
			SeasonPointProgress = null;
			SeasonPointProgressTxt = null;
			BonusPool = null;
			Jackpot = null;
			BackBtn = null;
			ItemSparkleSoftYellow = null;
			JackPot = null;
			ReceiveRewardSubPanel = null;
			Mask = null;
			
			mData = null;
		}
		
		public LuckSpinPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		LuckSpinPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new LuckSpinPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
