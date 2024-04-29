using QFramework;

namespace Champs3.Core
{
    public class DebugProfile : Singleton<DebugProfile>
    {
        public bool Debug = false;

        private DebugProfile()
        {
            
        }

        public void Initialized()
        {
            var locationURL = SHWebGLPlugin.GetLocationURLFunction();
            ParseUrl(locationURL);
        }

        private void ParseUrl(string url)
        {
            var paramats = url.Split("?");

            for (int i = 0; i < paramats.Length; i++)
            {
                var strs = paramats[i].Split("=");
                if (strs.Length < 2)
                {
                    LogKit.W($"Url error  paramat : {paramats[i]}");
                    continue;
                }
                if (strs[0] == "debug")
                {
                    Debug = bool.Parse(strs[1]);
                    LogKit.I("Debug Mode open");
                }
            }
        }
    }
}