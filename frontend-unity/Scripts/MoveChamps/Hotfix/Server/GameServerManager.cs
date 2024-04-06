// 
// 2023/12/22

using System;
using System.Collections;
using System.IO;
using QFramework;
using SquareHero.Hotfix;
using UnityCommon.Util;
using UnityEngine;

namespace SquareHero.Hotfix
{
    public class GameServerManager : Singleton<GameServerManager>
    {
        public GameServerConfig ServerConfig;
        public RoomInfo RoomInfo;
        public int AllPlayerCount = 5;
        public int RobotCount = 4;
        public int RelPlayerCount = 1;

        private GameServerManager()
        {
        }

        private void InitServerConfig()
        {
            string path = Path.GetDirectoryName(Application.dataPath);
            string configPath = $"{path}/{ServerConstValue.GameServerConfigName}";
            ServerConfig = GameServerConfig.Load(configPath);
        }


        /// <summary>
        /// 解析命令行参数
        /// </summary>
        private void ParseCommandLine()
        {
            // 如果在编辑器模式下，不解析命令行参数
            if (Application.isEditor)
            {
                return;
            }

            Debug.Log("start parse command line");
            var args = Environment.GetCommandLineArgs(); // 获取命令行参数
            for (int i = 0; i < args.Length; i++)
            {
                var param = args[i].ToLower();

                // 检查参数是否以"-"开头
                if (param.StartsWith("-"))
                {
                    // 将参数转换为枚举类型
                    KtCommandLineType commandLineType = ConvertToKtCommandType(param);
                    if (commandLineType != KtCommandLineType.None)
                    {
                        int valueIdx = i + 1;
                        if (valueIdx >= args.Length)
                        {
                            // 如果没有对应的值，记录错误信息
                            Debug.LogError($"当前{commandLineType}类型不存在值..");
                            continue;
                        }

                        // 获取参数值
                        string commandValue = args[valueIdx].ToLower().Trim();
                        Debug.Log($"command line: {commandLineType} -- {commandValue}");

                        // 更新配置文件中的值
                        this.ServerConfig.UpdateValueByCommandLine(commandLineType, commandValue);
                    }
                }
            }
        }

        // 字符串转换为枚举类型 
        private KtCommandLineType ConvertToKtCommandType(string value)
        {
            string type = value.Substring(1); //移除 -
            foreach (KtCommandLineType e in Enum.GetValues(typeof(KtCommandLineType)))
            {
                if (e.ToString().ToLower() == type)
                {
                    return e;
                }
            }

            Debug.Log($"当前参数类型不存在..{value}");
            return KtCommandLineType.None;
        }


        public IEnumerator InitializeAsync(Action onComplete)
        {
            InitServerConfig();
            ParseCommandLine();
            yield return ResourceManager.Instance.InitializeSync();
            yield return HttpHelper.Get($"{ServerConfig.RoomInfoUrl}?room_id={ServerConfig.RoomId}", result =>
            {
                RoomInfoResponse roomInfoResponse = JsonUtil.FromJson<RoomInfoResponse>(result);
                RoomInfo = roomInfoResponse.data;
            });
#if !UNITY_EDITOR
            RelPlayerCount = 0;
            AllPlayerCount = RoomInfo.player_info_list.Count;
            for (int i = 0; i < RoomInfo.player_info_list.Count; i++)
            {
                if (RoomInfo.player_info_list[i].is_robot)
                {
                    RobotCount++;
                    continue;
                }
                
                if (!RoomInfo.player_info_list[i].is_robot)
                {
                    RelPlayerCount++;
                }
                
            }
#endif

            onComplete?.Invoke();
        }
    }
}