using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace champs3.Hotfix.UI
{
	// Generate Id:7faa0309-72e9-4fe3-ab10-549c2fe61b72
	public partial class SettingPanel
	{
		public const string Name = "SettingPanel";
		
		[SerializeField]
		public UnityEngine.CanvasGroup Views;
		[SerializeField]
		public UnityEngine.UI.Button CloseBtn;
		[SerializeField]
		public UnityEngine.UI.Slider MasterSlider;
		[SerializeField]
		public UnityEngine.UI.Button MasterAddBtn;
		[SerializeField]
		public UnityEngine.UI.Button MasterReduceBtn;
		[SerializeField]
		public UnityEngine.UI.Slider MusicSlider;
		[SerializeField]
		public UnityEngine.UI.Button MusicAddBtn;
		[SerializeField]
		public UnityEngine.UI.Button MusicReduceBtn;
		[SerializeField]
		public UnityEngine.UI.Slider SoundSlider;
		[SerializeField]
		public UnityEngine.UI.Button SoundAddBtn;
		[SerializeField]
		public UnityEngine.UI.Button SoundrReduceBtn;
		
		private SettingPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Views = null;
			CloseBtn = null;
			MasterSlider = null;
			MasterAddBtn = null;
			MasterReduceBtn = null;
			MusicSlider = null;
			MusicAddBtn = null;
			MusicReduceBtn = null;
			SoundSlider = null;
			SoundAddBtn = null;
			SoundrReduceBtn = null;
			
			mData = null;
		}
		
		public SettingPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		SettingPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new SettingPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
