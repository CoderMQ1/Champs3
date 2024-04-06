// 
// 2023/12/15


using UnityEngine;

namespace SquareHero.Hotfix
{
    public class HttpGetNode : AbstractLoadingNode
    {
        protected bool Loaded;

        protected string Result;

        protected MonoBehaviour MonoBehaviour;

        protected string Url;
        public HttpGetNode(MonoBehaviour monoBehaviour, string url)
        {
            MonoBehaviour = monoBehaviour;
            Url = url;
        }


        protected override void OnStart()
        {
            base.OnStart();

            MonoBehaviour.StartCoroutine(HttpHelper.Get(Url, result =>
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