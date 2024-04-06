using System.Collections.Generic;
using QFramework;
using SquareHero.Hotfix.Generate;
using UnityEngine;

namespace SquareHero.Hotfix
{
    public static class ExcelConfig
    {
        private static bool _initialized;
        public static PropsConfigTable PropConfigTable;
        public static AssetConfigTable AssetConfigTable;
        public static SkinConfigTable SkinConfigTable;
        public static RoleTalentTable RoleTalentTable;
        public static ItemConfigTable ItemConfigTable;
        public static ShopGoodsTable ShopGoodsTable;
        public static CostItemAttributeTable CostItemAttributeTable;
        

        public static void Initialize()
        {
            if (!_initialized)
            {
                
                ResourceManager.Instance.GetAssetAsync<PropsConfigTable>("PropsConfig", propConfigTable =>
                {
                    PropConfigTable = propConfigTable;
                });
                ResourceManager.Instance.GetAssetAsync<AssetConfigTable>("AssetConfig", assetConfigTable =>
                {
                    AssetConfigTable = assetConfigTable;
                });
                ResourceManager.Instance.GetAssetAsync<SkinConfigTable>("SkinConfig", skinConfigTable =>
                {
                    SkinConfigTable = skinConfigTable;
                });
                
                ResourceManager.Instance.GetAssetAsync<RoleTalentTable>("RoleTalent", roleTalentConfig =>
                {
                    RoleTalentTable = roleTalentConfig;
                });
                ResourceManager.Instance.GetAssetAsync<ItemConfigTable>("ItemConfig", itemConfigTable =>
                {
                    ItemConfigTable = itemConfigTable;
                });
                
                ResourceManager.Instance.GetAssetAsync<ShopGoodsTable>("ShopGoods", shopGoodsTable =>
                {
                    ShopGoodsTable = shopGoodsTable;
                });
                ResourceManager.Instance.GetAssetAsync<CostItemAttributeTable>("CostItemAttribute", costItemAttributeTable =>
                {
                    CostItemAttributeTable = costItemAttributeTable;
                });
            }
        }

        public static List<AbstractProp> GetPropConfigs(int[] propIds)
        {

            if (PropConfigTable != null)
            {
                List<AbstractProp> propConfigs = new List<AbstractProp>();
                for (int i = 0; i < propIds.Length; i++)
                {
                    Prop1 prop1 = new Prop1(propIds[i]);
                    propConfigs.Add(prop1);
                    
                }

                return propConfigs;
            }
            else
            {
                LogKit.E("have not Config Prop");
            }

            return null;
        }
    }
}