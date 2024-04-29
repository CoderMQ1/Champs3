//2024/2/27 14:54:04
using UnityEngine;
namespace champs3.Hotfix.Generate
{
	public class LuckSpinRewardSetting : ScriptableObject
	{
		//ID
		public int Id;
		//轮盘排序
		public int BlockSort;
		//需要次数
		public int MustTimes;
		//权重
		public int Weight;
		//消耗奖池数量
		public int CostPoolNum;
		//奖励特效等级
		public int RewardEffect;
		//奖励类型（res为道具，Pool为奖池）
		public string RewardType;
		//奖励ID（Type为Pool时，2为子代币转盘奖池）
		public string RewardID;
		//奖励数量（Type为Pool时，配置为奖池百分比，换成奖池千分比）
		public int RewardNum;
	}
}
