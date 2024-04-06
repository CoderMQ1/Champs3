// 
// 2023/12/04

using QFramework;
using SquareHero.Hotfix.Events;

namespace SquareHero.Hotfix
{
    public class WaitCreateMapNode : AbstractLoadingNode
    {
        private long _userId;
        private bool _isDone;
        public WaitCreateMapNode(long userId)
        {
            _userId = userId;

            TypeEventSystem.Global.Register<MapEvents.OnClientFinishCreateMap>(TryFinish);
        }

        public void TryFinish(MapEvents.OnClientFinishCreateMap evt)
        {
            LogKit.I($"player {evt.UserId}- {_userId} created map");
            if (evt.UserId == _userId)
            {
                LogKit.I($"player {evt.UserId} created map");
                _isDone = true;
                TypeEventSystem.Global.Register<MapEvents.OnClientFinishCreateMap>(TryFinish);
            }
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