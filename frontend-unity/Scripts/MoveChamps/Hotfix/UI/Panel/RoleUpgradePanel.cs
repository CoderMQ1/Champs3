using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using SquareHero.Core.Loading;
using SquareHero.Hotfix.Events;
using SquareHero.Hotfix.Generate;
using SquareHero.Hotfix.Main;
using SquareHero.Hotfix.Model;
using SquareHero.Hotfix.Toast;
using UnityCommon.Util;

namespace SquareHero.Hotfix.UI
{
	public class RoleUpgradePanelData : UIPanelData
	{
		public long RoleToken;
	}
	public partial class RoleUpgradePanel : UIPanel
	{
		private MainModel _mainModel;
		private bool upgraded;
		private int _price;

		private RoleUpgradeConfigTable _config;

		protected RoleData _currentRoleData;
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as RoleUpgradePanelData ?? new RoleUpgradePanelData();
			// please add init code here
			Views.alpha = 0;
			UpgradeBtn.onClick.AddListener(UpgradeRole);
			BackBtn.onClick.AddListener(() =>
			{
				Close();
			});
			
			Views.GetComponent<Button>().onClick.AddListener(() =>
			{
				Close();
			});


		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
			mData = uiData as RoleUpgradePanelData ?? new RoleUpgradePanelData();
		}
		
		protected override void OnShow()
		{
			
			// LogKit.I($"RoleUpgradePanel onshow {mData.RoleData.Character}");
			upgraded = false;
			UpgradeTip.gameObject.SetActive(!upgraded);
			AfterUpgradedTip.gameObject.SetActive(upgraded);
			RewardInfo.gameObject.SetActive(!upgraded);
			NextLevelInfo.gameObject.SetActive(upgraded);
			UpgradeBtn.gameObject.SetActive(!upgraded);
			Load();
			CoinBox.Refresh();
			Web3.Instance.UpgradeRoleHandler += OnUpgradeRole;
		}


		protected void Load()
		{
			Loading loading = new Loading();
			
			_mainModel = MainController.Instance.GetArchitecture().GetModel<MainModel>();
			
			var currentRoleData = _mainModel.RoleDatas.Find(data =>
			{
				return data.NftID == mData.RoleToken;
			});
			var skinConfig = ExcelConfig.SkinConfigTable.Data.Find(config =>
			{
				return config.Name == currentRoleData.Character;
			});

			var assetConfig = ExcelConfig.AssetConfigTable.Data.Find(config =>
			{
				return config.Id == skinConfig.AssetId;
			});
		
			LoadAssetNode<Texture> roleTexLoadNode = new LoadAssetNode<Texture>($"{ConstValue.AssetGroup.Texture}_{assetConfig.BigIcon}_b");

			LoadAssetNode<RoleUpgradeConfigTable> configLoadNode =
				new LoadAssetNode<RoleUpgradeConfigTable>($"RoleUpgradeConfig");
			
			loading.AddNode(configLoadNode);

			loading.AddNode(roleTexLoadNode);

			loading.OnCompleted += () =>
			{
				// RoleIcon.texture = roleTexLoadNode.GetAsset();
				_config = configLoadNode.GetAsset();
				FillData();
			};
			
			loading.Start();
		}



		protected void FillData()
		{
			Views.alpha = 0;
			Views.transform.localScale = Vector3.zero;
			var sequence = DOTween.Sequence();
			sequence.Append(Views.transform.DOScale(Vector3.one * 1.2f, 0.2f));
			sequence.Append(Views.transform.DOScale(Vector3.one * 1f, 0.1f));
			sequence.Insert(0, Views.DOFade(0, 1, 0.2f));
			sequence.Play();
			

			var currentRoleData = _mainModel.RoleDatas.Find(data =>
			{
				return data.NftID == mData.RoleToken;
			});
			
			var roleUpgradeConfig = _config.Data.Find(config =>
			{
				return config.CurrentLevel == currentRoleData.Level;
			});
			
			CurrentLevel.text =currentRoleData.Level.ToString();
			RunSpeed.text = currentRoleData.GetAttributeValue(AttributesType.RunSpeed).ToString();
			SwimmingSpeed.text =currentRoleData.GetAttributeValue(AttributesType.SwimSpeed).ToString();
			ClimbingSpeed.text = currentRoleData.GetAttributeValue(AttributesType.ClimbSpeed).ToString();
			FlyingSpeed.text = currentRoleData.GetAttributeValue(AttributesType.FlySpeed).ToString();
			RewardCount.text = "X" + roleUpgradeConfig.UpgradeAttribute.ToString();
			UpgradePrice.text = roleUpgradeConfig.NeedMoney + "X";
		}
		
		

