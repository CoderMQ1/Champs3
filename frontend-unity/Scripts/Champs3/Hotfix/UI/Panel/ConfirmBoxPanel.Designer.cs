using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace champs3.Hotfix.UI
{
	// Generate Id:b0dd99ff-7f91-49a3-863d-61730fa554ae
	public partial class ConfirmBoxPanel
	{
		public const string Name = "ConfirmBoxPanel";
		
		[SerializeField]
		public TMPro.TextMeshProUGUI Content;
		[SerializeField]
		public UnityEngine.UI.Button Cancle;
		[SerializeField]
		public UnityEngine.UI.Button Confirm;
		
		private ConfirmBoxPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Content = null;
			Cancle = null;
			Confirm = null;
			
			mData = null;
		}
		
		public ConfirmBoxPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		ConfirmBoxPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new ConfirmBoxPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
