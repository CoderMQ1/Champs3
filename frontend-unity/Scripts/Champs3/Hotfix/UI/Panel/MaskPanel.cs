using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace champs3.Hotfix.UI
{
	public class MaskPanelData : UIPanelData
	{
	}
	public partial class MaskPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as MaskPanelData ?? new MaskPanelData();
			// please add init code here
			Views.alpha = 0;
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
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


		public void FadeIn(Action callback)
		{
			UIKit.DisableUIInput();
			Views.DOFade(0, 1, 0.4f).onComplete += () =>
			{
				UIKit.EnableUIInput();
				callback?.Invoke();
			};
		}
		
		public void FadeOut(Action callback = null)
		{
			UIKit.DisableUIInput();
			Views.DOFade(1, 0, 0.4f).onComplete += () =>
			{
				UIKit.EnableUIInput();
				callback?.Invoke();
			};
		}
	}
}
