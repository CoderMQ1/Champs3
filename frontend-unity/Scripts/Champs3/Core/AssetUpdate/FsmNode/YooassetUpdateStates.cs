// 
// 2023/12/19

namespace champs3.Core
{
    public enum YooassetUpdateStates
    {
        Initialize,
        UpdateVersion,
        UpdateManifest,
        CreateDownloader,
        DownloadOver,
        DownloadFile,
        ClearCache,
        Done
    }
}