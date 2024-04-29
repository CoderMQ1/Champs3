using System.Collections.Generic;
using QFramework;
using champs3.Hotfix.Generate;
using champs3.Hotfix.Map;

namespace champs3.Hotfix
{
    public static class SettlementHelper
    {
        public static readonly float TileLength = 5f;
        /// <summary>
        /// 结算入口
        /// </summary>
        /// <param name="mapId">地图Id</param>
        /// <param name="playerInfos">玩家信息</param>
        public static void Settlement(int mapId, List<PlayerInfo> playerInfos)
        {
            List<TrackData> trackDatas = new List<TrackData>();
            //加载地图配置
            ResourceManager.Instance.GetAssetAsync<TrackConfigTable>("TrackConfig", table =>
            {
                
                //查询对应地图配置
                var trackConfig = table.Data.Find(config =>
                {
                    return config.Id == mapId;
                });
                
                //计算地图各个路段的长度
                for (int i = 0; i < trackConfig.TrackModule.Length; i++)
                {
                    var module = trackConfig.TrackModule[i];
                    var moduleCount = trackConfig.TrackModuleNum[i];


                    TileType tileType = (TileType)(module >= 1000 ? module : module - 1);
                    
                    switch (tileType)
                    {
                        case TileType.Start:
                        case TileType.End:
                            trackDatas.Add(new TrackData(){TileType = tileType, Length = TileLength * moduleCount / 2});
                            break;
                        case TileType.Ground:
                        case TileType.Water:
                        case TileType.Cliff:
                        case TileType.Flight:
                            trackDatas.Add(new TrackData(){TileType = tileType, Length =  TileLength * moduleCount});
                            break;
                    }
                }
                
                
                for (int i = 0; i < playerInfos.Count; i++)
                {
                    LogKit.I($"{playerInfos[i].name} UseTime : {GetPlayUseTime(trackDatas, playerInfos[i])}");
                }
                
                
            });
        }
        /// <summary>
        /// 计算玩家的成绩
        /// </summary>
        /// <param name="trackDatas">赛道数据</param>
        /// <param name="playerInfo">玩家信息</param>
        /// <returns>玩家跑完所需时间</returns>
        public static float GetPlayUseTime(List<TrackData> trackDatas, PlayerInfo playerInfo)
        {
            float time = 0;

            //道具配表CostItemAttribute
            var costItemAttribute = ExcelConfig.CostItemAttributeTable.Data.Find(attribute =>
            {
                return playerInfo.item_id == attribute.Id;
            });
            //道具配表CostItemAttribute
            CostItemAttribute usiPropsConfig = costItemAttribute;
            
            
            //道具已使用的cis
            int usedTime = 0;
            //道具可使用的次数
            var useTimes = costItemAttribute != null ? int.Parse(costItemAttribute.UsageTimes) : 0;
            for (int i = 0; i < trackDatas.Count; i++)
            {
                
                var trackData = trackDatas[i];
                if (trackData.TileType == TileType.End)
                {
                    continue;
                }
                float speed = 0;
                bool useProps = false;
                
                //判断能否使用道具
                if (costItemAttribute != null)
                {
                    if (useTimes == -1 || useTimes > usedTime)
                    {
                        for (int j = 0; j < costItemAttribute.Terrain.Length; j++)
                        {
                            if (costItemAttribute.Terrain[j] == (int)trackData.TileType + 1)
                            {
                                useProps = true;
                                usedTime++;
                                break;
                            }
                        }
                    }
                }
                
                int speedType = 1;
                switch (trackData.TileType)
                {
                    case TileType.Start:
                    case TileType.Ground:
                        speedType = 1;
                        break;
                    case TileType.Water:
                        speedType = 2;
                        break;
                    case TileType.Cliff:
                        speedType = 3;
                        break;
                    case TileType.Flight:
                        speedType = 4;
                        break;
                }

                speed = GetRoleSpeed(playerInfo.role_info, speedType);
                if (useProps)
                {
                    float propsAdditive = speed * (usiPropsConfig.SpeedIncrease / 100f);
                    //道具无限使用或者道具加成距离大于赛道长度
                    if (usiPropsConfig.Distance == -1 || usiPropsConfig.Distance >= trackData.Length)
                    {
                        time += trackData.Length / (speed + propsAdditive);
                        // LogKit.I($"{playerInfo.name} {trackData.TileType.ToString()} time {time} - {useProps}");
                    }
                    else
                    {
                        time = usiPropsConfig.Distance / (speed + propsAdditive) +
                               (trackData.Length - usiPropsConfig.Distance) / speed;
                        // LogKit.I($"{playerInfo.name} {trackData.TileType.ToString()} time {time} - {useProps}");
                    }
                }
                else
                {
                    time += trackData.Length / speed;
                    // LogKit.I($"{playerInfo.name} {trackData.TileType.ToString()} time {time} - {useProps}");
                }
            }
            return time;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleData"></param>
        /// <param name="type">1。跑步；2.游泳；3.攀爬；4.飞行</param>
        /// <returns></returns>
        public static float GetRoleSpeed(RoleData roleData, int type)
        {
            var roleAttribute = roleData.Attributes.Find(attribute =>
            {
                return attribute.AttriType == type;
            });

            return roleAttribute.Speed;
        }
    }
    

    public struct TrackData
    {
        //路段类型
        public TileType TileType;
        //路段长度
        public float Length;
    }
}