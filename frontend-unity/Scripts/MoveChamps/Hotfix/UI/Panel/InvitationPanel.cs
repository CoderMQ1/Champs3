using UnityEngine;
using UnityEngine.UI;
using QFramework;
using SquareHero.Core;
using SquareHero.Hotfix.Main;
using SquareHero.Hotfix.Model;
using UnityCommon.Util;

namespace SquareHero.Hotfix.UI
{
	public class InvitationPanelData : UIPanelData
	{
	}
	public partial class InvitationPanel : UIPanel
	{
		private readonly string InvitationLink = "https://minttest19.runx.xyz/#/mint/?refer=";
		private readonly string InvitationLink2 = "https%3A%2F%2Fminttest19.runx.xyz%2F%23%2Fmint%2F%3Frefer%3D{account}";

		private readonly string TipStr = "You have got  <color=#FCFF00><size=48>{num}</size> </color> batteries.";
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as InvitationPanelData ?? new InvitationPanelData();
			// please add init code here
			
			BackBtn.onClick.AddListener(() =>
			{
				AudioKit.PlaySound(SoundName.clickbutton.ToString());

				var maskPanel = UIKit.GetPanel<MaskPanel>();
				maskPanel.FadeIn(() =>
				{
					UIKit.ClosePanel<InvitationPanel>();
					
					maskPanel.FadeOut();
				});
			});
			

			var mainModel = MainController.Instance.GetArchitecture().GetModel<MainModel>();
			Link.text = InvitationLink + Web3.Instance.Account;
			
			Twitter.onClick.AddListener(() =>
			{
				// Global.Instance.ShareToTwitter.ShareMessage($"Runx is a fun, fair launch, decentralized light strategy game that can be played on browsers! Click on the link and earn RTC with me! &url={InvitationLink2.Replace("{account}", Web3.Instance.Account)} &via=runxxyz");
				Global.Instance.ShareToTwitter.ShareMessage($"Free Charge &url={InvitationLink2.Replace("{account}", Web3.Instance.Account)} &via=runxxyz");
			});
			CopyBtn.onClick.AddListener(() =>
			{
				SHWebGLPlugin.CopyToClipboard(InvitationLink + Web3.Instance.Account);
			});
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{

			Loading loading = new Loading();

			HttpGetNode httpGetNode = new HttpGetNode(this,GameUrlConstValue.InviteReward.Url());
			loading.AddNode(httpGetNode);

			loading.OnCompleted += () =>
			{
				FillDate();

				var rewardResponse = JsonUtil.FromJson<InviteRewardResponse>(httpGetNode.GetResult());
				Tip.text = TipStr.Replace("{num}", rewardResponse.Data.total_reward_num.ToString());
			};
			loading.Start();
		}
		
		protected void FillDate()
		{
			var maskPanel = UIKit.GetPanel<MaskPanel>();
			maskPanel.FadeOut(null);
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}
	}
}
