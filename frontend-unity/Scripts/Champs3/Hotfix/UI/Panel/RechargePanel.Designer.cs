using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace champs3.Hotfix.UI
{
	// Generate Id:cf45c232-be38-4ffd-8d35-10aa9028df96
	public partial class RechargePanel
	{
		public const string Name = "RechargePanel";
		
		[SerializeField]
		public UnityEngine.CanvasGroup Views;
		[SerializeField]
		public TMPro.TMP_InputField WalletInput;
		[SerializeField]
		public TMPro.TextMeshProUGUI WalletBalance;
		[SerializeField]
		public UnityEngine.UI.Button MacBtn;
		[SerializeField]
		public TMPro.TMP_InputField GameInput;
		[SerializeField]
		public TMPro.TextMeshProUGUI GameBalance;
		[SerializeField]
		public UnityEngine.UI.Button TransferBtn;
		[SerializeField]
		public UnityEngine.UI.Button CloseBtn;
		
		private RechargePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Views = null;
			WalletInput = null;
			WalletBalance = null;
			MacBtn = null;
			GameInput = null;
			GameBalance = null;
			TransferBtn = null;
			CloseBtn = null;
			
			mData = null;
		}
		
		public RechargePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		RechargePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new RechargePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
