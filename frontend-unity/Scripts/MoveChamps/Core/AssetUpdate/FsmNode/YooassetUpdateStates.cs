// 
// 2023/12/19

namespace SquareHero.Core
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