// 
// 2023/12/26

using UnityEngine;

namespace SquareHero.Hotfix
{
    public class HttpJsonPostNode : AbstractLoadingNode
    {
        protected bool Loaded;

        protected string Result;

        protected MonoBehaviour MonoBehaviour;

        protected string Url;
        protected string JsonData;
        public HttpJsonPostNode(MonoBehaviour monoBehaviour, string url, string jsonData)
        {
            MonoBehaviour = monoBehaviour;
            Url = url;
            JsonData = jsonData;
        }


        protected override void OnStart()
        {
            base.OnStart();

            MonoBehaviour.StartCoroutine(HttpHelper.PostJson(Url, JsonData, result =>
            {
                SetResult(result);
            }));
        }

        public override float Progress()
        {
            return Loaded ? 1 : 0;
        }

        public override bool IsDone()
        {
            return Loaded;
        }

        public override bool CanStart()
        {
            return true;
        }

        public void SetResult(string result)
        {
            Result = result;
            Loaded = true;
        }

        public string GetResult()
        {
            return Result;
        }
    }
}