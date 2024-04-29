using System.Collections.Generic;
using System.Linq;
using Com.ForbiddenByte.OSA.CustomAdapters.GridView;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using champs3.Core.Loading;
using champs3.Hotfix.Main;
using champs3.Hotfix.Model;
using champs3.Hotfix.Player;
using TMPro;
using UnityCommon.Util;
using UnityEngine.U2D;

namespace champs3.Hotfix.UI
{
	public class BackpackPanelData : UIPanelData
	{
	}
	public partial class BackpackPanel : UIPanel
	{

		protected List<PlayerAssets> _propDatas;
		protected List<PlayerAssets> _otherPropDatas;

		protected SpriteAtlas propGradeAtlas;
		protected SpriteAtlas propAtlas;
		protected SpriteAtlas characterAtlas;
		protected SpriteAtlas attributesAtlas;

		protected BindableProperty<int> Selected = new BindableProperty<int>(-1);
		protected BindableProperty<int> RoleIndex = new BindableProperty<int>(-1);
			
		protected MainModel _mainModel;

		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as BackpackPanelData ?? new BackpackPanelData();
			// please add init code here
			Views.alpha = 0;
			
			Tags.AddListener(OnSelectTag);
			
			
			UseRoleBtn.onClick.AddListener(() =>
			{
				var mainPanel = UIKit.GetPanel<MainPanel>();
				mainPanel.ChangeRole(RoleIndex.Value);
				ClosePanel();
			});
			
			BackBtn.onClick.AddListener(ClosePanel);
			
		}
		
		
		protected override void OnOpen(IUIData uiData = null)
		{
			// PropsList.OnCreate += onCreatePropViewsHolder;
			// PropsList.OnUpdate += (viewsHolder =>
			// {
			// 	OnUpdatePropItem(viewsHolder as PropViewsHolder);
			// });
			
			// PropsList.OnCreate += onCreatePropViewsHolder;
			// PropsList.OnUpdate += (viewsHolder =>
			// {
			// 	OnUpdatePropItem(viewsHolder as PropViewsHolder);
			// });

			RoleList.Adapter = new RoleGridViewAdapter();
			RoleList.Adapter.UpdateHandler += holder =>
			{
				OnUpdateRoleItem(holder as RoleCellViewsHolder);
			};
			
			
			PropsList.Adapter = new PropGridViewAdapter();
			PropsList.Adapter.UpdateHandler += holder =>
			{
				OnUpdatePropItem(holder as PropsViewsHolder);
			};
			
			OtherList.Adapter = new PropGridViewAdapter();
			OtherList.Adapter.UpdateHandler += holder =>
			{
				OnOtherPropItem(holder as PropsViewsHolder);
			};

			Selected.Register(OnPropSelect);
			RoleIndex.Register(OnSelectRole);
		}

		protected void ClosePanel()
		{
			AudioKit.PlaySound(SoundName.clickbutton.ToString());
			Views.DOFade(1, 0, 0.2f).onComplete += () =>
			{
				CloseSelf();
			};
		}

		protected void OnSelectTag(int index)
		{

			switch (index)
			{
				case 0:
					ShowRoleBag();
					break;
				case 1:
					ShowPropsBag();
					break;
				case 2:
					ShowOtherBag();
					break;
			}

			for (int i = 0; i < Tags.transform.childCount; i++)
			{
				Tags.transform.GetChild(i).Find("Select").gameObject.SetActive(i == index);
			}
		}


		protected void ShowRoleBag()
		{
			RoleBackpack.gameObject.SetActive(true);
			
			PropsBackpack.gameObject.SetActive(false);
			OtherBackpack.gameObject.SetActive(false);
			
			CurrentRoleInfo.gameObject.SetActive(_mainModel.RoleDatas.Count > 0);
			CurrentPropsInfo.gameObject.SetActive(false);
			CurrentOhterInfo.gameObject.SetActive(false);
		}
		
		protected void ShowPropsBag()
		{
			RoleBackpack.gameObject.SetActive(false);
			OtherBackpack.gameObject.SetActive(false);
			PropsBackpack.gameObject.SetActive(true);

			CurrentRoleInfo.gameObject.SetActive(false);
			CurrentPropsInfo.gameObject.SetActive(_mainModel.Bag.PlayerGameProps.Count > 0);
			CurrentOhterInfo.gameObject.SetActive(false);
		}

