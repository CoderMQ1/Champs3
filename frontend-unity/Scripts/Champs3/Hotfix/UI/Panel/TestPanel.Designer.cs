using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace champs3.Hotfix.UI
{
	// Generate Id:e7cff9bf-c04e-45bd-91c1-c4b3283d8d29
	public partial class TestPanel
	{
		public const string Name = "TestPanel";
		
		[SerializeField]
		public UnityEngine.CanvasGroup Views;
		[SerializeField]
		public UnityEngine.UI.Button BackBtn;
		
		private TestPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Views = null;
			BackBtn = null;
			
			mData = null;
		}
		
		public TestPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		TestPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new TestPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
