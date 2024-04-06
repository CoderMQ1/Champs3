using SquareHero.Hotfix;
using UnityEngine.SceneManagement;

namespace SquareHero.Core.Loading
{
    public class LoadSceneNode : AbstractLoadingNode
    {
        private string _location;
        private LoadSceneMode _loadSceneMode;
        private bool _suspendLoad;
        private int _priority;
        private ISceneLoader _sceneLoader;

        public LoadSceneNode(string location, LoadSceneMode sceneMode = LoadSceneMode.Single, bool suspendLoad = false, int priority = 100)
        {
            _location = location;
            _loadSceneMode = sceneMode;
            _suspendLoad = suspendLoad;
            _priority = priority;
        }

        protected override void OnStart()
        {
            base.OnStart();

            _sceneLoader = ResourceManager.Instance.LoadSceneAsync(_location, () => { }, _loadSceneMode, _suspendLoad, _priority);
        }


        public override float Progress()
        {
            return _sceneLoader.Progress();
        }

        public override bool IsDone()
        {
            return _sceneLoader.isDone();
        }

        public override bool CanStart()
        {
            return true;
        }
    }
}