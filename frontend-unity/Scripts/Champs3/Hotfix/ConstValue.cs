// 
// 2023/12/26

using champs3.Core;
using UnityEngine.TextCore.Text;

namespace champs3.Hotfix
{
    public static class ConstValue
    {

        public static bool Debug = false;
        
        public readonly static string CoinAssetsName = "DPM";
        public readonly static string DiamondAssetsName = "FPU";
        public readonly static string SpinChip = "Luck-chip|10005";
        public readonly static string SpinKey = "Luck-key|10006";

        public readonly static int TokenId = 2;  
        public readonly static int SpinChipId = 10005;  
        public readonly static int SpinKeyId = 10006;  
        public readonly static int EnergyDrinkId = 10010;


        public static class GameText
        {
            public readonly static string TokenNotEnoughTip = "You don't have enough Tokens";
        }

        #region AssetLocation

        public readonly static string AssetSplit = "_";
        
        public static class AssetGroup
        {
            public readonly static string Atlas = "Atlas";
            public readonly static string Texture = "Texture";
            public readonly static string Config = "Config";
            public readonly static string Sound = "Sound" + AssetSplit;
        }

        public static class AtlasLocation
        {
            public readonly static string Character = AssetGroup.Atlas + AssetSplit +"Character";
            public readonly static string Attributes = AssetGroup.Atlas + AssetSplit +"Attributes";
            public readonly static string Props = AssetGroup.Atlas + AssetSplit +"Props";
            public readonly static string Map = AssetGroup.Atlas + AssetSplit +"Map";
            public readonly static string PropsGrade = AssetGroup.Atlas + AssetSplit +"PropGrade";
            public readonly static string LuckSpin = AssetGroup.Atlas + AssetSplit +"LickSpin";
        }

        public static class ConfigLocation
        {
            public readonly static string LuckSpinRewardSetting =  "LuckSpinRewardSetting";
            public readonly static string ItemConfig =  "ItemConfig";
        }

        #endregion
    }


    public enum PropsGrade
    {
        Common = 1,
        UnCommon,
        Rare,
        Legendary,
        Epic
    }

    public class GameUrlConstValue
    {
        public static string GuestAddress = "";
#if UNITY_WEBGL && !UNITY_EDITOR
        public static string AssetAddress 
        {
            get
            {
                if (DebugProfile.Instance.Debug)
                {
                    return "http://106.75.176.67/";
                }

                return "https://info.champs3.io/";
            }
        }
#else
         public static string AssetAddress = "https://info.champs3.io/";
#endif

        public static readonly GameUrl Login = new GameUrl("api/v1/common/login_by_email", GameUrlType.Asset);
        public static readonly GameUrl GetNonce = new GameUrl("api/v1/web/get_nonce", GameUrlType.Asset);
        public static readonly GameUrl WalletLogin = new GameUrl("api/v1/common/login_by_wallet", GameUrlType.Asset);
        public static readonly GameUrl GetRolesData = new GameUrl("api/pack/role/list", GameUrlType.Asset);
        public static readonly GameUrl SelectRole = new GameUrl("api/pack/role/select", GameUrlType.Asset);
        public static readonly GameUrl GetAsset = new GameUrl("api/v1/web/user/asset/list", GameUrlType.Asset);
        public static readonly GameUrl SelectTrack = new GameUrl("api/roommgr/select_track", GameUrlType.Asset);
        public static readonly GameUrl SelectProps = new GameUrl("api/pack/item/select", GameUrlType.Asset);
        public static readonly GameUrl UpgradeRole = new GameUrl("/api/pack/role/upgrade", GameUrlType.Asset);
        public static readonly GameUrl AllItem = new GameUrl("/api/pack/item/all", GameUrlType.Asset);
        public static readonly GameUrl Buy = new GameUrl("api/ecomic/shop/buy", GameUrlType.Asset);
        public static readonly GameUrl Spin = new GameUrl("api/ecomic/lottery/spin", GameUrlType.Asset);
        public static readonly GameUrl SpinExchange = new GameUrl("api/ecomic/lottery/exchange", GameUrlType.Asset);
        public static readonly GameUrl SpinInfo = new GameUrl("api/ecomic/lottery/info", GameUrlType.Asset);
        public static readonly GameUrl WalletData = new GameUrl("api/v1/web/user/asset/recharge_withdraw_info", GameUrlType.Asset);
        public static readonly GameUrl Rechange = new GameUrl("api/v1/web/user/asset/withdraw", GameUrlType.Asset);
        public static readonly GameUrl MapInfo = new GameUrl("api/roommgr/map", GameUrlType.Asset);
        public static readonly GameUrl PvpPrice = new GameUrl("api/roommgr/pvp_fee", GameUrlType.Asset);
        public static readonly GameUrl RoomInfo = new GameUrl("api/roommgr/racing_info", GameUrlType.Asset);
        public static readonly GameUrl History = new GameUrl("api/roommgr/racing_record", GameUrlType.Asset);
        public static readonly GameUrl ApproveStatus = new GameUrl("api/roommgr/approve_status", GameUrlType.Asset);
        public static readonly GameUrl InviteReward = new GameUrl("api/userinfo/invite_reward", GameUrlType.Asset);
    }

}