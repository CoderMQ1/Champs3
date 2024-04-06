// 
// 2023/12/22

using System;
using System.Collections.Generic;
using SquareHero.Hotfix;
using SquareHero.Hotfix.Model;

namespace SquareHero.Hotfix
{
    public class RoomInfoResponse
    {
        public int code;
        public RoomInfo data;
    }
    [Serializable]
    public class RoomInfo
    {
        public long room_id;
        public int game_mode;
        public int game_type;
        public List<PlayerInfo> player_info_list;
        public List<GiftData> track_rewards;
    }
    [Serializable]
    public class PlayerInfo
    {
        public long user_id;
        public bool is_robot;
        public string name;
        public RoleData role_info;
        public int item_id;
        public int track_id;
        public float consume_time;
    }

    [Serializable]
    public class GiftData
    {
        public List<RewardInfo> RewardList;
        public int TokenItemId;
        public int Total;
        public int TrackId;
    }
    [Serializable]
    public class RewardInfo
    {

        public int RewardType;
        public int TokenNum;
    }
}