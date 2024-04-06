// 
// 2024/01/17

using System.Collections.Generic;
using SquareHero.Hotfix.Model;
using UnityEngine;

namespace SquareHero.Hotfix.Map
{
    [CreateAssetMenu(fileName = "Map", menuName = "SquareHero/Map/Map", order = 1)]
    public class MapData : ScriptableObject
    {
        public int MapId;
        
        public List<TileData> TileDatas;
        
        [HideInInspector]
        public List<List<GiftModel>> GiftModels;
    }
}