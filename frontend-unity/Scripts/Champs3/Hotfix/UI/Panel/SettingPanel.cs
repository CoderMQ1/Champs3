using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace champs3.Hotfix.UI
{
	public class SettingPanelData : UIPanelData
	{
	}
	public partial class SettingPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as SettingPanelData ?? new SettingPanelData();
			// please add init code here
			Views.alpha = 0;
			
			CloseBtn.onClick.AddListener(() =>
			{
				Views.DOFade(1, 0, 0.4f).onComplete += () =>
				{
					UIKit.ClosePanel(this);
				};
			});
			
			MasterSlider.onValueChanged.AddListener(value =>
			{
				AudioKit.Settings.MasterVolume.Value = value;
				AudioKit.MusicPlayer.SetVolume(AudioKit.Settings.MasterVolume.Value * AudioKit.Settings.MusicVolume.Value);
			});
			
			MusicSlider.onValueChanged.AddListener(value =>
			{
				AudioKit.Settings.MusicVolume.Value = value;
				AudioKit.MusicPlayer.SetVolume(AudioKit.Settings.MasterVolume.Value * AudioKit.Settings.MusicVolume.Value);
			});
			
			SoundSlider.onValueChanged.AddListener(value =>
			{
				AudioKit.Settings.SoundVolume.Value = value;
			});
			
			
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
			MasterSlider.value = AudioKit.Settings.MasterVolume.Value;
			MusicSlider.value = AudioKit.Settings.MusicVolume.Value;
			SoundSlider.value = AudioKit.Settings.SoundVolume.Value;
			Views.DOFade(0, 1, 0.4f);

		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}
	}
}
