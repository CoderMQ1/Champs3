using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace champs3.Hotfix.UI
{
	// Generate Id:bb95468c-f174-4960-bb02-022854b7575a
	public partial class MaskPanel
	{
		public const string Name = "MaskPanel";
		
		[SerializeField]
		public UnityEngine.CanvasGroup Views;
		
		private MaskPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Views = null;
			
			mData = null;
		}
		
		public MaskPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		MaskPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new MaskPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
