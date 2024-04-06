//2024/3/21 10:49:11
using UnityEngine;
namespace SquareHero.Hotfix.Generate
{
	public class ShopGoods : ScriptableObject
	{
		//ID
		public int Id;
		//商品ID
		public string GoodsId;
		//所属商店类型
		public int Group;
		//子商店类型（不填没有）
		public int SubShopType;
		//是否显示
		public int IsShow;
		//限购数量（-1为不限量）
		public int LimitNum;
		//商品类型
		public string GoodsType;
		//ID
		public int GoodsArg;
		//商品数量
		public int GoodsArg2;
		//售卖类型（res为资源，RMB为美刀）
		public string ConsumeType;
		//类型参数
		public int ConsumeArg;
		//数量参数
		public float ConsumeArg2;
	}
}
