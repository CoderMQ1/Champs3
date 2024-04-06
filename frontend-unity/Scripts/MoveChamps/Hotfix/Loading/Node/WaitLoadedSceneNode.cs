// 
// 2023/12/04

using QFramework;
using SquareHero.Core.Loading;
using SquareHero.Hotfix.Events;


namespace SquareHero.Hotfix
{
    public class WaitLoadedSceneNode : AbstractLoadingNode
    {
        private string _sceneName;
        
        private long _userId;

        private bool _isDone;

        public WaitLoadedSceneNode(string sceneName, long userId)
        {
            _sceneName = sceneName;
            _userId = userId;
            TypeEventSystem.Global.Register<GameEvents.OnClientLoadedScene>(TryFinish);
        }

        private void TryFinish(GameEvents.OnClientLoadedScene evt)
        {
            // LogKit.I($"player {evt.UserId} loaded scene {evt.SceneName}");
            // if (evt.UserId == _userId && _sceneName.Equals(evt.SceneName))
            // {
            //     
            //     _isDone = true;
            //     TypeEventSystem.Global.Register<GameEvents.OnClientLoadedScene>(TryFinish);
            // }
            
        }


        public override float Progress()
        {
            return _isDone ? 1 : 0;
        }

        public override bool IsDone()
        {
            return _isDone;
        }

        public override bool CanStart()
        {
            return true;
        }
    }
}