		protected void ShowOtherBag()
		{
			OtherBackpack.gameObject.SetActive(true);
			RoleBackpack.gameObject.SetActive(false);
			PropsBackpack.gameObject.SetActive(false);
			
			CurrentRoleInfo.gameObject.SetActive(false);
			CurrentPropsInfo.gameObject.SetActive(false);
			CurrentOhterInfo.gameObject.SetActive(_otherPropDatas.Count > 0);
			if (_otherPropDatas.Count > 0)
			{
				OnSelectOtherProps(0);
			}
		}

		protected void OnUpdateRoleItem(RoleCellViewsHolder viewsHolder)
		{
			var index = viewsHolder.ItemIndex;
			var roleData = _mainModel.RoleDatas[index];
			var skinConfig = ExcelConfig.SkinConfigTable.Data.Find(config =>
			{
				return config.Name == roleData.Character;
			});

			
			var roleCard = characterAtlas.GetSprite($"card_frame_{roleData.Character}");
			
			viewsHolder.RoleItem.Icon.sprite = roleCard;

			viewsHolder.RoleItem.Energy.text = roleData.Energy.ToString();


			viewsHolder.RoleItem.Views.onClick.RemoveAllListeners();
			
			viewsHolder.RoleItem.Views.onClick.AddListener(() =>
			{
				RoleIndex.Value = index;
			});
		}

		private void OnSelectRole(int index)
		{

			for (int i = 0; i < RoleList.GetItemsCount(); i++)
			{
				var shCellViewsHolder = RoleList.GetCellViewsHolderIfVisible(i);
				if (shCellViewsHolder != null)
				{
					var roleCellViewsHolder = shCellViewsHolder as RoleCellViewsHolder;
					
					roleCellViewsHolder.RoleItem.Selected.gameObject.SetActive(i == index);
				}
				
			}
			
			var roleData = _mainModel.RoleDatas[index];
			var skinConfig = ExcelConfig.SkinConfigTable.Data.Find(config =>
			{
				return config.Name == roleData.Character;
			});

			string character = roleData.Character;

			if (RoleCard.texture == null || RoleCard.texture.name != character)
			{
				ResourceManager.Instance.GetAssetAsync<Texture>($"{ConstValue.AssetGroup.Texture}_{character}",
					texture =>
					{
						RoleCard.texture = texture;
						CurrentRoleInfo.gameObject.SetActive(this);
					});
			}

			string grade = $"RoleCard_{skinConfig.Grade}";

			if (RoleCardBG.texture == null || RoleCardBG.texture.name != grade)
			{
				ResourceManager.Instance.GetAssetAsync<Texture>($"{ConstValue.AssetGroup.Texture}_{grade}",
					texture =>
					{
						RoleCardBG.texture = texture;
						CurrentRoleInfo.gameObject.SetActive(this);
					});
			}

			for (int i = 0; i < Stars.childCount; i++)
			{
				Stars.GetChild(i).GetChild(0).gameObject.SetActive(i < skinConfig.Grade);
			}
			
			CurrentRoleEnergy.text = roleData.Energy.ToString();

			LevelNum.text = $"LV.{roleData.Level}";

			RunSpeed.text = roleData.GetRunAttributeValue().ToString();
			SwimSpeed.text = roleData.GetSwimAttributeValue().ToString();
			ClimbSpeed.text = roleData.GetClimbAttributeValue().ToString();
			FlySpeed.text = roleData.GetFlyAttributeValue().ToString();
			
			var roleTalent = ExcelConfig.RoleTalentTable.Data.Find(config =>
			{
				return config.Id == roleData.TalentId;
			});
			
			
			RunIcon.sprite = characterAtlas.GetSprite($"1_1");
			SwimIcon.sprite = characterAtlas.GetSprite($"2_1");
			ClimbIcon.sprite = characterAtlas.GetSprite($"3_1");
			FlyIcon.sprite = characterAtlas.GetSprite($"4_1");
			if (roleTalent != null)
			{
				var sprite = characterAtlas.GetSprite($"{roleTalent.AttriType}_{roleTalent.Grade}");
				switch (roleTalent.AttriType)
				{
					case 1:
						RunIcon.sprite = sprite;
						break;
					case 2:
						SwimIcon.sprite = sprite;
						break;
					case 3:
						ClimbIcon.sprite = sprite;
						break;
					case 4:
						FlyIcon.sprite = sprite;
						break;
				}
			}
		}
		
		private void GetAttributesValue(List<RoleAttribute> attributes, AttributesType attributesType, TextMeshProUGUI label)
		{
			string value = (attributes[(int)attributesType].AttriValue +
			                attributes[(int)attributesType].TalentValue).ToString();

			label.text = value;
		}

