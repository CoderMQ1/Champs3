// 
// 2023/12/19

using QFramework;
using YooAsset;

namespace SquareHero.Core
{
    public class DoneNode : AbstractState<YooassetUpdateStates, AssetUpdater>
    {
        public DoneNode(FSM<YooassetUpdateStates> fsm, AssetUpdater target) : base(fsm, target)
        {
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            mTarget.Tip.text = "Download Asset Finish";
            YooAssets.LoadSceneAsync("Scene_StartScene");
        }
    }
}