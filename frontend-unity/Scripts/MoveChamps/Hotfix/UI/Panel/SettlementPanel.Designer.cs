using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace SquareHero.Hotfix.UI
{
	// Generate Id:50cdd015-ce68-4ce9-b9f3-6b322adfda33
	public partial class SettlementPanel
	{
		public const string Name = "SettlementPanel";
		
		[SerializeField]
		public RectTransform Views;
		[SerializeField]
		public UnityEngine.UI.Image RewardsLabel;
		[SerializeField]
		public UnityEngine.UI.Button ContinueBtn;
		[SerializeField]
		public TMPro.TextMeshProUGUI CountDown;
		[SerializeField]
		public UnityEngine.UI.Image LeftRank;
		[SerializeField]
		public RectTransform PointRanks;
		[SerializeField]
		public UnityEngine.UI.Button LuckDrawBtn;
		[SerializeField]
		public UnityEngine.UI.Image RightRank;
		[SerializeField]
		public RectTransform TokensRanks;
		[SerializeField]
		public UnityEngine.UI.Button ShareBtn;
		[SerializeField]
		public UnityEngine.UI.Button ReturnBtn;
		
		private SettlementPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Views = null;
			RewardsLabel = null;
			ContinueBtn = null;
			CountDown = null;
			LeftRank = null;
			PointRanks = null;
			LuckDrawBtn = null;
			RightRank = null;
			TokensRanks = null;
			ShareBtn = null;
			ReturnBtn = null;
			
			mData = null;
		}
		
		public SettlementPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		SettlementPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new SettlementPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
