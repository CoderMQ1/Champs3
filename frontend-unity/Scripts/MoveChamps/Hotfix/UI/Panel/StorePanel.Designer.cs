using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace SquareHero.Hotfix.UI
{
	// Generate Id:af6fd925-3bf8-4bdb-bbf7-ebdf5c47cad5
	public partial class StorePanel
	{
		public const string Name = "StorePanel";
		
		[SerializeField]
		public UnityEngine.CanvasGroup Views;
		[SerializeField]
		public SquareHero.Hotfix.CoinBox CoinBox;
		[SerializeField]
		public TMPro.TextMeshProUGUI PropsName;
		[SerializeField]
		public UnityEngine.UI.Image PropsIcon;
		[SerializeField]
		public TMPro.TextMeshProUGUI PropDesc;
		[SerializeField]
		public TMPro.TextMeshProUGUI SpeedIncrease;
		[SerializeField]
		public TMPro.TextMeshProUGUI TimeIncrease;
		[SerializeField]
		public TMPro.TextMeshProUGUI UsesIncrease;
		[SerializeField]
		public RectTransform PropsGrades;
		[SerializeField]
		public UnityEngine.UI.Button BuyBtn;
		[SerializeField]
		public TMPro.TextMeshProUGUI Price;
		[SerializeField]
		public UnityEngine.UI.Image Coin;
		[SerializeField]
		public UnityEngine.UI.Button BackBtn;
		[SerializeField]
		public SquareHero.Hotfix.SHScrollGridView PropsCommodityList;
		[SerializeField]
		public SquareHero.Hotfix.StorePaySubPanel StorePaySubPanel;
		
		private StorePanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Views = null;
			CoinBox = null;
			PropsName = null;
			PropsIcon = null;
			PropDesc = null;
			SpeedIncrease = null;
			TimeIncrease = null;
			UsesIncrease = null;
			PropsGrades = null;
			BuyBtn = null;
			Price = null;
			Coin = null;
			BackBtn = null;
			PropsCommodityList = null;
			StorePaySubPanel = null;
			
			mData = null;
		}
		
		public StorePanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		StorePanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new StorePanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
