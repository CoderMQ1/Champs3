using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace champs3.Hotfix.UI
{
	// Generate Id:e19a296d-ebbe-40e3-9a46-f26351ecba77
	public partial class MatchingPanel
	{
		public const string Name = "MatchingPanel";
		
		[SerializeField]
		public TMPro.TextMeshProUGUI MatchedNum;
		[SerializeField]
		public TMPro.TextMeshProUGUI MaxNum;
		[SerializeField]
		public UnityEngine.UI.Button CancelBtn;
		
		private MatchingPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			MatchedNum = null;
			MaxNum = null;
			CancelBtn = null;
			
			mData = null;
		}
		
		public MatchingPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		MatchingPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new MatchingPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
