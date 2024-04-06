using ProtoBuf;
using System.Collections.Generic;
namespace SquareHero.Hotfix.Generate
{
	public enum KeepOpCode 
	{
		_KeepOpCodeNone = 0, // 心跳包占用
		// IM
		Op_C2G_Login = 1,
		Op_G2C_Login = 2,
		Op_G2G_UserLogin = 3,
		Op_C2D_Request = 4,
		Op_D2C_RequestAck = 5,
		Op_D2C_Notify = 6,
		Op_C2D_NotifyAck = 7,
		Op_D2D_NotifyUser = 8,
		Op_G2D_UserLoginNotify = 9,
		Op_SendRoomMessage = 10,
		Op_SendRoomMessageAck = 11,
		Op_GetRoomLastMsg_Request = 12,
		Op_GetRoomLastMsg_Resp = 13,
		Op_G2D_GetFriendList = 14,
		Op_GetFriendListAck = 15,
		Op_AddFriend = 16,
		Op_AddFriendAck = 17,
		Op_FindFriend = 18,
		Op_FindFriendAck = 19,
		Op_D2D_NotifyFriendApply = 20,
		Op_D2C_NotifyFriendApply = 21,
		Op_C2D_NotifyFriendApplyACK = 22,
		Op_D2C_NotifyFriendApplyACK = 23,
		Op_D2C_NotifyFriendApplyResult = 24,
		Op_G2D_UserLoginCheckFriendApply = 25,
		Op_D2C_UserLoginReturnFriendApply = 26,
		// Room
		Op_C2G_EnterRoom = 27,
		Op_G2C_EnterRoom = 28,
		Op_G2G_UserEnterRoom = 29,
		Op_C2R_ExitRoom = 30,
		Op_R2C_ExitRoom = 31,
		Op_G2R_UserExitRoom = 32,
		OP_C2R_ChooseGame = 33,
		OP_R2C_ChooseGame = 34,
		OP_C2R_PlayerReady = 35,
		OP_R2C_PlayerReady = 36,
		OP_C2R_KickOut = 37,
		OP_R2C_KickOut = 38,
		OP_KickedOutMessage = 39,
		OP_RoomPlayerInfoMessage = 40,
		OP_ChooseGameResultMessage = 41,
		OP_PlayerReadyMessage = 42,
		OP_DestroyRoomMessage = 43,
		Op_G2R_UserDisconnect = 44,
		OP_C2R_Invite = 45,
		OP_R2C_Invite = 46,
		OP_C2R_StartGame = 47,
		OP_R2C_StartGame = 48,
		OP_RoomMatchSuccessMessage = 49,
		OP_MatchProgressMessage = 50,
		OP_C2R_ExitMatch = 51,
		OP_R2C_ExitMatch = 52,
		// Publish
		Op_C2A_SceneChange = 53, // 客户端切换场景
		Op_A2C_SceneActive = 54, // 推送场景变化信息
		Op_C2A_ReadPopUp = 55,   // 确认弹窗已读
		Op_S2C_UserSettle = 56, // 向客户端推送结算数据
		// Racing 竞速模式
		OP_C2R_StartMatchRacing = 57, //竞速模式匹配请求
		OP_R2C_StartMatchRacing = 58,
		OP_C2R_ExitRacingMatch = 59, //退出请求
		OP_R2C_ExitRacingMatch = 60,
		OP_RacingTrackSelectMessage = 61, //赛道选择消息
		OP_RacingGameStart = 62, //竞速比赛开始
		Op_G2C_ReloginOffline = 63, // 顶号下线
		// Settle
		Op_S2C_RoomUserSettleResult = 64,
		//spin
		OP_S2C_SPinResultMessage = 65,
		// Maincity room
		Op_C2G_Add_MainCityRoom = 100, //客户端请求进入主城房间
		Op_G2C_Add_MainCityRoom = 101,
	}

