using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.EventSystems;

namespace SquareHero.Hotfix.UI
{
	public class GameStartPanelData : UIPanelData
	{
	}
	public partial class GameStartPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as GameStartPanelData ?? new GameStartPanelData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
				
		}
		
		protected override void OnShow()
		{
			UIKit.DisableUIInput();
			BlackMask.DOFade(0, 0.2f).onComplete = () =>
			{
				UIKit.EnableUIInput();
			};
			
			// UIKit.
		}
		
		protected override void OnHide()
		{
			// BlackMask.DOFade(1, 0.2f);
		}
		
		protected override void OnClose()
		{
		}
		
	}
}
