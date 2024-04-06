using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace SquareHero.Hotfix.UI
{
	// Generate Id:a094aee0-4f73-4b36-b104-65986a818c8a
	public partial class LoadingPanel
	{
		public const string Name = "LoadingPanel";
		
		[SerializeField]
		public TMPro.TextMeshProUGUI LoadingLabel;
		
		private LoadingPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			LoadingLabel = null;
			
			mData = null;
		}
		
		public LoadingPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		LoadingPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new LoadingPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
