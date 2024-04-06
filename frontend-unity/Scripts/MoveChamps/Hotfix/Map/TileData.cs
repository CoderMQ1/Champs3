// 
// 2023/11/29

using System;

namespace SquareHero.Hotfix.Map
{
    [Serializable]
    public class TileData
    {
        public int id;

        public TileType tileType;

        public TilePosition tilePosition;
    }
}