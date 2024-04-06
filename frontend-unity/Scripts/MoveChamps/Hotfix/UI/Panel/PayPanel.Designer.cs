using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace SquareHero.Hotfix.UI
{
	// Generate Id:e54ffe81-5aca-4815-837f-25e17e822404
	public partial class PayPanel
	{
		public const string Name = "PayPanel";
		
		[SerializeField]
		public UnityEngine.UI.Button Views;
		[SerializeField]
		public UnityEngine.UI.Image Bg;
		[SerializeField]
		public UnityEngine.UI.Image Icon;
		[SerializeField]
		public TMPro.TextMeshProUGUI Price;
		[SerializeField]
		public UnityEngine.UI.Image Coin;
		[SerializeField]
		public UnityEngine.UI.Button BuyBtn;
		[SerializeField]
		public UnityEngine.UI.Button CancelBtn;
		
		private PayPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Views = null;
			Bg = null;
			Icon = null;
			Price = null;
			Coin = null;
			BuyBtn = null;
			CancelBtn = null;
			
			mData = null;
		}
		
		public PayPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		PayPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new PayPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
