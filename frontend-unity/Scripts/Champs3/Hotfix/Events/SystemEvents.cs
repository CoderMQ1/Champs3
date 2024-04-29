// 
// 2023/12/28

using System.Collections.Generic;

namespace champs3.Hotfix
{
    public class SystemEvents
    {
        public struct UpdatePlayerProps
        {
            public List<PropsData> PropDatas;
        }
        
        public struct UpdatePlayerAssets
        {
            public PlayerassetsResponse Response;
        }
    }
}