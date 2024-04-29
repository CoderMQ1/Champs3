using System;
using QFramework;
using UnityEngine;

namespace champs3.Hotfix.AssetsBundle
{
    public class YooassetAudioLoaderPool : AbstractAudioLoaderPool
    {
        protected override IAudioLoader CreateLoader()
        {
            return new YooAssetAudioLoader();
        }
    }

    public class YooAssetAudioLoader : IAudioLoader
    {
        public AudioClip Clip { get; }
        private string _assetName;
        public AudioClip LoadClip(AudioSearchKeys audioSearchKeys)
        {
            throw new Exception("not support load sync");
        }

        public void LoadClipAsync(AudioSearchKeys audioSearchKeys, Action<bool, AudioClip> onLoad)
        {
            _assetName = ConstValue.AssetGroup.Sound + audioSearchKeys.AssetName;
            ResourceManager.Instance.GetAssetAsync<AudioClip>(_assetName, clip =>
            {
                
                onLoad?.Invoke(true,clip);
            });
        }

        public void Unload()
        {
            ResourceManager.Instance.UnLoadAsset(_assetName);
        }
    }
}