// 
// 2023/12/19

using QFramework;
using UnityEngine;

namespace SquareHero.Core
{
    public class DownloadOver : AbstractState<YooassetUpdateStates, MonoBehaviour>
    {
        public DownloadOver(FSM<YooassetUpdateStates> fsm, MonoBehaviour target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            
            mFSM.ChangeState(YooassetUpdateStates.ClearCache);
        }
    }
}