// 
// 2023/12/13

using System;
using System.Collections.Generic;

namespace SquareHero.Hotfix
{
    public struct BaseResponse
    {
        public int code;
    }


    public struct GetNonceResponse
    {
        public int code;
        public bool status;
        public string message;
        public NonceData data;
    }
    

    public struct NonceData
    {
        public string nonce;
    }

    public struct WalletLoginData
    {
        public string wallet_address;
        public string nonce;
        public string signature;
        public string register_code;
    }

    public struct WalletLoginResponse
    {
        public int code;
        public bool statue;
        public string message;
        public string data;
    }

    public struct LoginResponse
    {
        public int code;
        public bool status;
        public string message;
        public string data;
    }

    public struct LoginData
    {
        public long user_id;
        public string user_name;
        public string player_code;
        public bool need_choose_server;
        public bool need_relogin;
        public string relogin_addr;
    }
    
    public struct keepData
    {
        
    }

    public struct GetPvpPriveResponse
    {
        public int code;
        public PvpPriceData data;
    }

    public struct PvpPriceData
    {
        public long pvp_fee;
    }

    public struct GetRoleResponse
    {
        public int code;
        public RoleDataList data;
    }
    
    public struct RoleDataList
    {
        public List<RoleData> user_role_list;
    }
    [Serializable]
    public class RoleData
    {
        public long NftID;
        public string Character;
        public int Rarity;
        public int TalentId;
        public List<RoleAttribute> Attributes;
        public int Level;
        public int Energy;
    }
    [Serializable]
    public struct RoleAttribute
    {
        public int AttriType;

        public int AttriValue;

        public int TalentValue;

        public float Speed;


    }

    public struct SelectRole
    {
        public long room_id;
        public long nft_id;
    }
    public struct SelectTrack
    {
        public long room_id;
        public int track_id;
    }
    public struct SelectProps
    {
        public long room_id;
        public long item_id;
    }
    
    public struct SelectPropsResponse
    {
        public int code;
    }
    

    #region 资产

    public struct PlayerassetsResponse
    {
        public int code;
        public bool status;
        public List<PlayerAsset> data;
    }

    public struct PlayerAsset
    {
        public int id;
        public int user_id;
        public string asset_name;
        public float available_amount;
        public float freeze_amount;
    }


    public struct AllPropResponse
    {
        public int code;
        public AllPropResponseData data;
    }
    
    public struct AllPropResponseData
    {
        public List<PropsData> user_item_list;
    }

    public class PropsData
    {
        public int item_id;
        public int grade;
        public int amount;
        public int price_amount;
        public string price_token;
    }

    public struct BuyMessage
    {
        public int goods_item_id;
        public int amount;
    }
    public struct BuyMessageResponse
    {
        public int code;
    }
    #endregion

    #region 抽奖
    
    public struct SpinExchangeResponse
    {
        public int code;
        public int key_num;
        public int chip_num;
    }
    
    public struct SpinResponse
    {
        public int code;
        public int reward_index;
        public int reward_effect;
        public int reward_item_id;
        public int reward_amount;
    }

    public struct SpinInfoResponse
    {
        public int code;
        public long bonus_pool;
        public long jackpot;
    }

    #endregion

    #region 充值

    public struct WalletDataResponse
    {
        public int code;
        public bool status;
        public string message;
        public WalletData data;
    }

    public struct WalletData
    {
        public string user_wallet_addr;
        public string recharge_addr;
        public string token_name;
        public string contrace_address;
        public float token_balance;
        public float withdraw_min_amount;
        public float withdraw_fee;

    }

    public struct RechargeData
    {
        public string token_name;
        public string contract_address;
        public float amount;
    }

    #endregion


    public struct MapInfoResponse
    {
        public int code;
        public MapInfoData data;
    }

    public struct MapInfoData
    {
        public long map_key;
        public int map_id;
    }


    public struct GetHistoryResponse
    {
        public int code;
        public HistoryData Data;
    }

    public class HistoryData
    {
        public List<RecordData> Records { get; set; }
    }

    public class RecordData
    {
        public long GameId;
        public long UserId;
        public long MapId;
        public int GameMode;
        public long GameStartTime;
        public long NftId;
        public int ItemId;
        public string Role;
        public int Rank;
        public List<RewardData> Rewards;
    }

    public struct RewardData
    {
        public long item_id;
        public long quantity;
    }

    public struct ApproveStatusResponse
    {
        public int code;
        public ApproveStatusData data;
    }

    public struct ApproveStatusData
    {
        public bool is_nft_approve;
        public bool is_token_approve;
    }

    public struct InviteRewardResponse
    {
        public int code;
        public InviteRewardData Data;
    }
    
    public struct InviteRewardData
    {
        public int total_reward_num;
    }
}