		protected override void OnHide()
		{
			Web3.Instance.UpgradeRoleHandler -= OnUpgradeRole;
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
		
		protected void OnUpgradeRole(UpgradeRoleOrderInfo orderInfo)
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

		protected IEnumerator CheckOrder(UpgradeRoleOrderInfo orderInfo)
		{
			bool checkedOrder = false;
			var upgradedRoleData = new RoleData();
			while (true)
			{
				yield return new WaitForSeconds(3);
				yield return HttpHelper.Get(GameUrlConstValue.GetRolesData.Url(), result =>
				{
					var getRoleResponse = JsonUtil.FromJson<GetRoleResponse>(result);
					upgradedRoleData = getRoleResponse.data.user_role_list.Find(data =>
					{
						return data.NftID == _currentRoleData.NftID;
					});
					
					if (upgradedRoleData.Level == orderInfo.Level)
					{
						_mainModel.RoleDatas = getRoleResponse.data.user_role_list;
						checkedOrder = true;
					}
				});

				if (checkedOrder)
				{
					break;
				}
			}

			if (checkedOrder)
			{
				upgraded = true;
				UpgradeTip.gameObject.SetActive(!upgraded);
				AfterUpgradedTip.gameObject.SetActive(upgraded);
				RewardInfo.gameObject.SetActive(!upgraded);
				NextLevelInfo.gameObject.SetActive(upgraded);
				// UpgradeBtn.gameObject.SetActive(!upgraded);
			
				var roleUpgradeConfig = _config.Data.Find(config =>
				{
					return config.CurrentLevel == upgradedRoleData.Level;
				});
				UpgradePrice.text = roleUpgradeConfig.NeedMoney + "X";
			
				NextLevel.text = upgradedRoleData.Level.ToString();
				var runSpeed = upgradedRoleData.GetAttributeValue(AttributesType.RunSpeed);
				var swimSpeed = upgradedRoleData.GetAttributeValue(AttributesType.SwimSpeed);
				var climbSpeed = upgradedRoleData.GetAttributeValue(AttributesType.ClimbSpeed);
				var flySpeed = upgradedRoleData.GetAttributeValue(AttributesType.FlySpeed);
				NextRunSpeed.text = runSpeed.ToString();
				NextSwimmingSpeed.text = swimSpeed.ToString();
				NextClimbingSpeed.text = climbSpeed.ToString();
				NextFlyingSpeed.text = flySpeed.ToString();
				
				var changedColor = new Color(1, 0.7529412f, 0, 1);
				if (runSpeed > _currentRoleData.GetAttributeValue(AttributesType.RunSpeed))
				{
					NextRunSpeed.color = changedColor;
				}
				else
				{
					NextRunSpeed.color = Color.white;
				}
				
				if (swimSpeed > _currentRoleData.GetAttributeValue(AttributesType.SwimSpeed))
				{
					NextSwimmingSpeed.color = changedColor;
				}
				else
				{
					NextSwimmingSpeed.color = Color.white;
				}
				
				if (climbSpeed > _currentRoleData.GetAttributeValue(AttributesType.ClimbSpeed))
				{
					NextClimbingSpeed.color = changedColor;
				}
				else
				{
					NextClimbingSpeed.color = Color.white;
				}
				
				if (flySpeed > _currentRoleData.GetAttributeValue(AttributesType.FlySpeed))
				{
					NextFlyingSpeed.color = changedColor;
				}
				else
				{
					NextFlyingSpeed.color = Color.white;
				}

				
				var mainPanel = UIKit.GetPanel<MainPanel>();
				mainPanel.RefreshSelectRoleAttributes(upgradedRoleData);
				_currentRoleData = upgradedRoleData;
				ToastManager.Instance.ToastMsg("Upgrade Success");
				// Web3.Instance.GetBalance();
				_mainModel.Bag.AddAssetNumber(ConstValue.TokenId, -orderInfo.Price.ParseToken());
			}
			else
			{
				ToastManager.Instance.ToastMsg("Network Error");
				Close();
			}
			UIKit.HidePanel<WaitingPanel>();
		}
		
		protected void UpgradeRole()
		{
			
			var currentRoleData = _mainModel.RoleDatas.Find(data =>
			{
				return data.NftID == mData.RoleToken;
			});
			
			var roleUpgradeConfig = _config.Data.Find(config =>
			{
				return config.CurrentLevel == currentRoleData.Level;
			});
		
			
			upgraded = false;
			UpgradeTip.gameObject.SetActive(!upgraded);
			AfterUpgradedTip.gameObject.SetActive(upgraded);
			RewardInfo.gameObject.SetActive(!upgraded);
			NextLevelInfo.gameObject.SetActive(upgraded);
			UpgradeBtn.gameObject.SetActive(!upgraded);
			RewardCount.text = "X" + roleUpgradeConfig.UpgradeAttribute.ToString();
			
			if (_mainModel.Bag.GetAssetNumber(ConstValue.TokenId) > roleUpgradeConfig.NeedMoney)
			{
				UIKit.OpenPanelAsync<WaitingPanel>(panel =>
				{
					upgraded = false;
					CurrentLevel.text = currentRoleData.Level.ToString();
					RunSpeed.text = currentRoleData.GetAttributeValue(AttributesType.RunSpeed).ToString();
					SwimmingSpeed.text = currentRoleData.GetAttributeValue(AttributesType.SwimSpeed).ToString();
					ClimbingSpeed.text = currentRoleData.GetAttributeValue(AttributesType.ClimbSpeed).ToString();
					FlyingSpeed.text = currentRoleData.GetAttributeValue(AttributesType.FlySpeed).ToString();

					_currentRoleData = currentRoleData;

					UpgradePrice.text = roleUpgradeConfig.NeedMoney + "X";
					Web3.Instance.UpgradeRole(new UpgradeRoleOrderInfo(){RoleToken = currentRoleData.NftID, Price = roleUpgradeConfig.NeedMoney, Level = currentRoleData.Level + 1});
				});
			}
			else
			{
				ToastManager.Instance.ToastMsg(ConstValue.GameText.TokenNotEnoughTip);
			}

		}

		// protected void UpgradeRole()
		// {
		// 	LoadingMask.gameObject.SetActive(true);
		// 	upgraded = false;
		// 	HttpGetNode getSkinLode = new HttpGetNode(this, GameUrlConstValue.GetRolesData.Url());
		// 	_mainModel = MainController.Instance.GetArchitecture().GetModel<MainModel>();
		// 	getSkinLode.CompletHandler += () =>
		// 	{
		// 		var result = getSkinLode.GetResult();
		// 		var getRoleResponse = JsonUtil.FromJson<GetRoleResponse>(result);
		// 		_mainModel.RoleDatas = getRoleResponse.data.user_role_list;
		// 		SendEvent(UIEvents.ID.RefreshSelectRoleAttributes);
		// 	};
		// 	
		// 	
		// 	var currentRoleData = _mainModel.RoleDatas.Find(data =>
		// 	{
		// 		return data.NftID == mData.RoleData.NftID;
		// 	});
		// 		
		// 	CurrentLevel.text = currentRoleData.Level.ToString();
		// 	RunSpeed.text = currentRoleData.GetRunAttributeValue().ToString();
		// 	SwimmingSpeed.text = currentRoleData.GetSwimAttributeValue().ToString();
		// 	ClimbingSpeed.text = currentRoleData.GetClimbAttributeValue().ToString();
		// 	FlyingSpeed.text = currentRoleData.GetFlyAttributeValue().ToString();
		//
		// 	
		// 	HttpGetNode getAssetLode = new GetPlayerAssetNode(this);
		//
		// 	getAssetLode.CompletHandler += () =>
		// 	{
		// 		var result = getAssetLode.GetResult();
		//
		// 		var playAssetResponse = JsonUtil.FromJson<PlayerassetsResponse>(result);
		// 		SendMsg(new UIEvents.RefreshPlayerAssets()
		// 		{
		// 			Response = playAssetResponse
		// 		});
		// 	};
		//
		// 	HttpJsonPostNode postNode = new HttpJsonPostNode(this, GameUrlConstValue.UpgradeRole.Url(),
		// 		$"{{\"nft_id\": {mData.RoleData.NftID}}}");
		// 	
		// 	Loading loading = new Loading(1);
  //           
		// 	loading.AddNode(postNode);
		// 	loading.AddNode(getSkinLode);
		// 	loading.AddNode(getAssetLode);
		// 	loading.OnCompleted += () =>
		// 	{			
		//
		// 		var upgradedRoleData = _mainModel.RoleDatas.Find(data =>
		// 		{
		// 			return data.NftID == mData.RoleData.NftID;
		// 		});
		//
		//
		// 		// RefreshSelectRoleAttributes(upgradedRoleData);
		// 		
		// 		upgraded = true;
		// 		UpgradeTip.gameObject.SetActive(!upgraded);
		// 		AfterUpgradedTip.gameObject.SetActive(upgraded);
		// 		RewardInfo.gameObject.SetActive(!upgraded);
		// 		NextLevelInfo.gameObject.SetActive(upgraded);
		// 		// UpgradeBtn.gameObject.SetActive(!upgraded);
		// 		
		// 		var roleUpgradeConfig = _config.Data.Find(config =>
		// 		{
		// 			return config.CurrentLevel == upgradedRoleData.Level;
		// 		});
		// 		UpgradePrice.text = roleUpgradeConfig.NeedMoney + "X";
		// 		
		// 		NextLevel.text = upgradedRoleData.Level.ToString();
		// 		NextRunSpeed.text = upgradedRoleData.GetRunAttributeValue().ToString();
		// 		NextSwimmingSpeed.text = upgradedRoleData.GetSwimAttributeValue().ToString();
		// 		NextClimbingSpeed.text = upgradedRoleData.GetClimbAttributeValue().ToString();
		// 		NextFlyingSpeed.text = upgradedRoleData.GetFlyAttributeValue().ToString();
		// 		
		// 		LogKit.I($"Role [{upgradedRoleData.Character}]:[{upgradedRoleData.NftID}] upgrade to : {upgradedRoleData.Level}");
		// 		LoadingMask.gameObject.SetActive(false);
		// 		
		// 		
		// 		CoinBox.Refresh();
		// 	};
		// 	loading.Start();
		// }
	}
}
