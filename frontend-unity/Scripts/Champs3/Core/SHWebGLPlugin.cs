using System.Runtime.InteropServices;

namespace champs3.Core
{
    public class SHWebGLPlugin
    {

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern string GetLocationURLFunction();
        [DllImport("__Internal")]
        public static extern void CopyToClipboard(string text);
#else
        public static string GetLocationURLFunction()
        {
            return "http://localhost:9003/";
        }

        public static void CopyToClipboard(string text)
        {
            
        }
#endif

    }
}