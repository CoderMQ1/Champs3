// 
// 2023/11/29

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SquareHero.Hotfix.Map
{
    [CreateAssetMenu(fileName = "Tile", menuName = "SquareHero/Map/TileGroup", order = 0)]
    public class TileGroup : SerializedScriptableObject
    {
        public Dictionary<TileType, Dictionary<TilePosition, int>> data;
    }
}