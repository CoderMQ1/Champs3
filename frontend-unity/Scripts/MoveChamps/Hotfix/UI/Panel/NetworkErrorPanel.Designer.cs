using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace SquareHero.Hotfix.UI
{
	// Generate Id:a6b2baa1-0a80-4b75-b554-6eba5024d5e8
	public partial class NetworkErrorPanel
	{
		public const string Name = "NetworkErrorPanel";
		
		[SerializeField]
		public TMPro.TextMeshProUGUI Content;
		[SerializeField]
		public UnityEngine.UI.Button Cancle;
		[SerializeField]
		public UnityEngine.UI.Button Confirm;
		
		private NetworkErrorPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Content = null;
			Cancle = null;
			Confirm = null;
			
			mData = null;
		}
		
		public NetworkErrorPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		NetworkErrorPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new NetworkErrorPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
