// 
// 2023/12/19

using UnityCommon.Util;

namespace SquareHero
{
    public class SHServerConfig
    {
        public int code;
        public SHServerConfigData data;


        private static SHServerConfig _mInstance;

        public static SHServerConfig Instance
        {
            get
            {
                return _mInstance;
            }
        }

        public static SHServerConfig Initialize(string fromJson)
        {
            _mInstance = JsonUtil.FromJson<SHServerConfig>(fromJson);
            return _mInstance;
        }
    }

    public class SHServerConfigData
    {
        public string client_name;
        public string platform;
        public string Version;
        public int build;
        public string resource_url;
    }
}