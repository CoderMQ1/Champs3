//2024/2/27 19:27:42
using UnityEngine;
namespace SquareHero.Hotfix.Generate
{
	public class PropsConfig : ScriptableObject
	{
		//道具Id
		public int Id;
		//名字
		public string Name;
		//稀有度
		public int Grade;
		//使用次数
		public int UsageTimes;
		//地形
		public int[] Terrain;
		//加速距离，单位和TrackModule里的Lenght一样，-1为无限
		public int Distance;
		//加速属性
		public int[] SpeedType;
		//加速百分比
		public int SpeedIncrease;
		//资源Id
		public int AssetId;
		//描述
		public string Desc;
	}
}
