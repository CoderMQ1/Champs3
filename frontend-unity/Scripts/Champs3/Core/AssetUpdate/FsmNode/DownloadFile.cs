// 
// 2023/12/19

using System.Collections;
using QFramework;
using YooAsset;

namespace champs3.Core
{
    public class DownloadFile : AbstractState<YooassetUpdateStates, AssetUpdater>
    {
        public DownloadFile(FSM<YooassetUpdateStates> fsm, AssetUpdater target) : base(fsm, target)
        {

        }

        protected override void OnEnter()
        {
            base.OnEnter();
            mTarget.StartCoroutine(BeginDownload());
        }

        private IEnumerator BeginDownload()
        {
            var downloader = mTarget.DownloaderOperation;

            // 注册下载回调
            // downloader.OnDownloadErrorCallback = PatchEventDefine.WebFileDownloadFailed.SendEventMessage;
            downloader.OnDownloadProgressCallback = (count, downloadCount, bytes, downloadBytes) =>
            {
                mTarget.OnDownloadFile(count, downloadCount, bytes, downloadBytes);
            };
            downloader.BeginDownload();
            yield return downloader;

            // 检测下载结果
            if (downloader.Status != EOperationStatus.Succeed)
                yield break;
            
            mFSM.ChangeState(YooassetUpdateStates.Done);
        }
    }
}