	public enum ErrCode 
	{
		Ok = 0,
		ErrMissingSetVal = 1,    // 缺失配表数据
		ErrRequestFiled = 2,     // 请求参数错误
		ErrRpcFiled = 3,         // 调用Rpc错误
		ErrStateIsFaild = 4,     // 状态不符合
		ErrNoAliaveInstance = 5, // 没有可供选择的节点
		ErrUnkonow = 6,          // 未知错误
		ErrBalanceNoPaid = 7,    // 余额不足，无法支付
		ErrUserNotFound = 8, //未找到user
		ErrFriendRequestProcessing = 9,//好友申请对方处理中
		ErrAlreadyFriend = 10,//双方已是好友
		ErrFriendUpperLimit = 11,//好友达到上限
		ErrUserOffLine = 12, //玩家离线
		ErrUserAlreadyInRoom = 13, //玩家已在房间
		ErrUserAlreadyInGame = 14, //玩家在游戏中
		ErrUserRoomIsFull = 15, //房间已满
		ErrUserRoomIsInGame = 16, //房间已在游戏中
		ErrUserRoomDestroyed = 17, //房间被销毁
		ErrMatchGroupIsFull = 18, //匹配资源已经耗尽
		ErrUserAccountPassword = 19,    // 账户或密码错误
		ErrUserTokenInvalid = 20,    // Token校验失败
		ErrRacingTrackUsed = 21,    // 赛道已被占用
	}

	[ProtoContract]
	public partial class S2C_UserSettle
	{
		[ProtoMember(1)]
		public long game_id { get; set; }

		[ProtoMember(2)]
		public int rank { get; set; }

		[ProtoMember(3)]
		public long item_id { get; set; }

		[ProtoMember(4)]
		public int quantity { get; set; }

		[ProtoMember(5)]
		public int extra { get; set; }

		[ProtoMember(6)]
		public bool is_limit { get; set; }

	}
	[ProtoContract]
	public partial class C2G_Login
	{
		[ProtoMember(1)]
		public string Token { get; set; }

	}
	[ProtoContract]
	public partial class G2C_Login
	{
		[ProtoMember(1)]
		public ErrCode error { get; set; }

		[ProtoMember(2)]
		public long UserID { get; set; }

		[ProtoMember(3)]
		public string UserName { get; set; }

		[ProtoMember(4)]
		public long SkinID { get; set; }

		[ProtoMember(5)]
		public string PlayerCode { get; set; }

	}
	[ProtoContract]
	public partial class G2G_UserLogin
	{
		[ProtoMember(1)]
		public long UserID { get; set; }

		[ProtoMember(2)]
		public long InstanceID { get; set; }

	}
	public enum MState 
	{
		MStateNone = 0,
		Request = 1,
		RAck = 2,
		Notify = 3,
		NAck = 4,
		AAck = 5,
	}

	public enum MChatType 
	{
		MChatTypeNone = 0,
		World = 1,   // 世界频道
		Guild = 2,   // 公会频道
		Team = 3,    // 组队频道
		Private = 4, // 私聊
		Room = 5,    //房间
	}

	[ProtoContract]
	public partial class MKey
	{
		[ProtoMember(1)]
		public long MsgId { get; set; }

		[ProtoMember(2)]
		public long Sender { get; set; }

	}
	[ProtoContract]
	public partial class Message
	{
	public enum MContentType 
	{
		MContentTypeNone = 0,
		Text = 1,    // 文字
		Picture = 2, // 图片
	}

		[ProtoMember(1)]
		public MKey Key { get; set; }

		[ProtoMember(2)]
		public MChatType ChatType { get; set; }

		[ProtoMember(3)]
		public long Receiver { get; set; }

		[ProtoMember(4)]
		public MContentType ContentType { get; set; }

		[ProtoMember(5)]
		public string Content { get; set; }

		[ProtoMember(6)]
		public MState State { get; set; }

		[ProtoMember(7)]
		public long ChatSort { get; set; }

		[ProtoMember(8)]
		public string NickName { get; set; }

		[ProtoMember(9)]
		public string Avator { get; set; }

		[ProtoMember(10)]
		public long IsRead { get; set; }

	}
	[ProtoContract]
	public partial class C2D_Request
	{
		[ProtoMember(1)]
		public Message Msg { get; set; }

	}
	[ProtoContract]
	public partial class D2C_RequestAck
	{
		[ProtoMember(1)]
		public ErrCode error { get; set; }

