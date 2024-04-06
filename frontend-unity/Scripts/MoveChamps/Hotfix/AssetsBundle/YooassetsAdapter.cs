// 
// 2023/12/15

using System.Collections;
using QFramework;
using YooAsset;

namespace SquareHero.Hotfix
{
    public static class YooassetsAdapter
    {

        private static bool _initialized;
        
        public static IEnumerator InitializeYooAsset(string mainPackage)
        {
            if (!_initialized)
            {
                YooAssets.Initialize(new YooAssetLogImpl());
                if (!YooAssets.HasPackage(mainPackage))
                {
                    var resourcePackage = YooAssets.CreatePackage("MainPackage");
                    if (resourcePackage == null)
                    {
                        LogKit.E("MainPackage not found");
                    }
                    YooAssets.SetDefaultPackage(resourcePackage);
#if UNITY_EDITOR
                    var initParameters = new EditorSimulateModeParameters();
                    initParameters.SimulateManifestFilePath  = EditorSimulateModeHelper.SimulateBuild("MainPackage");
                    yield return resourcePackage.InitializeAsync(initParameters);
#elif UNITY_WEBGL
                    string defaultHostServer = "http://127.0.0.1/CDN/WebGL/v1.0";
                    string fallbackHostServer = "http://127.0.0.1/CDN/WebGL/v1.0";
                    var createParameters = new WebPlayModeParameters();
                    createParameters.BuildinQueryServices = new GameQueryServices();
                    createParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                    yield return resourcePackage.InitializeAsync(createParameters);
#else
                    var initParameters = new OfflinePlayModeParameters();
                    yield return resourcePackage.InitializeAsync(initParameters);
#endif
                }
            }
        }

    }
}