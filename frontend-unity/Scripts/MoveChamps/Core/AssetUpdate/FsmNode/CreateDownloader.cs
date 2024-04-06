// 
// 2023/12/19

using System.Collections;
using QFramework;
using UnityEngine;
using YooAsset;

namespace SquareHero.Core
{
    public class CreateDownloader : AbstractState<YooassetUpdateStates, AssetUpdater>
    {
        public CreateDownloader(FSM<YooassetUpdateStates> fsm, AssetUpdater target) : base(fsm, target)
        {
            
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            mTarget.StartCoroutine(StartCreateDownloader());
        }

        IEnumerator StartCreateDownloader()
        {
            yield return new WaitForSecondsRealtime(0.5f);

            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            var downloader = YooAssets.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
            mTarget.DownloaderOperation = downloader;

            if (downloader.TotalDownloadCount == 0)
            {
                Debug.Log("Not found any download files !");
                mFSM.ChangeState(YooassetUpdateStates.DownloadOver);
                // _machine.ChangeState<FsmDownloadOver>();
            }
            else
            {
                //A total of 10 files were found that need to be downloaded
                Debug.Log($"Found total {downloader.TotalDownloadCount} files that need download ！");

                // 发现新更新文件后，挂起流程系统
                // 注意：开发者需要在下载前检测磁盘空间不足
                int totalDownloadCount = downloader.TotalDownloadCount;
                long totalDownloadBytes = downloader.TotalDownloadBytes;
                mTarget.StartDownLoad(totalDownloadCount, totalDownloadBytes);
            }
        }
    }
}