		[ProtoMember(2)]
		public MKey Key { get; set; }

	}
	[ProtoContract]
	public partial class D2C_Notify
	{
	}
	[ProtoContract]
	public partial class C2D_NotifyAck
	{
		[ProtoMember(1)]
		public ErrCode error { get; set; }

		[ProtoMember(3)]
		public MChatType ChatType { get; set; }

	}
	[ProtoContract]
	public partial class D2D_NotifyUser
	{
		[ProtoMember(1)]
		public Message Msg { get; set; }

	}
	[ProtoContract]
	public partial class G2D_UserLoginNotify
	{
		[ProtoMember(1)]
		public long UserID { get; set; }

	}
	[ProtoContract]
	public partial class NotifyRoomUsers
	{
		[ProtoMember(1)]
		public long RoomId { get; set; }

		[ProtoMember(2)]
		public Message Msg { get; set; }

	}
	[ProtoContract]
	public partial class GetRoomLastMsgReq
	{
		[ProtoMember(1)]
		public long RoomId { get; set; }

	}
	[ProtoContract]
	public partial class GetRoomLastMsgResp
	{
		[ProtoMember(2)]
		public ErrCode error { get; set; }

	}
	[ProtoContract]
	public partial class FriendListResp
	{
		[ProtoMember(1)]
		public ErrCode error { get; set; }

	}
	[ProtoContract]
	public partial class FindFriendReq
	{
		[ProtoMember(1)]
		public string PlayerCode { get; set; }

	}
	[ProtoContract]
	public partial class FindFriendResp
	{
		[ProtoMember(1)]
		public ErrCode error { get; set; }

		[ProtoMember(2)]
		public UserInfo UserInfo { get; set; }

	}
	[ProtoContract]
	public partial class AddFriendReq
	{
		[ProtoMember(1)]
		public long Applicant { get; set; }

		[ProtoMember(2)]
		public long Receiver { get; set; }

	}
	[ProtoContract]
	public partial class AddFriendResp
	{
		[ProtoMember(1)]
		public ErrCode error { get; set; }

	}
	[ProtoContract]
	public partial class D2D_NotifyFriendApply
	{
		[ProtoMember(1)]
		public long Applicant { get; set; }

		[ProtoMember(2)]
		public long Receiver { get; set; }

	}
	[ProtoContract]
	public partial class D2C_NotifyFriendApply
	{
		[ProtoMember(1)]
		public UserInfo UserInfo { get; set; }

	}
	[ProtoContract]
	public partial class C2D_NotifyFriendApplyACK
	{
		[ProtoMember(1)]
		public long Applicant { get; set; }

		[ProtoMember(2)]
		public long Receiver { get; set; }

		[ProtoMember(3)]
		public long Result { get; set; }

	}
	[ProtoContract]
	public partial class D2C_NotifyFriendApplyACK
	{
		[ProtoMember(1)]
		public ErrCode error { get; set; }

		[ProtoMember(2)]
		public long Applicant { get; set; }

		[ProtoMember(3)]
		public long Receiver { get; set; }

		[ProtoMember(4)]
		public long Result { get; set; }

	}
	[ProtoContract]
	public partial class D2C_NotifyFriendApplyResult
	{
		[ProtoMember(1)]
		public UserInfo UserInfo { get; set; }

		[ProtoMember(2)]
		public long Result { get; set; }

	}
	[ProtoContract]
	public partial class D2C_UserLoginReturnFriendApply
	{
	}
	[ProtoContract]
	public partial class C2G_Add_MainCityRoom
	{
	}
	[ProtoContract]
	public partial class G2C_Add_MainCityRoom
	{
		[ProtoMember(1)]
		public long RoomId { get; set; }

		[ProtoMember(2)]
		public string CityServerUrl { get; set; }

		[ProtoMember(3)]
		public ErrCode error { get; set; }

	}
	public enum UserInfoStatus 
	{
		UserInfoStatusUnknow = 0,
		UserInfoStatusOnline = 1,  // 在线
		UserInfoStatusOffline = 2, // 离线
		UserInfoStatusPlaying = 3, // 游戏中
		UserInfoStatusRoom = 4,    // 在房间
	}

