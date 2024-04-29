using UnityEngine;
using UnityEngine.UI;
using QFramework;
using UnityEngine.Video;

namespace champs3.Hotfix.UI
{
	public class TestPanelData : UIPanelData
	{
	}
	public partial class TestPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as TestPanelData ?? new TestPanelData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			UIKit.ShowPanel<LoadingPanel>();
			BackBtn.onClick.AddListener(() =>
			{
				ResourceManager.Instance.LoadSceneAsync("Scene_Level1Start", () =>
				{
					var instantiate = Instantiate(Global.Instance.VideoPlayer.gameObject);

					var videoPlayer = instantiate.GetComponent<VideoPlayer>();

					Destroy(Global.Instance.VideoPlayer.gameObject);
					Global.Instance.VideoPlayer = videoPlayer;
					Global.Instance.VideoPlayer.url = "https://resource.champs3.io/HotfixAsset/WebGL/Video/Main.mp4";
					Global.Instance.VideoPlayer.targetTexture = new RenderTexture(1920, 1080, 16);
					videoPlayer.Play();
					UIKit.OpenPanelAsync<VideoPanel>(panel =>
					{
						UIKit.OpenPanelAsync<MainPanel>();
						UIKit.ClosePanel<TestPanel>();
					});
				});
			});
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
