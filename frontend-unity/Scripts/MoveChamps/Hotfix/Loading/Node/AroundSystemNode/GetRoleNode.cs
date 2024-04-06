// 
// 2023/12/28

using System.Collections.Generic;
using QFramework;
using SquareHero.Hotfix.Main;
using SquareHero.Hotfix.Model;
using UnityCommon.Util;
using UnityEngine;

namespace SquareHero.Hotfix
{
    public class GetRoleNode : HttpGetNode
    {
        public GetRoleNode(MonoBehaviour monoBehaviour) : base(monoBehaviour, GameUrlConstValue.GetRolesData.Url())
        {
        }


        protected override void OnFinish()
        {
            base.OnFinish();
            var result = GetResult();
            var getRoleResponse = JsonUtil.FromJson<GetRoleResponse>(result);
            var mainModel = MainController.Instance.GetArchitecture().GetModel<MainModel>();
            mainModel.RoleDatas = getRoleResponse.data.user_role_list;
            if (mainModel.RoleDatas == null)
            {
                mainModel.RoleDatas = new List<RoleData>();
            }
            mainModel.RoleDatas.Sort((a, b) =>
            {
                if (a.Energy > b.Energy)
                {
                    return -1;
                }

                if (a.Energy < b.Energy)
                {
                    return 1;
                }

                return 0;
            });
        }
    }
}