// 
// 2023/12/19

using QFramework;
using UnityEngine;

namespace champs3.Core
{
    public class ClearCache : AbstractState<YooassetUpdateStates, AssetUpdater>
    {
        public ClearCache(FSM<YooassetUpdateStates> fsm, AssetUpdater target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            
            var package = YooAsset.YooAssets.GetPackage(mTarget.PackageName);
            var operation = package.ClearUnusedCacheFilesAsync();
            operation.Completed += operation =>
            {
                mFSM.ChangeState(YooassetUpdateStates.Done);
            };
        }
    }
}