using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace SquareHero.Hotfix.UI
{
	// Generate Id:2a4fe034-ee04-4869-a138-9c149c9ebbef
	public partial class WaitingPanel
	{
		public const string Name = "WaitingPanel";
		
		[SerializeField]
		public UnityEngine.CanvasGroup Views;
		[SerializeField]
		public UnityEngine.UI.Image LoadingMask;
		[SerializeField]
		public TMPro.TextMeshProUGUI Message;
		
		private WaitingPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Views = null;
			LoadingMask = null;
			Message = null;
			
			mData = null;
		}
		
		public WaitingPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		WaitingPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new WaitingPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