		protected void OnUpdatePropItem(PropsViewsHolder viewsHolder)
		{
			var index = viewsHolder.ItemIndex;
			
			var propData = _propDatas[index];

			var propItem = viewsHolder.PropItem;

			var propConfig = ExcelConfig.PropConfigTable.Data.Find(asset =>
			{
				return asset.Id == propData.Id;
			});

			if (propConfig != null)
			{
				var assetConfig = ExcelConfig.AssetConfigTable.Data.Find(asset =>
				{
					return asset.Id == propConfig.AssetId;
				});

				propItem.Bg.sprite = propGradeAtlas.GetSprite($"Prop_Grade_{propData.ItemConfig.Grade}");
				propItem.Icon.sprite = propAtlas.GetSprite(assetConfig.BigIcon);
				propItem.Count.text = propData.Number.ToString();
				propItem.Views.onClick.RemoveAllListeners();
				propItem.Views.onClick.AddListener(() =>
				{
					AudioKit.PlaySound(SoundName.clickbutton.ToString());
					Selected.Value = viewsHolder.ItemIndex;
				});
			}
			else
			{
				LogKit.E($"Not Found PropConfig [{propData.Id}]");
			}
		}

		public void OnPropSelect(int index)
		{
			
			for (int i = 0; i < PropsList.GetItemsCount(); i++)
			{
				var child = PropsList.GetCellViewsHolder(i) as PropsViewsHolder;
				child.PropItem.Selected.gameObject.SetActive(i == index);
						
			}
			
			var propData = _propDatas[index];
			
			var propConfig = ExcelConfig.PropConfigTable.Data.Find(asset =>
			{
				return asset.Id == propData.Id;
			});

			if (propConfig != null)
			{
				var assetConfig = ExcelConfig.AssetConfigTable.Data.Find(asset =>
				{
					return asset.Id == propConfig.AssetId;
				});

				PropsName.text = propConfig.Name;
				PropsIcon.sprite = propAtlas.GetSprite(assetConfig.BigIcon);
				SpeedIncrease.text = propConfig.SpeedIncrease + " %";
				TimeIncrease.text = propConfig.Distance + " s";
				if (propConfig.Distance == -1)
				{
					TimeIncrease.text = "<size=30>∞</size>";
				}
				else
				{
					TimeIncrease.text = propConfig.Distance.ToString();
				}
				if (propConfig.UsageTimes == -1)
				{
					UsesIncrease.text = "<size=30>∞</size>";
				}
				else
				{
					UsesIncrease.text = propConfig.UsageTimes.ToString();
				}
				PropsDesc.text = propData.ItemConfig.Explain;
				PropsGrade.sprite = propGradeAtlas.GetSprite($"Prop_Small_Grade_{propData.ItemConfig.Grade}");
				PropsGradeLabel.text = ((PropsGrade)propData.ItemConfig.Grade).ToString();
			}
		}

		protected void OnOtherPropItem(PropsViewsHolder viewsHolder)
		{
			var index = viewsHolder.ItemIndex;

			
			var propData = _otherPropDatas[index];

			var propItem = viewsHolder.PropItem;
			
			propItem.Bg.gameObject.SetActive(false);
			var sprite = propAtlas.GetSprite(propData.ItemConfig.IconBig);
			propItem.Icon.sprite = sprite;
			propItem.Icon.rectTransform.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
			propItem.Count.text = propData.Number.ToString();
			propItem.Views.onClick.RemoveAllListeners();
			propItem.Views.onClick.AddListener(() =>
			{
				AudioKit.PlaySound(SoundName.clickbutton.ToString());
				OnSelectOtherProps(index);
			});
			
		}

		protected void OnSelectOtherProps(int index)
		{
			var propData = _otherPropDatas[index];

			OtherName.text = propData.ItemConfig.Name;
			var othersIconSprite = propAtlas.GetSprite(propData.ItemConfig.IconBig);
			OthersIcon.sprite = othersIconSprite;
			OthersIcon.rectTransform.sizeDelta = new Vector2(othersIconSprite.rect.width, othersIconSprite.rect.height);
			OhterDesc.text = propData.ItemConfig.Explain;
			SynthesisBtn.gameObject.SetActive(propData.Id != 10010);
			
		}

		protected override void OnShow()
		{
			Load();
		}

		
		
		protected void GetProps()
		{

		}


