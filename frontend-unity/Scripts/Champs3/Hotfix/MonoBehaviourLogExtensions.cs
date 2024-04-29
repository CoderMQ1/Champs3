// 
// 2023/11/30

using QFramework;
using UnityEngine;

namespace champs3.Hotfix{
public static class MonoBehaviourLogExtensions
{
    
    public static void Log(this MonoBehaviour mono,string msg, params object[] args)
    {
        LogKit.I($"{mono.gameObject.name} : {msg}", args);
    }

    public static void NullCheck(this UnityEngine.Object obj)
    {
        if (obj == null)
        {
            LogKit.E($"object [{obj.name}] check fall");
        }
    }
}
}
