// 
// 2023/12/19

using System.Collections;
using System.Collections.Generic;
using QFramework;
using SquareHero.Core;
using UnityCommon;
using UnityEngine;
using UnityEngine.Networking;
using YooAsset;

namespace SquareHero.Core
{
    public class UpdateVersion: AbstractState<YooassetUpdateStates, AssetUpdater>
    {
        public UpdateVersion(FSM<YooassetUpdateStates> fsm, AssetUpdater target) : base(fsm, target)
        {
            
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            LogKit.I("UpdateVersion");
            mTarget.StartCoroutine(GetStaticVersion());
        }
        
        private IEnumerator GetStaticVersion()
        {
            yield return new WaitForSecondsRealtime(0.5f);

            var package = YooAssets.GetPackage(mTarget.PackageName);
            var operation = package.UpdatePackageVersionAsync();
            yield return operation;


            if (operation.Status == EOperationStatus.Succeed)
            {
                
                mTarget.PackageVersion = operation.PackageVersion;
                // Debug.Log($"远端最新版本为: {operation.PackageVersion}");
                // _machine.ChangeState<FsmUpdateManifest>();
                mFSM.ChangeState(YooassetUpdateStates.UpdateManifest);
            }
            else
            {
                Debug.LogWarning(operation.Error);
                // PatchEventDefine.PackageVersionUpdateFailed.SendEventMessage();
            }
        }
    }
}