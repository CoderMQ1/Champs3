using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace SquareHero.Hotfix.UI
{
	// Generate Id:76a05cda-530a-483b-9bfd-eeed26bc03a9
	public partial class ExchangeEnergyPanel
	{
		public const string Name = "ExchangeEnergyPanel";
		
		[SerializeField]
		public UnityEngine.CanvasGroup Views;
		[SerializeField]
		public TMPro.TextMeshProUGUI Count;
		[SerializeField]
		public TMPro.TMP_InputField ExchangeInput;
		[SerializeField]
		public UnityEngine.UI.Button ReduceBtn;
		[SerializeField]
		public UnityEngine.UI.Button AddBtn;
		[SerializeField]
		public UnityEngine.UI.Button MaxBtn;
		[SerializeField]
		public UnityEngine.UI.Button ExchangeBtn;
		[SerializeField]
		public UnityEngine.UI.Button CloseBtn;
		
		private ExchangeEnergyPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Views = null;
			Count = null;
			ExchangeInput = null;
			ReduceBtn = null;
			AddBtn = null;
			MaxBtn = null;
			ExchangeBtn = null;
			CloseBtn = null;
			
			mData = null;
		}
		
		public ExchangeEnergyPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		ExchangeEnergyPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new ExchangeEnergyPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
