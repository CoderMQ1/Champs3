// 
// 2023/12/19

using System;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YooAsset;

namespace champs3.Core
{
    public class AssetUpdater : MonoBehaviour
    {

        private string _downFileTip = "Downloading file  {0}/{1}M  {2}M/s";
        
        public TextMeshProUGUI Tip;
        public Slider Progress;
        public RawImage Vertical;
        
        public readonly string PackageName = "MainPackage";
        public string PackageVersion;
        public ResourceDownloaderOperation DownloaderOperation;

        public EPlayMode EPlayMode = EPlayMode.EditorSimulateMode;
        
        public FSM<YooassetUpdateStates> FSM = new FSM<YooassetUpdateStates>();
        #region EDITOR EXPOSED FIELDS

        #endregion

        #region FIELDS
        private float checkSpeedPerSeconds = 1; //检测下载速度间隔 单位秒
        private DateTime lastTime;
        private bool downloadStart;
        #endregion

        #region PROPERTIES

        #endregion

        #region EVENT FUNCTION

        private void Awake()
        {
            if (GameStart.Instance.Platform == RuntimePlatform.Android || GameStart.Instance.Platform == RuntimePlatform.IPhonePlayer)
            {
                var canvasScaler = transform.parent.GetComponent<CanvasScaler>();
                canvasScaler.referenceResolution = new Vector2(1080, 1920);
                Vertical.gameObject.SetActive(true);
                Progress.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 63);
            }
        }

        private void Start()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            EPlayMode = EPlayMode.WebPlayMode;
#endif

            Progress.value = 0;
            Progress.gameObject.SetActive(false);
            
            FSM.AddState(YooassetUpdateStates.Initialize, new InitializeNode(FSM, this));
            FSM.AddState(YooassetUpdateStates.UpdateVersion, new UpdateVersion(FSM, this));
            FSM.AddState(YooassetUpdateStates.UpdateManifest, new UpdateManifest(FSM, this));
            FSM.AddState(YooassetUpdateStates.CreateDownloader, new CreateDownloader(FSM, this));
            FSM.AddState(YooassetUpdateStates.DownloadOver, new DownloadOver(FSM, this));
            FSM.AddState(YooassetUpdateStates.DownloadFile, new DownloadFile(FSM, this));
            FSM.AddState(YooassetUpdateStates.ClearCache, new ClearCache(FSM, this));
            FSM.AddState(YooassetUpdateStates.Done, new DoneNode(FSM, this));
            
            
            FSM.StartState(YooassetUpdateStates.Initialize);
        }

        #endregion

        #region METHODS

        public void StartDownLoad(int totalDownloadCount, long totalDownloadBytes)
        {

            OnDownloadFile(totalDownloadCount, 0, totalDownloadBytes, 0);
            
            Progress.gameObject.SetActive(true);
            FSM.ChangeState(YooassetUpdateStates.DownloadFile);
        }

        private long downloadedSize;
        
        public void OnDownloadFile(int totalDownloadCount, int downloadedCount, long totalDownloadBytes, long downloadedBytes)
        {
            float totalDownloadSizeMB = totalDownloadBytes / 1048576f;
            float downloadedSizeMB = downloadedBytes / 1048576f;
            
            double downloadSpeed = 0;
            if (!downloadStart)
            {
                downloadStart = true;
                lastTime = DateTime.Now;
                Tip.text = string.Format(_downFileTip, downloadedSizeMB, totalDownloadSizeMB, 0);
                downloadedSize = downloadedBytes;
                return;
            }
            
            double seconds = (DateTime.Now - lastTime).TotalSeconds;
            if (seconds >= this.checkSpeedPerSeconds)
            {
                downloadSpeed = (downloadedBytes - downloadedSize) / (seconds * 1048576f);
                downloadedSize = downloadedBytes;
                lastTime = DateTime.Now;
                string speed = downloadSpeed.ToString("0.00");
                Tip.text = string.Format(_downFileTip, downloadedSizeMB.ToString("0.00"), totalDownloadSizeMB.ToString("0.00"), speed);
                Progress.value = downloadedBytes * 1F / totalDownloadBytes;
            }
        }

        #endregion

        
    }

}