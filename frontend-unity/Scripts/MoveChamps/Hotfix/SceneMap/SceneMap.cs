// 
// 2023/11/30

using UnityCommon;
using UnityEngine;

namespace SquareHero.Hotfix
{
    [CreateAssetMenu(fileName = "SceneMap", menuName = "SquareHero/SceneMap/SceneMap", order = 1)]
    public class SceneMap : ScriptableObject
    {
        [Scene]
        public string fp;
    }
}