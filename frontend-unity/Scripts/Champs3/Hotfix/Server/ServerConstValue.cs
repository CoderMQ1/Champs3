// 
// 2023/12/22

namespace champs3.Hotfix
{
    public class ServerConstValue
    {
        public const string GameServerConfigName = "GameServerConfig.json";
    }


    /// <summary>
    /// 命令行启动Mirror游戏服务参数
    /// </summary>
    public enum KtCommandLineType
    {
        None,
        ServerType, //服务器类型
        Mode, //运行模式 Server Host Client
        Ip, //服务器Ip
        Port, //服务器端口
        SettlementUrl, //结算地址
        RoomId, //房间Id
        GameMaxPlayerCount, //房间最大人数
        GameMode,
        GameType,
        Round,
        RoomInfoUrl,//获取房间信息地址
        RoomInfo,//房间信息
        MapId, // 地图Id
    }
}