	public enum SceneState 
	{
		SceneStateUnknow = 0, // 未知
		SceneStateLobby = 1,  // 在大厅
		SceneStateRoom = 2,   // 在房间
		SceneStateGame = 3,   // 在游戏中
	}

	[ProtoContract]
	public partial class UserInfo
	{
		[ProtoMember(1)]
		public long UserID { get; set; }

		[ProtoMember(2)]
		public string UserName { get; set; }

		[ProtoMember(3)]
		public long SkinID { get; set; }

		[ProtoMember(4)]
		public string PlayerCode { get; set; }

		[ProtoMember(5)]
		public UserInfoStatus Status { get; set; }

		[ProtoMember(6)]
		public SceneState CurrentScene { get; set; }

		[ProtoMember(7)]
		public long FriendListID { get; set; }

		[ProtoMember(8)]
		public long RoomID { get; set; }

	}
	[ProtoContract]
	public partial class MatchProgressMessage
	{
		[ProtoMember(1)]
		public long target { get; set; }

		[ProtoMember(2)]
		public long current { get; set; }

	}
	[ProtoContract]
	public partial class C2R_ExitMatch
	{
	}
	[ProtoContract]
	public partial class R2C_ExitMatch
	{
		[ProtoMember(1)]
		public ErrCode error { get; set; }

	}
	[ProtoContract]
	public partial class C2R_StartMatchRacing
	{
		[ProtoMember(1)]
		public long GameType { get; set; }

	}
	[ProtoContract]
	public partial class R2C_StartMatchRacing
	{
		[ProtoMember(1)]
		public ErrCode error { get; set; }

	}
	[ProtoContract]
	public partial class C2R_ExitRacingMatch
	{
	}
	[ProtoContract]
	public partial class R2C_ExitRacingMatch
	{
		[ProtoMember(1)]
		public ErrCode error { get; set; }

	}
	[ProtoContract]
	public partial class RoomMatchSuccessMessage
	{
		[ProtoMember(1)]
		public long RoomId { get; set; }

		[ProtoMember(2)]
		public long CountDown { get; set; }

		[ProtoMember(3)]
		public long MapId { get; set; }

	}
	[ProtoContract]
	public partial class RacingGameStartMessage
	{
		[ProtoMember(1)]
		public long RoomId { get; set; }

		[ProtoMember(2)]
		public string MirrorUrl { get; set; }

	}
	[ProtoContract]
	public partial class RacingTrackSelectMessage
	{
		[ProtoMember(1)]
		public long RoomId { get; set; }

		[ProtoMember(2)]
		public long UserId { get; set; }

		[ProtoMember(3)]
		public long TrackId { get; set; }

	}
	[ProtoContract]
	public partial class G2C_ReloginOffline
	{
	}
	[ProtoContract]
	public partial class RewardInfo
	{
		[ProtoMember(1)]
		public long item_id { get; set; }

		[ProtoMember(2)]
		public int quantity { get; set; }

	}
	[ProtoContract]
	public partial class UserSettleInfo
	{
		[ProtoMember(1)]
		public long userId { get; set; }

		[ProtoMember(2)]
		public long rank { get; set; }
		[ProtoMember(3)]
		public List<RewardInfo> reward_list { get; set; }
	}
	[ProtoContract]
	public partial class RoomUserSettleResult
	{
		[ProtoMember(1)]
		public long game_id { get; set; }
		[ProtoMember(2)]
		public List<UserSettleInfo> UserSettleList { get; set; }

	}
	[ProtoContract]
	public partial class SPinResultMessage
	{
		[ProtoMember(1)]
		public long USerID { get; set; }

		[ProtoMember(2)]
		public string Player { get; set; }

		[ProtoMember(3)]
		public long SpinId { get; set; }

		[ProtoMember(4)]
		public long TargetId { get; set; }

		[ProtoMember(5)]
		public long RewardID { get; set; }

		[ProtoMember(6)]
		public long RewardNum { get; set; }

		[ProtoMember(7)]
		public long BonusPool { get; set; }

		[ProtoMember(8)]
		public long Jackpot { get; set; }

	}
}
