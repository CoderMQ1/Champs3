using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using SquareHero.Hotfix.Main;
using SquareHero.Hotfix.Model;
using UnityCommon.Util;

namespace SquareHero.Hotfix.UI
{
	public class ExchangeEnergyPanelData : UIPanelData
	{
		public long RoleToken;
	}
	public partial class ExchangeEnergyPanel : UIPanel
	{
		private MainModel _mainModel;
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as ExchangeEnergyPanelData ?? new ExchangeEnergyPanelData();
			// please add init code here
			
			
			AddBtn.onClick.AddListener(() =>
			{
				int count = int.Parse(ExchangeInput.text);

				int add = (int)Mathf.Min(count + 1, _mainModel.Bag.GetAssetNumber(ConstValue.EnergyDrinkId));
				ExchangeInput.text = add.ToString();
			});
			
			ReduceBtn.onClick.AddListener(() =>
			{
				int count = int.Parse(ExchangeInput.text);

				int reduce = (int)Mathf.Max(count - 1, 0);
				ExchangeInput.text = reduce.ToString();
			});
			
			MaxBtn.onClick.AddListener(() =>
			{
				ExchangeInput.text = _mainModel.Bag.GetAssetNumber(ConstValue.EnergyDrinkId).ToString();
			});
			
			ExchangeBtn.onClick.AddListener(Exchange);

			CloseBtn.onClick.AddListener(Close);
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
			_mainModel = MainController.Instance.GetModel<MainModel>();
			Count.text = _mainModel.Bag.GetAssetNumber(ConstValue.EnergyDrinkId).ToString();
			ExchangeInput.text = "0";
			Web3.Instance.ExchangeEnergyHandler += OnExchangeEnergy;
			
			
			Views.alpha = 0;
			Views.transform.localScale = Vector3.zero;
			var sequence = DOTween.Sequence();
			sequence.Append(Views.transform.DOScale(Vector3.one * 1.2f, 0.2f));
			sequence.Append(Views.transform.DOScale(Vector3.one * 1f, 0.1f));
			sequence.Insert(0, Views.DOFade(0, 1, 0.2f));
			sequence.Play();
		}
		
		protected override void OnHide()
		{
			Web3.Instance.ExchangeEnergyHandler -= OnExchangeEnergy;
		}
		
		protected override void OnClose()
		{
		}
		
		protected void Close()
		{
			Views.transform.DOScale(Vector3.zero, 0.2f);
			Views.DOFade(1, 0, 0.2f).onComplete += () =>
			{
				UIKit.HidePanel(this.GetType());
			};
		}

		public void Exchange()
		{
			int count = int.Parse(ExchangeInput.text);
			if (count > 0)
			{
				UIKit.OpenPanelAsync<WaitingPanel>(panel =>
				{
					Web3.Instance.ExchangeEnergy(mData.RoleToken, count);
				});
			}
		}

		public void OnExchangeEnergy(ExchangeEnergyOrderInfo orderInfo)
		{
			var waitingPanel = UIKit.GetPanel<WaitingPanel>();

			if (waitingPanel == null)
			{
				UIKit.OpenPanelAsync<WaitingPanel>();
			}

			{
				UIKit.ShowPanel<WaitingPanel>();
			}
			StartCoroutine(CheckOrder(orderInfo));
		}

		public IEnumerator CheckOrder(ExchangeEnergyOrderInfo orderInfo)
		{
			bool checkedOrder = false;
			var currentRoleData = _mainModel.RoleDatas.Find(data =>
			{
				return data.NftID == orderInfo.RoleToken;
			});
			var upgradedRoleData = new RoleData();
			while (true)
			{
				yield return new WaitForSeconds(3);
				yield return HttpHelper.Get(GameUrlConstValue.GetRolesData.Url(), result =>
				{
					var getRoleResponse = JsonUtil.FromJson<GetRoleResponse>(result);
					upgradedRoleData = getRoleResponse.data.user_role_list.Find(data =>
					{
						return data.NftID == orderInfo.RoleToken;
					});

					if (upgradedRoleData.Energy - currentRoleData.Energy == orderInfo.Amount * 10)
					{
						checkedOrder = true;

						var mainPanel = UIKit.GetPanel<MainPanel>();
						if (mainPanel != null)
						{
							mainPanel.RefreshSelectRoleAttributes(upgradedRoleData);
						}

						_mainModel.RoleDatas = getRoleResponse.data.user_role_list;
						_mainModel.Bag.AddAssetNumber(ConstValue.EnergyDrinkId, orderInfo.Amount * -1);
					}
				});

				if (checkedOrder)
				{
					break;
				}
			}
			

			
			UIKit.HidePanel<WaitingPanel>();
			Count.text = _mainModel.Bag.GetAssetNumber(ConstValue.EnergyDrinkId).ToString();
			ExchangeInput.text = "0";
			
		}
	}
}
