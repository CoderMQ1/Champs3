using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace champs3.Hotfix.UI
{
	// Generate Id:75fef319-ddb3-48b2-9e53-d3547c0c4192
	public partial class VideoPanel
	{
		public const string Name = "VideoPanel";
		
		[SerializeField]
		public UnityEngine.CanvasGroup Views;
		[SerializeField]
		public UnityEngine.UI.RawImage Bg;
		
		private VideoPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Views = null;
			Bg = null;
			
			mData = null;
		}
		
		public VideoPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		VideoPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new VideoPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
