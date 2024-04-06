using SquareHero.Hotfix;

namespace SquareHero.Core.Loading
{
    public class LoadAssetNode<T> : AbstractLoadingNode where T : UnityEngine.Object
    {
        private T _asset;
        private string _location;
        private IResLoader _loader;

        public LoadAssetNode(string location)
        {
            _location = location;
        }

        protected override void OnStart()
         {
             _loader = ResourceManager.Instance.GetAssetAsync<T>(_location, (asset) =>
             {
                 _asset = asset;
             });
         }


         public override float Progress()
         {
             return _loader.Progress();
         }

         public override bool IsDone()
         {
             return _loader.IsDone();
         }

         public T GetAsset()
         {
             return _asset;
         }

         public override bool CanStart()
         {
             return true;
         }
    }
}