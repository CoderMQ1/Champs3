using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace SquareHero.Hotfix.UI
{
	// Generate Id:2ed93935-34f0-4896-852d-dec6046843ee
	public partial class BackpackPanel
	{
		public const string Name = "BackpackPanel";
		
		[SerializeField]
		public UnityEngine.CanvasGroup Views;
		[SerializeField]
		public UnityEngine.UI.Button BackBtn;
		[SerializeField]
		public UnityEngine.UI.ToggleGroup Tags;
		[SerializeField]
		public UnityEngine.UI.Image RoleBackpack;
		[SerializeField]
		public SquareHero.Hotfix.SHScrollGridView RoleList;
		[SerializeField]
		public UnityEngine.UI.Image PropsBackpack;
		[SerializeField]
		public SquareHero.Hotfix.SHScrollGridView PropsList;
		[SerializeField]
		public UnityEngine.UI.Image OtherBackpack;
		[SerializeField]
		public SquareHero.Hotfix.SHScrollGridView OtherList;
		[SerializeField]
		public RectTransform CurrentRoleInfo;
		[SerializeField]
		public UnityEngine.UI.RawImage RoleCardBG;
		[SerializeField]
		public UnityEngine.UI.RawImage RoleCard;
		[SerializeField]
		public TMPro.TextMeshProUGUI CurrentRoleEnergy;
		[SerializeField]
		public RectTransform Stars;
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
		public UnityEngine.UI.Image ClimbIcon;
		[SerializeField]
		public TMPro.TextMeshProUGUI ClimbSpeed;
		[SerializeField]
		public UnityEngine.UI.Image FlyIcon;
		[SerializeField]
		public TMPro.TextMeshProUGUI FlySpeed;
		[SerializeField]
		public UnityEngine.UI.Button UseRoleBtn;
		[SerializeField]
		public RectTransform CurrentPropsInfo;
		[SerializeField]
		public RectTransform PropInfoBox;
		[SerializeField]
		public TMPro.TextMeshProUGUI PropsName;
		[SerializeField]
		public UnityEngine.UI.Image PropsIcon;
		[SerializeField]
		public TMPro.TextMeshProUGUI PropsDesc;
		[SerializeField]
		public TMPro.TextMeshProUGUI SpeedIncrease;
		[SerializeField]
		public TMPro.TextMeshProUGUI TimeIncrease;
		[SerializeField]
		public TMPro.TextMeshProUGUI UsesIncrease;
		[SerializeField]
		public UnityEngine.UI.Image PropsGrade;
		[SerializeField]
		public TMPro.TextMeshProUGUI PropsGradeLabel;
		[SerializeField]
		public RectTransform CurrentOhterInfo;
		[SerializeField]
		public TMPro.TextMeshProUGUI OtherName;
		[SerializeField]
		public UnityEngine.UI.Image RotateBg;
		[SerializeField]
		public UnityEngine.UI.Image OthersIcon;
		[SerializeField]
		public TMPro.TextMeshProUGUI OhterDesc;
		[SerializeField]
		public UnityEngine.UI.Button SynthesisBtn;
		
		private BackpackPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Views = null;
			BackBtn = null;
			Tags = null;
			RoleBackpack = null;
			RoleList = null;
			PropsBackpack = null;
			PropsList = null;
			OtherBackpack = null;
			OtherList = null;
			CurrentRoleInfo = null;
			RoleCardBG = null;
			RoleCard = null;
			CurrentRoleEnergy = null;
			Stars = null;
			LevelNum = null;
			RunIcon = null;
			RunSpeed = null;
			SwimIcon = null;
			SwimSpeed = null;
			ClimbIcon = null;
			ClimbSpeed = null;
			FlyIcon = null;
			FlySpeed = null;
			UseRoleBtn = null;
			CurrentPropsInfo = null;
			PropInfoBox = null;
			PropsName = null;
			PropsIcon = null;
			PropsDesc = null;
			SpeedIncrease = null;
			TimeIncrease = null;
			UsesIncrease = null;
			PropsGrade = null;
			PropsGradeLabel = null;
			CurrentOhterInfo = null;
			OtherName = null;
			RotateBg = null;
			OthersIcon = null;
			OhterDesc = null;
			SynthesisBtn = null;
			
			mData = null;
		}
		
		public BackpackPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		BackpackPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new BackpackPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
