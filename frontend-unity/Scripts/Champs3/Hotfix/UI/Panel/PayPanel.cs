using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace champs3.Hotfix.UI
{
	public class PayPanelData : UIPanelData
	{
		public PropsData PropsData;
		public Sprite PropsIcon;
		public Sprite PropsGrade;
	}
	public partial class PayPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as PayPanelData ?? new PayPanelData();
			// please add init code here
			
			BuyBtn.onClick.AddListener(Buy);
			// CancelBtn.onClick.AddListener();
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			Icon.sprite = mData.PropsIcon;
			Bg.sprite = mData.PropsGrade;
			Price.text = mData.PropsData.price_amount.ToString();
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}

		public void Buy()
		{
			
		}
	}
}
