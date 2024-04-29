using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
#if MetamaskPlugin

using champs3.Hotfix.MetaMask;

#endif
using UnityCommon.Util;

namespace champs3.Hotfix.UI
{
	public class MetaMaskPanelData : UIPanelData
	{
	}
	public partial class MetaMaskPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as MetaMaskPanelData ?? new MetaMaskPanelData();
			// please add init code here
			// MetaMaskAdatper.Instance.Initlized();

			// MetaMaskAdatper.Instance.Connect();
			
#if MetamaskPlugin
			Connenct.onClick.AddListener(() =>
			{
				LoginHelper.ConnecMetaMask();
				
			});
			
			
			DisConnenct.onClick.AddListener(() =>
			{
				MetaMaskAdatper.Instance.Disconnect();
				MetaMaskAdatper.Instance.EndSession();
			});
#endif
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
		
		
		
		
	}
}
