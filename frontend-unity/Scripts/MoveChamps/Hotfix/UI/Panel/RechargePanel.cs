using UnityEngine;
using UnityEngine.UI;
using QFramework;
using SquareHero.Hotfix.Main;
using SquareHero.Hotfix.Model;
using UnityCommon.Util;

namespace SquareHero.Hotfix.UI
{
	public class RechargePanelData : UIPanelData
	{
	}
	public partial class RechargePanel : UIPanel
	{
		protected WalletData WalletData;
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as RechargePanelData ?? new RechargePanelData();
			// please add init code here
			Views.alpha = 0;
			CloseBtn.onClick.AddListener(() =>
			{
				Views.DOFade(1, 0, 0.4f).onComplete = () =>
				{
					UIKit.ClosePanel(this);
				};
			});
			
			TransferBtn.onClick.AddListener(() =>
			{
				Recharge();
			});
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
			var enumerator = HttpHelper.Get(GameUrlConstValue.WalletData.Url(), result =>
			{

				WalletDataResponse response = JsonUtil.FromJson<WalletDataResponse>(result);
				if (response.code == 0)
				{
					WalletData = response.data;
					WalletBalance.text = $"Balance : {WalletData.token_balance}";
				}
				
				GameBalance.text = $"Balance : {MainController.Instance.GetArchitecture().GetModel<MainModel>().Bag.Diamond}";
				Views.DOFade(0, 1, 0.4f);
			});
			StartCoroutine(enumerator);
		}

		private void Recharge()
		{
			var rechargeValue = float.Parse(GameInput.text);
			if (rechargeValue <= 0 || WalletData.token_balance <= 0)
			{
				return;
			}
			Loading loading = new Loading();
			HttpJsonPostNode rechargeNode = new HttpJsonPostNode(this, GameUrlConstValue.Rechange.Url(),
				new RechargeData()
				{
					token_name = "FPU",
					contract_address = WalletData.contrace_address,
					amount = rechargeValue
				}.ToJson());
			HttpGetNode walletDataNode = new HttpGetNode(this, GameUrlConstValue.WalletData.Url());
			GetPlayerAssetNode node = new GetPlayerAssetNode(this);
				
			loading.AddNode(rechargeNode);
			loading.AddNode(walletDataNode);
			loading.AddNode(node);

			loading.OnCompleted = () =>
			{
				WalletDataResponse response = JsonUtil.FromJson<WalletDataResponse>(walletDataNode.GetResult());
				if (response.code == 0)
				{
					WalletData = response.data;
					WalletBalance.text = $"Balance : {WalletData.token_balance}";
				}

				GameBalance.text = $"Balance : {MainController.Instance.GetArchitecture().GetModel<MainModel>().Bag.Diamond}";
			};
			loading.Start();
		}

		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}
	}
}
