//2023/12/11 15:10:49
using UnityEngine;
namespace SquareHero.Hotfix.Generate
{
	public class AssetConfig : ScriptableObject
	{
		//资源Id
		public int Id;
		//名字
		public string Name;
		//预制体名字
		public string PrefabName;
		//大图标
		public string BigIcon;
		//小图标
		public string SmallIcon;
		//资源类型(1,皮肤，2挂件)
		public int AssetType;
		//挂件位置
		public string SuspendTransform;
	}
}
