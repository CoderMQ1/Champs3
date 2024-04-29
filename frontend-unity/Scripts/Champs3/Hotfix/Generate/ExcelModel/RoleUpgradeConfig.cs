//2024/2/4 21:33:32
using UnityEngine;
namespace champs3.Hotfix.Generate
{
	public class RoleUpgradeConfig : ScriptableObject
	{
		//Id
		public int Id;
		//当前等级
		public int CurrentLevel;
		//升级后等级
		public int UpgradeLevel;
		//升级需要代币
		public int NeedMoney;
		//升级获得属性点
		public int UpgradeAttribute;
		//升级消耗物品ID，关联物品表ID
		public int NeedItemId;
	}
}
