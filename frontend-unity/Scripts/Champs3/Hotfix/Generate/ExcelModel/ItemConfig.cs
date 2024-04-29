//2024/2/27 19:42:28
using UnityEngine;
namespace champs3.Hotfix.Generate
{
	public class ItemConfig : ScriptableObject
	{
		//道具Id
		public int Id;
		//稀有度(1普通，2罕见，3稀有，4史诗，5传说)
		public int Grade;
		//道具类型(1通常道具，2积分类，3皮肤，4掉落组，5皮肤部位，6消耗道具，10代币类)
		public int Type;
		//类型名称
		public string TypeName;
		//道具名称
		public string Name;
		//图标资源（大图标）
		public string IconBig;
		//图标资源（小图标）
		public string IconSmall;
		//平台资产名称
		public string AssetName;
		//物品说明
		public string Explain;
		//皮肤来源（仅对皮肤类有效,1为商城，2为活动，3为NFT）
		public int[] SkinScoure;
	}
}
