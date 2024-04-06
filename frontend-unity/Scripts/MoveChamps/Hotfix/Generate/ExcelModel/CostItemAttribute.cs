//2024/2/21 11:19:17
using UnityEngine;
namespace SquareHero.Hotfix.Generate
{
	public class CostItemAttribute : ScriptableObject
	{
		//道具Id
		public int Id;
		//使用次数（-1为无限）
		public string UsageTimes;
		//适用地形（1为平地，2为泳道，3为悬崖，4为天空）
		public int[] Terrain;
		//加速距离，单位和TrackModule里的Lenght一样，-1为无限
		public int Distance;
		//加速属性类型
		public int[] SpeedType;
		//道具加速百分比
		public int SpeedIncrease;
	}
}
