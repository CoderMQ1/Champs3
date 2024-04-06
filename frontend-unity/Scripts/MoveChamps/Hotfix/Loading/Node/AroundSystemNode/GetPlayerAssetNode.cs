// 
// 2023/12/28

using QFramework;
using SquareHero.Hotfix.Events;
using UnityCommon.Util;
using UnityEngine;

namespace SquareHero.Hotfix
{
    public class GetPlayerAssetNode : HttpGetNode
    {
        public GetPlayerAssetNode(MonoBehaviour monoBehaviour) : base(monoBehaviour, GameUrlConstValue.GetAsset.Url())
        {
        }

        protected override void OnFinish()
        {
            base.OnFinish();
            
            var result = GetResult();
            var playAssetResponse = JsonUtil.FromJson<PlayerassetsResponse>(result);
            TypeEventSystem.Global.Send(new SystemEvents.UpdatePlayerAssets()
            {
                Response = playAssetResponse
            });
            UIManager.Instance.SendMsg(new UIEvents.RefreshPlayerAssets()
            {
                Response = playAssetResponse
            });
        }
    }
}