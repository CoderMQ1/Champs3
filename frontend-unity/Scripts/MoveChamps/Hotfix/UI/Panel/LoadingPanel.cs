using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace SquareHero.Hotfix.UI
{
	public class LoadingPanelData : UIPanelData
	{
	}
	public partial class LoadingPanel : UIPanel
	{

		private Sequence sequence;
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as LoadingPanelData ?? new LoadingPanelData();
			// please add init code here
			
			
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{



		}
		
		protected override void OnShow()
		{
			if (sequence != null )
			{
				sequence.Play();
			}
			else
			{
				sequence = DOTween.Sequence();
				LoadingLabel.text = "Loading.";
				sequence.AppendInterval(0.4f);
				sequence.InsertCallback(0.4f,() =>
				{
					LoadingLabel.text = "Loading..";
				});
				sequence.AppendInterval(0.4f);
				sequence.InsertCallback(0.8f,() =>
				{
					LoadingLabel.text = "Loading...";
				});
				sequence.AppendInterval(0.4f);
				sequence.InsertCallback(1.2f,() =>
				{
					LoadingLabel.text = "Loading.";
				});
				sequence.SetLoops(-1);
				sequence.Play();
			}
		}
		
		protected override void OnHide()
		{
			sequence.Pause();
		}
		
		protected override void OnClose()
		{
			sequence.SetLoops(0);
			sequence.Kill(true);
		}
	}
}
