using UnityEngine;
using UnityEngine.UI;
using QFramework;
using SquareHero.Core;
using UnityEngine.Video;

namespace SquareHero.Hotfix.UI
{
	public class VideoPanelData : UIPanelData
	{
	}
	public partial class VideoPanel : UIPanel
	{
		private VideoPlayer _videoPlayer;
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as VideoPanelData ?? new VideoPanelData();
			// please add init code here
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
			// VideoPlayer.url = "https://resource.squarehero.io/HotfixAsset/WebGL/Video/Main.mp4";
			Bg.texture = Global.Instance.VideoPlayer.targetTexture;
			if (GameStart.Instance.IsMobilePlatform())
			{
				Bg.uvRect = new Rect(0.22f, 0, 0.56f, 1);
			}
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
			// if (_videoPlayer)
			// {
			// 	Destroy(_videoPlayer);
			// }
		}
	}
}
