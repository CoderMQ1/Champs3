using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace SquareHero.Hotfix.UI
{
	// Generate Id:9bcc212f-3107-47af-9784-ec0b2351a7c7
	public partial class LevelPanel
	{
		public const string Name = "LevelPanel";
		
		[SerializeField]
		public UnityEngine.UI.Image Num1;
		[SerializeField]
		public UnityEngine.UI.Image Num2;
		[SerializeField]
		public UnityEngine.UI.Image Num3;
		[SerializeField]
		public UnityEngine.UI.Image StartLabel;
		[SerializeField]
		public UnityEngine.UI.Image PropBox;
		[SerializeField]
		public UnityEngine.UI.Image PropIcon;
		[SerializeField]
		public UnityEngine.UI.Image PropUsedBg;
		[SerializeField]
		public TMPro.TextMeshProUGUI PropLeftUseTimes;
		[SerializeField]
		public UnityEngine.UI.Image CoinRank;
		[SerializeField]
		public UnityEngine.UI.Image Rank1;
		[SerializeField]
		public UnityEngine.UI.Image Rank2;
		[SerializeField]
		public UnityEngine.UI.Image Rank3;
		[SerializeField]
		public RectTransform CoinTip;
		[SerializeField]
		public TMPro.TextMeshProUGUI Toast;
		[SerializeField]
		public UnityEngine.UI.Image LocalPlayerIndicator;
		[SerializeField]
		public TMPro.TextMeshProUGUI PlayerName;
		[SerializeField]
		public UnityEngine.UI.Button BackBtn;
		
		private LevelPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Num1 = null;
			Num2 = null;
			Num3 = null;
			StartLabel = null;
			PropBox = null;
			PropIcon = null;
			PropUsedBg = null;
			PropLeftUseTimes = null;
			CoinRank = null;
			Rank1 = null;
			Rank2 = null;
			Rank3 = null;
			CoinTip = null;
			Toast = null;
			LocalPlayerIndicator = null;
			PlayerName = null;
			BackBtn = null;
			
			mData = null;
		}
		
		public LevelPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		LevelPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new LevelPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
