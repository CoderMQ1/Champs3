// 
// 2023/11/30

using UnityCommon;
using UnityEngine;

namespace champs3.Hotfix
{
    [CreateAssetMenu(fileName = "SceneMap", menuName = "champs3/SceneMap/SceneMap", order = 1)]
    public class SceneMap : ScriptableObject
    {
        [Scene]
        public string fp;
    }
}