		protected void Load()
		{
			Loading loading = new Loading();

			
			LoadAssetNode<SpriteAtlas> propAtlasLoader = new LoadAssetNode<SpriteAtlas>(ConstValue.AtlasLocation.Props);
			LoadAssetNode<SpriteAtlas> propGradeAtlasLoader = new LoadAssetNode<SpriteAtlas>(ConstValue.AtlasLocation.PropsGrade);
			LoadAssetNode<SpriteAtlas> characterAtlasLoader = new LoadAssetNode<SpriteAtlas>(ConstValue.AtlasLocation.Character);
			LoadAssetNode<SpriteAtlas> attributesAtlasLoader = new LoadAssetNode<SpriteAtlas>(ConstValue.AtlasLocation.Attributes);
			loading.AddNode(propAtlasLoader);
			loading.AddNode(propGradeAtlasLoader);
			loading.AddNode(characterAtlasLoader);
			loading.AddNode(attributesAtlasLoader);

			loading.OnCompleted += () =>
			{
				propAtlas = propAtlasLoader.GetAsset();
				propGradeAtlas = propGradeAtlasLoader.GetAsset();
				characterAtlas = characterAtlasLoader.GetAsset();
				attributesAtlas = attributesAtlasLoader.GetAsset();
				FillData();
			};
			loading.Start();
		}

		protected void FillData()
		{
			
			_mainModel = MainController.Instance.GetArchitecture().GetModel<MainModel>();
			var playerAssetsArray = _mainModel.Bag.PlayerGameProps.Values.ToArray();
			_propDatas = new List<PlayerAssets>();
			for (int i = 0; i < playerAssetsArray.Length; i++)
			{
				if (playerAssetsArray[i].Number > 0)
				{
					_propDatas.Add(playerAssetsArray[i]);
				}
			}

			var assetsArray = _mainModel.Bag.PlayerOtherProps.Values.ToArray();
			_otherPropDatas = new List<PlayerAssets>();
			for (int i = 0; i < assetsArray.Length; i++)
			{
				if (assetsArray[i].Number > 0)
				{
					_otherPropDatas.Add(assetsArray[i]);
				}
			}


			if (_mainModel.RoleDatas.Count > 0)
			{
				RoleList.ResetItems(_mainModel.RoleDatas.Count);
			
				RoleIndex.Value = 0;
			}
			else
			{
				CurrentRoleInfo.gameObject.SetActive(false);
			}
			
			
			PropsList.ResetItems(_propDatas.Count);
			
			OtherList.ResetItems(_otherPropDatas.Count);

			if (_propDatas.Count > 0)
			{
				
				Selected.Value = 0;
			}
			
			OtherBackpack.gameObject.SetActive(false);
			PropsBackpack.gameObject.SetActive(false);
			
			Views.DOFade(0, 1, 0.4f);
		}

		protected override void OnHide()
		{
			Selected.UnRegister(OnPropSelect);
		}
		
		protected override void OnClose()
		{
		}
	}
	
	public class PropsViewsHolder : SHCellViewsHolder
	{
		public PropListItem PropItem;
		
		public override void CollectViews()
		{
			base.CollectViews();
			PropItem = root.GetComponent<PropListItem>();
		}
	}
	
	public class PropCellGroupViewsHolder : SHCellGroupViewsHolder
	{
		protected override SHCellViewsHolder CreateCellViewsHolder()
		{
			return new PropsViewsHolder();
		}
	}

	public class PropGridViewAdapter : SHScrollGridViewAdapter
	{
		public override CellGroupViewsHolder<SHCellViewsHolder> GetNewCellGroupViewsHolder()
		{
			return new PropCellGroupViewsHolder();
		}
	}
	
	
	public class RoleCellViewsHolder : SHCellViewsHolder
	{
		public RoleBackpackItem RoleItem;
		
		public override void CollectViews()
		{
			base.CollectViews();
			RoleItem = root.GetComponent<RoleBackpackItem>();
		}
	}
	
	public class RoleCellGroupViewsHolder : SHCellGroupViewsHolder
	{
		protected override SHCellViewsHolder CreateCellViewsHolder()
		{
			return new RoleCellViewsHolder();
		}
	}
	
	public class RoleGridViewAdapter : SHScrollGridViewAdapter
	{
		public override CellGroupViewsHolder<SHCellViewsHolder> GetNewCellGroupViewsHolder()
		{
			return new RoleCellGroupViewsHolder();
		}
	}
}
