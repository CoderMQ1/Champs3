using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace champs3.Hotfix.UI
{
	// Generate Id:66adafdc-4c62-479e-b9dc-0fb6e36e5485
	public partial class RoleUpgradePanel
	{
		public const string Name = "RoleUpgradePanel";
		
		[SerializeField]
		public UnityEngine.CanvasGroup Views;
		[SerializeField]
		public UnityEngine.UI.RawImage RoleIcon;
		[SerializeField]
		public TMPro.TextMeshProUGUI CurrentLevel;
		[SerializeField]
		public TMPro.TextMeshProUGUI RunSpeed;
		[SerializeField]
		public TMPro.TextMeshProUGUI SwimmingSpeed;
		[SerializeField]
		public TMPro.TextMeshProUGUI ClimbingSpeed;
		[SerializeField]
		public TMPro.TextMeshProUGUI FlyingSpeed;
		[SerializeField]
		public TMPro.TextMeshProUGUI UpgradeTip;
		[SerializeField]
		public UnityEngine.UI.Image AfterUpgradedTip;
		[SerializeField]
		public UnityEngine.UI.Image RewardInfo;
		[SerializeField]
		public TMPro.TextMeshProUGUI RewardCount;
		[SerializeField]
		public UnityEngine.UI.Button UpgradeBtn;
		[SerializeField]
		public TMPro.TextMeshProUGUI UpgradePrice;
		[SerializeField]
		public UnityEngine.UI.Image NextLevelInfo;
		[SerializeField]
		public TMPro.TextMeshProUGUI NextLevel;
		[SerializeField]
		public TMPro.TextMeshProUGUI NextRunSpeed;
		[SerializeField]
		public TMPro.TextMeshProUGUI NextSwimmingSpeed;
		[SerializeField]
		public TMPro.TextMeshProUGUI NextClimbingSpeed;
		[SerializeField]
		public TMPro.TextMeshProUGUI NextFlyingSpeed;
		[SerializeField]
		public UnityEngine.UI.Image LoadingMask;
		[SerializeField]
		public UnityEngine.UI.Button BackBtn;
		[SerializeField]
		public champs3.Hotfix.CoinBox CoinBox;
		
		private RoleUpgradePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Views = null;
			RoleIcon = null;
			CurrentLevel = null;
			RunSpeed = null;
			SwimmingSpeed = null;
			ClimbingSpeed = null;
			FlyingSpeed = null;
			UpgradeTip = null;
			AfterUpgradedTip = null;
			RewardInfo = null;
			RewardCount = null;
			UpgradeBtn = null;
			UpgradePrice = null;
			NextLevelInfo = null;
			NextLevel = null;
			NextRunSpeed = null;
			NextSwimmingSpeed = null;
			NextClimbingSpeed = null;
			NextFlyingSpeed = null;
			LoadingMask = null;
			BackBtn = null;
			CoinBox = null;
			
			mData = null;
		}
		
		public RoleUpgradePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		RoleUpgradePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new RoleUpgradePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
