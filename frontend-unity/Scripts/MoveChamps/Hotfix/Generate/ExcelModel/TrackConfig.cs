//2024/2/21 20:17:39
using UnityEngine;
namespace SquareHero.Hotfix.Generate
{
	public class TrackConfig : ScriptableObject
	{
		//ID
		public int Id;
		//跑道配置，关联TrackModule
		public int[] TrackModule;
		//TrackModule地块数量
		public int[] TrackModuleNum;
	}
}
