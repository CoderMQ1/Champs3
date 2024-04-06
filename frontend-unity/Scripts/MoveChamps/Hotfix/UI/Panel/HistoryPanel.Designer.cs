using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace SquareHero.Hotfix.UI
{
	// Generate Id:3301746b-bd42-4d60-9240-8398268b9f91
	public partial class HistoryPanel
	{
		public const string Name = "HistoryPanel";
		
		[SerializeField]
		public UnityEngine.CanvasGroup Views;
		[SerializeField]
		public UnityEngine.UI.Button BackBtn;
		[SerializeField]
		public SquareHero.Hotfix.SHScrollView HistoryList;
		
		private HistoryPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Views = null;
			BackBtn = null;
			HistoryList = null;
			
			mData = null;
		}
		
		public HistoryPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		HistoryPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new HistoryPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
