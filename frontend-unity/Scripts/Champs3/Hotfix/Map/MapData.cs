// 
// 2024/01/17

using System.Collections.Generic;
using champs3.Hotfix.Model;
using UnityEngine;

namespace champs3.Hotfix.Map
{
    [CreateAssetMenu(fileName = "Map", menuName = "champs3/Map/Map", order = 1)]
    public class MapData : ScriptableObject
    {
        public int MapId;
        
        public List<TileData> TileDatas;
        
        [HideInInspector]
        public List<List<GiftModel>> GiftModels;
    }
}