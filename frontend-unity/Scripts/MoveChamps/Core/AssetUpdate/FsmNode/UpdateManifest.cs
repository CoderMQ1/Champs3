// 
// 2023/12/19

using System.Collections;
using QFramework;
using UnityEngine;
using YooAsset;

namespace SquareHero.Core
{
    public class UpdateManifest : AbstractState<YooassetUpdateStates, AssetUpdater>
    {
        public UpdateManifest(FSM<YooassetUpdateStates> fsm, AssetUpdater target) : base(fsm, target)
        {
            
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            LogKit.I("UpdateManifest");
            mTarget.StartCoroutine(StartUpdateManifest());
        }
        
        private IEnumerator StartUpdateManifest()
        {
            yield return new WaitForSecondsRealtime(0.5f);

            bool savePackageVersion = true;
            var package = YooAssets.GetPackage(mTarget.PackageName);
            //PatchManager.Instance.PackageVersion
            var operation = package.UpdatePackageManifestAsync(mTarget.PackageVersion, savePackageVersion);
            yield return operation;

            if(operation.Status == EOperationStatus.Succeed)
            {
                // _machine.ChangeState<FsmCreateDownloader>();
                mFSM.ChangeState(YooassetUpdateStates.CreateDownloader);
            }
            else
            {
                Debug.LogWarning(operation.Error);
                // PatchEventDefine.PatchManifestUpdateFailed.SendEventMessage();
            }
        }
    }
}