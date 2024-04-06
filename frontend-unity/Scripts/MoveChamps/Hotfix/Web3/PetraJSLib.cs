using System.Runtime.InteropServices;

namespace SquareHero.Hotfix
{
    public class PetraJSLib
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        public static extern void ConnectToPetra();
        [DllImport("__Internal")]
        public static extern void Mint();
        [DllImport("__Internal")]
        public static extern void Match();
        #else
        
        public static void ConnectToPetra()
        {
        }
        
        public static void Mint()
        {
        }

        public static void Match()
        {
        }
        
        #endif

    }
}