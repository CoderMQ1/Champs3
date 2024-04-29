using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace champs3.Hotfix.UI
{
	// Generate Id:c06ef29d-2da1-45f6-b040-b5eb0f82adf3
	public partial class MetaMaskPanel
	{
		public const string Name = "MetaMaskPanel";
		
		[SerializeField]
		public UnityEngine.UI.RawImage QRCode;
		[SerializeField]
		public UnityEngine.UI.Button DisConnenct;
		[SerializeField]
		public UnityEngine.UI.Button Connenct;
		[SerializeField]
		public TMPro.TextMeshProUGUI DeepLink;
		[SerializeField]
		public TMPro.TextMeshProUGUI UniversalLink;
		[SerializeField]
		public UnityEngine.UI.Button OpenDeep;
		
		private MetaMaskPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			QRCode = null;
			DisConnenct = null;
			Connenct = null;
			DeepLink = null;
			UniversalLink = null;
			OpenDeep = null;
			
			mData = null;
		}
		
		public MetaMaskPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		MetaMaskPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new MetaMaskPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
