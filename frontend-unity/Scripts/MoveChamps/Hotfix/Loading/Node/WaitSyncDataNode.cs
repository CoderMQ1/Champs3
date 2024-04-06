// 
// 2023/12/04

using QFramework;
using SquareHero.Core.Loading;
using SquareHero.Hotfix.Events;


namespace SquareHero.Hotfix
{
    public class WaitSyncDataNode : AbstractLoadingNode
    {
        private float _progress;

        private long _userId;

        private bool _isDone;

        public WaitSyncDataNode(long userId)
        {
            _userId = userId;

            TypeEventSystem.Global.Register<PlayerEvents.OnPlayerInitialized>(TryFinish);
        }

        private void TryFinish(PlayerEvents.OnPlayerInitialized evt)
        {
            if (evt.UserId == _userId)
            {
                LogKit.I($"player {evt.UserId} synced Data");
                _isDone = true;
                _progress = 1f;
                TypeEventSystem.Global.UnRegister<PlayerEvents.OnPlayerInitialized>(TryFinish);
            }
        }

        public override float Progress()
        {
            return _progress;
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