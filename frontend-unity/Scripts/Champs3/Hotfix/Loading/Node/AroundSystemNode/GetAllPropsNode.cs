// 
// 2023/12/28

using QFramework;
using UnityCommon.Util;
using UnityEngine;

namespace champs3.Hotfix
{
    public class GetAllPropsNode : HttpGetNode
    {
        public GetAllPropsNode(MonoBehaviour monoBehaviour) : base(monoBehaviour, GameUrlConstValue.AllItem.Url())
        {
        }


        protected override void OnFinish()
        {
            base.OnFinish();
            var result = GetResult();
            AllPropResponse response = JsonUtil.FromJson<AllPropResponse>(result);
            TypeEventSystem.Global.Send(new SystemEvents.UpdatePlayerProps()
            {
                PropDatas = response.data.user_item_list
            });
        }
    }
}