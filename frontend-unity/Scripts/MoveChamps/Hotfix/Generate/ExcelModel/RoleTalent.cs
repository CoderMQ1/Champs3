//2024/1/10 16:14:53
using UnityEngine;
namespace SquareHero.Hotfix.Generate
{
	public class RoleTalent : ScriptableObject
	{
		//ID
		public int Id;
		//说明
		public string Explain;
		//稀有度（2为罕见，3为稀有，4为史诗，5为传说）
		public int Grade;
		//属性增加类型（1为跑步速度，2为游泳速度，3为攀爬速度，4为飞行速度）
		public int AttriType;
		//基础速度属性加成
		public int BaseSpeedAdd;
		//百分比速度属性加成（实际数值要/100）
		public int PercentageSpeedAdd;
		//说明
		public string Name;
	}
}
