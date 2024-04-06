// 
// 2023/12/22

using System.IO;
using UnityCommon.Util;
using UnityEngine;

namespace SquareHero.Hotfix
{
    public class GameServerConfig
    {
        public bool IsDebug = false; //是否是调试模式
        public long RoomId = 0; //房间Id
        public int GameMode = 1;//游戏模式 1，休闲 2，竞技
        public int GameType = 1;//游戏类型
        public int Round = 1;//游戏轮次
        public int GameMaxPlayerCount = 5; //房间最大人数
        public int MapId = 0;
        public string ServerAddress; //服务器地址
        public ushort ServerPort; //服务器端口
        public string SettlementUrl = "http://106.75.175.56/api/settle/major/round"; //结算URL
        public string RoomInfoUrl = "http://106.75.175.56/api/settle/major/round"; //结算URL
        public string RoomInfo = "";

        /// <summary>
        /// 使用命令行参数更新游戏配置
        /// </summary>
        /// <param name="commandLineType"></param>
        /// <param name="value"></param>
        public void UpdateValueByCommandLine(KtCommandLineType commandLineType, string value)
        {
            switch (commandLineType)
            {

                case KtCommandLineType.Ip:
                {
                    this.ServerAddress = value;
                    break;
                }
                case KtCommandLineType.Port:
                {
                    ushort port = 0;
                    if (ushort.TryParse(value, out port))
                    {
                        this.ServerPort = port;
                    }
                    else
                    {
                        Debug.LogError($"命令行赋值错误:{commandLineType}--{value}");
                    }
                    break;
                }
                case KtCommandLineType.SettlementUrl:
                {
                    this.SettlementUrl = value;
                    break;
                }
                case KtCommandLineType.RoomInfoUrl:
                {
                    this.RoomInfoUrl = value;
                    break;
                }
                case KtCommandLineType.RoomInfo:
                {
                    this.RoomInfo = value;
                    break;
                }
                case KtCommandLineType.RoomId:
                {
                    long id = 0;
                    if (long.TryParse(value, out id))
                    {
                        this.RoomId = id;
                    }
                    else
                    {
                        Debug.LogError($"命令行赋值错误:{commandLineType}--{value}");
                    }
                    break;
                }
                case KtCommandLineType.MapId:
                {
                    int id = 0;
                    if (int.TryParse(value, out id))
                    {
                        this.MapId = id;
                    }
                    else
                    {
                        Debug.LogError($"命令行赋值错误:{commandLineType}--{value}");
                    }
                    break;
                }
                case KtCommandLineType.GameMaxPlayerCount:
                {
                    ushort id = 0;
                    if (ushort.TryParse(value, out id))
                    {
                        this.GameMaxPlayerCount = id;
                    }
                    else
                    {
                        Debug.LogError($"命令行赋值错误:{commandLineType}--{value}");
                    }
                    break;
                }
                
                case KtCommandLineType.GameMode:
                {
                    int id = 0;
                    if (int.TryParse(value, out id))
                    {
                        this.GameMode = id;
                    }
                    else
                    {
                        Debug.LogError($"命令行赋值错误:{commandLineType}--{value}");
                    }
                    break;
                }
                case KtCommandLineType.Round:
                {
                    int round = 0;
                    if (int.TryParse(value, out round))
                    {
                        this.Round = round;
                    }
                    else
                    {
                        Debug.LogError($"命令行赋值错误:{commandLineType}--{value}");
                    }
                    break;
                }
                case KtCommandLineType.GameType:
                {
                    int type = 0;
                    if (int.TryParse(value, out type))
                    {
                        this.GameType = type;
                    }
                    else
                    {
                        Debug.LogError($"命令行赋值错误:{commandLineType}--{value}");
                    }
                    break;
                }
                case KtCommandLineType.ServerType:
                {
                    int type = 0;
                    if (int.TryParse(value, out type))
                    {
                        this.GameType = type;
                    }
                    else
                    {
                        Debug.LogError($"命令行赋值错误:{commandLineType}--{value}");
                    }
                    break;
                }
                default:
                {
                    Debug.Log($"命令行赋值GameServerConfig未定义:{commandLineType}--{value}");
                    break;
                }
            }
        }
        
        /// <summary>
        /// 加载游戏配置
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static GameServerConfig Load(string path)
        {
            Debug.Log($"load config:{path}");
            if (!File.Exists(path))
            {
                Debug.LogError($"GameServerConfig is not exist! : {path}");
                return new GameServerConfig();
            }

            string json = File.ReadAllText(path);
            Debug.Log(json);
            GameServerConfig config  = JsonUtil.FromJson<GameServerConfig>(json);
            // Debug.Log($"----->{config.ServerType}");
            return config;
        }
    }
}