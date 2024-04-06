using System;
using System.Collections.Generic;


namespace ProtoBuf
{
    public class ProtoMessagePool
    {
        private readonly static Dictionary<Type, List<object>> _pool = new Dictionary<Type, List<object>>();
        private readonly static int MaxCapacity = 500;

        public static T Get<T>()
        {
            T val = (T)Get(typeof (T));
            return val;
        }
        
        public static object Get(Type type)
        {
            object obj = null;
            List<object> list = null;
            if (_pool.TryGetValue(type, out list))
            {
                // if (type.ToString() == "ET.FootballGameRoleInfo")
                // {
                //     UnityEngine.Debug.Log($"获取ET.FootballGameRoleInfo {list.Count}");
                // }
                if (list.Count > 0)
                {
                    obj = list[0];
                    var temp = (IMessageBase)obj;
                    if (temp != null)
                    {
                        temp.Init();
                    }   
                    list.RemoveAt(0);
                    // UnityEngine.Debug.Log($"proto对象池获取》》{type}  {list.Count}");
                    return obj;
                }
            }
            // UnityEngine.Debug.Log($"proto新创建》》{type}");
            obj = PType.CreateInstance(type);
            return obj;
        }

        public static void ReleaseAll()
        {
            foreach (var kv in _pool)
                
            {
                List<object> list = kv.Value;
                list.Clear();
            }
            _pool.Clear();
            UnityEngine.Debug.Log("释放全部proto缓存..");
        }

        public static void Release<T>(T obj)
        {
            Type type = typeof (T);
            Release(type, obj);
        }
        
        public static void Release(Type type, object message)
        {
            List<object> list = null;
            if (!_pool.TryGetValue(type, out list))
            {
                list = new List<object>();
                _pool.Add(type, list);
            }

            if (list.Count >= MaxCapacity)
            {
                // UnityEngine.Debug.Log($"proto回收超过》》{type}   {list.Count}");
                // message = null;
                // return;
            }

            if (list.Contains(message))
            {
                UnityEngine.Debug.LogError($"proto重复回收》》{type} {message.GetHashCode()}   {list.Count}");
            }
            list.Add(message);
            // UnityEngine.Debug.Log($"proto回收》》{type} {message.GetHashCode()}  {list.Count}");
        }
        
    }
}