using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace SquareHero.Hotfix.UI
{
	// Generate Id:609dd70c-5cbd-4b05-bc3d-e2d32b37f7c4
	public partial class ReadyPanel
	{
		public const string Name = "ReadyPanel";
		
		[SerializeField]
		public UnityEngine.UI.Image Role;
		[SerializeField]
		public UnityEngine.UI.Image PropGrade;
		[SerializeField]
		public UnityEngine.UI.Image PropIcon;
		[SerializeField]
		public TMPro.TextMeshProUGUI LevelNum;
		[SerializeField]
		public UnityEngine.UI.Image RunIcon;
		[SerializeField]
		public TMPro.TextMeshProUGUI RunSpeed;
		[SerializeField]
		public UnityEngine.UI.Image SwimIcon;
		[SerializeField]
		public TMPro.TextMeshProUGUI SwimSpeed;
		[SerializeField]
		public TMPro.TextMeshProUGUI ClimbSpeed;
		[SerializeField]
		public UnityEngine.UI.Image ClimbIcon;
		[SerializeField]
		public TMPro.TextMeshProUGUI FlySpeed;
		[SerializeField]
		public UnityEngine.UI.Image FlyIcon;
		[SerializeField]
		public UnityEngine.UI.Image TalentGradeBg;
		[SerializeField]
		public UnityEngine.UI.Image TalentIcon;
		[SerializeField]
		public UnityEngine.UI.ToggleGroup SkinContent;
		[SerializeField]
		public UnityEngine.UI.Button PropsLeftArrow;
		[SerializeField]
		public UnityEngine.UI.Button PropsRightArrow;
		[SerializeField]
		public SquareHero.Hotfix.SHScrollView PropsList;
		[SerializeField]
		public TMPro.TextMeshProUGUI PropName;
		[SerializeField]
		public TMPro.TextMeshProUGUI PropDesc;
		[SerializeField]
		public TMPro.TextMeshProUGUI SpeedIncrease;
		[SerializeField]
		public TMPro.TextMeshProUGUI AffectTime;
		[SerializeField]
		public TMPro.TextMeshProUGUI UseTimes;
		[SerializeField]
		public RectTransform Tracks;
		[SerializeField]
		public RectTransform Map;
		[SerializeField]
		public TMPro.TextMeshProUGUI SelectTrackTip;
		[SerializeField]
		public UnityEngine.UI.Button StartBtn;
		[SerializeField]
		public UnityEngine.UI.Button RoleLeftArrow;
		[SerializeField]
		public UnityEngine.UI.Button RoleRightArrow;
		[SerializeField]
		public SquareHero.Hotfix.SHScrollView RolesScrollView;
		[SerializeField]
		public UnityEngine.UI.Button Views;
		[SerializeField]
		public SquareHero.Hotfix.CoinBox CoinBox;
		[SerializeField]
		public UnityEngine.UI.Button BackBtn;
		[SerializeField]
		public RectTransform CountDownBox;
		[SerializeField]
		public TMPro.TextMeshProUGUI CountDown;
		
		private ReadyPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Role = null;
			PropGrade = null;
			PropIcon = null;
			LevelNum = null;
			RunIcon = null;
			RunSpeed = null;
			SwimIcon = null;
			SwimSpeed = null;
			ClimbSpeed = null;
			ClimbIcon = null;
			FlySpeed = null;
			FlyIcon = null;
			TalentGradeBg = null;
			TalentIcon = null;
			SkinContent = null;
			PropsLeftArrow = null;
			PropsRightArrow = null;
			PropsList = null;
			PropName = null;
			PropDesc = null;
			SpeedIncrease = null;
			AffectTime = null;
			UseTimes = null;
			Tracks = null;
			Map = null;
			SelectTrackTip = null;
			StartBtn = null;
			RoleLeftArrow = null;
			RoleRightArrow = null;
			RolesScrollView = null;
			Views = null;
			CoinBox = null;
			BackBtn = null;
			CountDownBox = null;
			CountDown = null;
			
			mData = null;
		}
		
		public ReadyPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		ReadyPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new ReadyPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
