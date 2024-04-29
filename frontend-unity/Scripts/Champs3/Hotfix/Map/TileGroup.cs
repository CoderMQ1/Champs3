// 
// 2023/11/29

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace champs3.Hotfix.Map
{
    [CreateAssetMenu(fileName = "Tile", menuName = "champs3/Map/TileGroup", order = 0)]
    public class TileGroup : SerializedScriptableObject
    {
        public Dictionary<TileType, Dictionary<TilePosition, int>> data;
    }
}