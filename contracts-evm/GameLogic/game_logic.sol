// SPDX-License-Identifier: GPL-3.0
pragma solidity >=0.7.0 <0.9.0;
import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/security/ReentrancyGuard.sol";

struct Attr{
    uint256 rarity;
    uint256 level;
    uint256 talent;
    uint256 runningSpeed;
    uint256 swimmingSpeed;
    uint256 climbingSpeed;
    uint256 flightSpeed;
    uint256 energy;
}

abstract contract RoleNftInterface {

  function Consume(uint256 tokenId) public virtual;
  function Empower (uint256 tokenId,uint256 amount) public virtual;
  function getAttrByTokenId(uint256 tokenId) public virtual view returns(Attr memory);
  function Upgrade(uint256 tokenid,uint256 plusRun,uint256 plusSwim,uint256 plusFlight,uint256 plusClimb) public virtual;
  function ownerOf(uint256 tokenId) public view virtual returns (address);
}

struct RoleUpgrade {
        uint CurrentLevel;
        uint UpgradeLevel;
        uint NeedMoney;
        uint UpgradeAttribute;
}
struct RoleUpgradeSt{
    address PayTokenContract;
    RoleUpgrade[] RoleUpgradeItems;
}

struct LuckSpinRewardPumpSet {
    uint MineAcumulate;
    uint MineProportion;
    uint CompetitiveProportion;
}

struct ContractAddrSet {
        address NftRoleContract;
        address HeroTokenContract;
        address SpinContract;
        address GameAssetContract;
        address HeroBoxContract;
        address GameLogicContract;
}

struct ItemConfigSet {
    uint256 SpinCostItemId;
    uint256 BatteryItemId;
    uint256 PvpConsumeItemId;
    uint256 HeroTokenItemId;
}

abstract contract GameConfigInterface {

  function getRoleUpgradeSet() public virtual  view returns (RoleUpgradeSt memory);
  function getLuckSpinRewardPumps() public virtual view returns(LuckSpinRewardPumpSet[] memory);
  function getContractAddrSet() public virtual view returns (ContractAddrSet memory);
  function getItemConfigSet() public virtual view returns (ItemConfigSet memory);
  function getGamePlayerNum() public virtual view returns (uint256);
}

abstract contract ERC20Interface {
  function transferFrom(address from, address to, uint tokens)  public virtual;
}

abstract contract HeroTokenInterface {
  function transferFrom(address from, address to, uint tokens)  public virtual;
  function releaseToken() public virtual ;
  function getCurMinerInfo() public virtual view returns (MineInfo memory);
}


abstract contract SpinInterface {
  function addBonusPool(uint256 amount) public virtual;
}


abstract contract GameAssetInterface {
  function addAsset(address player,uint256 itemId,uint256 amount) public virtual;
  function getAsset(address player,uint256 itemId)  public virtual returns(uint256);
  function minusAsset(address,uint256 itemId,uint256 amount) public virtual;
}


struct PlayerInfo {
    address Addr;
    uint256 tokenId;
}
struct GameRoomInfo{
    uint256 RoomId;
    uint256 BlockNum;
    PlayerInfo[] Players; 
}
struct Reward{
    uint256 ItemId;
    uint256 Amount;
}
struct PlayerReward{
    address Addr;
    Reward[] Rewards;
}
struct SettleResult {
    uint256 roomId;
    PlayerReward[] PlayerRewards;
}

struct GameData {
    address NftRoleContract;
    address GameConfigContract;
    address HeroTokenContract;
    address SpinContract;
    address GameAssetContract;
    uint256 SpinCostItemId;
    uint256 BatteryItemId;
    uint256 PvpConsumeItemId;
    uint256 HeroTokenItemId;
    uint256 PlayerNum;
}

struct MineInfo {
    uint256 CurRound;
    uint256 CurMineAmount;
}


contract GameLogic is Ownable,ReentrancyGuard {

    event MatchProgress(uint256 roomId,address player,uint256 tokenId,uint256 targetNum);
    event MatchSuccess(uint256 roomId);
    event GameSettleLog(uint256 roomId,address player,uint256 rank,uint256 itemId,uint256 amount);
    event PlayerStartGameLog(uint256 gameMode,uint256 raceMapKey,address player,uint256 nftId,uint256 itemId);
    event PvpFeeUpdate(uint256 pvpFee, uint256 updateTime);
    
    GameData public gameData;
    RoleNftInterface public roleNft;
    GameConfigInterface public gameConfig;
    ERC20Interface public upgradePayToken;
    HeroTokenInterface public heroToken;
    SpinInterface public spin;
    GameAssetInterface public gameAsset;
    GameRoomInfo public gameRoom;
    RoleUpgradeSt public upgradeInfo;
    LuckSpinRewardPumpSet[] public LuckSpinRewardPumps;
    uint256 public PvpFee;
    uint256 public LastUpdatePvpFeeTime;


    mapping(address => bool) internal _admins;
    

    constructor()Ownable(msg.sender){}

    function setAdmin(address user, bool enabled) onlyOwner public {
        _admins[user] = enabled;
    }

    function isAdmin(address user) public view returns (bool) {
        return _admins[user];
    }

    //function setGameData(address nftRoleContract,uint256 spinCostItemId,address gameConfigContract,address heroTokenContract,address spinContract,address gameAssetContract,uint256 batteryItemId) public onlyOwner {
    function setGameData(address gameConfigContract) public onlyOwner {
        gameConfig = GameConfigInterface(gameConfigContract);
        ContractAddrSet memory contractInfos = gameConfig.getContractAddrSet();
        ItemConfigSet memory itemConfigInfo = gameConfig.getItemConfigSet();

        gameData.NftRoleContract = contractInfos.NftRoleContract;
        gameData.GameConfigContract = gameConfigContract;
        gameData.SpinCostItemId = itemConfigInfo.SpinCostItemId;
        gameData.HeroTokenContract = contractInfos.HeroTokenContract;
        gameData.SpinContract = contractInfos.SpinContract;
        gameData.GameAssetContract = contractInfos.GameAssetContract;
        gameData.BatteryItemId = itemConfigInfo.BatteryItemId;
        gameData.PvpConsumeItemId = itemConfigInfo.PvpConsumeItemId;
        gameData.HeroTokenItemId = itemConfigInfo.HeroTokenItemId;
        gameData.PlayerNum = gameConfig.getGamePlayerNum();

        roleNft = RoleNftInterface(contractInfos.NftRoleContract);
        
        RoleUpgradeSt memory upgradeSet = gameConfig.getRoleUpgradeSet();
        upgradeInfo.PayTokenContract = upgradeSet.PayTokenContract;
        delete upgradeInfo.RoleUpgradeItems;

        for (uint i = 0; i < upgradeSet.RoleUpgradeItems.length; i++) {
            upgradeInfo.RoleUpgradeItems.push(upgradeSet.RoleUpgradeItems[i]);
        }

        LuckSpinRewardPumpSet[] memory pumps = gameConfig.getLuckSpinRewardPumps();
        delete LuckSpinRewardPumps;
        for(uint i = 0; i < pumps.length; i++) {
            LuckSpinRewardPumps.push(pumps[i]);
        }
        
        upgradePayToken = ERC20Interface(upgradeInfo.PayTokenContract);
        heroToken = HeroTokenInterface(contractInfos.HeroTokenContract);
        spin = SpinInterface(contractInfos.SpinContract);
        gameAsset = GameAssetInterface(contractInfos.GameAssetContract);

        updatePvpFee();
    } 

    function setAssetContract(address gameAssetContract,uint256 batteryItemId) public onlyOwner {
        gameData.GameAssetContract = gameAssetContract;
        gameData.BatteryItemId = batteryItemId;
        gameAsset = GameAssetInterface(gameAssetContract);
    }

    function getProportion(uint256 mineCount) internal  view returns(LuckSpinRewardPumpSet memory set){
        for (uint256 i = 0; i < LuckSpinRewardPumps.length; i++) {
            if (mineCount < LuckSpinRewardPumps[i].MineAcumulate) {
                return LuckSpinRewardPumps[i];
            }
        }
    }

    function updatePvpFee() internal {
        if (block.timestamp >= LastUpdatePvpFeeTime + 3600){
                MineInfo memory mineInfo = heroToken.getCurMinerInfo();
                uint256 curMineAmount = mineInfo.CurMineAmount;
                PvpFee = curMineAmount/gameData.PlayerNum;
                LastUpdatePvpFeeTime = block.timestamp;
                emit PvpFeeUpdate(PvpFee, LastUpdatePvpFeeTime);
        }
    }

    function playerInGame(address player) internal view returns(bool) {
        for (uint i = 0; i < gameRoom.Players.length; i++) {
            if (gameRoom.Players[i].Addr == player){
                return true;
            }
        }
        return false;
    }
    function startGame(uint256 gameMode,uint256 raceMapKey,uint256 tokenId,uint256 itemId) public {
        require(msg.sender==roleNft.ownerOf(tokenId),"not owner of nft ");
        require(gameMode == 1 || gameMode == 2,"game Mode should be 1 or 2");
        if (gameMode == 1) {
            Attr memory attr = roleNft.getAttrByTokenId(tokenId);
            require(attr.energy > 0,"enenry of nft is out of usage");
            //require(playerInGame(msg.sender)==false,"player has been in game");
            roleNft.Consume(tokenId);
        }
        
        if (gameMode == 2) {
            heroToken.transferFrom(msg.sender, owner(), PvpFee);
        }
        
        if(itemId!=0){
            gameAsset.minusAsset(msg.sender,itemId,1);
        }

        emit PlayerStartGameLog(gameMode,raceMapKey,msg.sender,tokenId,itemId);
        updatePvpFee();
    }

    function roleUpgrade(uint256 tokenId) public {
        Attr memory attr = roleNft.getAttrByTokenId(tokenId);
        uint curLevel = attr.level;
        uint needMoney =  upgradeInfo.RoleUpgradeItems[curLevel].NeedMoney;
        uint attribute = upgradeInfo.RoleUpgradeItems[curLevel].UpgradeAttribute;

        upgradePayToken.transferFrom(msg.sender, owner(), needMoney);

        uint256 seed = block.timestamp;
        uint256 plusRun = 0;uint256 plusSwim = 0;uint256 plusClimb = 0;uint256 plusFlight = 0;
        uint plusAttr = 0;
        for(uint i = 0; i< attribute; i++)
        {
            plusAttr = seed % 4;
            if (plusAttr == 0){
                plusRun++;
            }else if (plusAttr == 1) {
                plusSwim++;
            }else if (plusAttr == 2) {
                plusClimb++;
            }else{
                plusFlight++;
            }
            seed = uint256(keccak256(abi.encodePacked(seed)));
        }
        roleNft.Upgrade(tokenId,plusRun,plusSwim,plusClimb,plusFlight);
    }

    function settle(uint256 roomId,uint256 gameMode,address[] memory rankAddr) public {
        require(
            _admins[msg.sender],
            "Only a admin can operator"
        );
        require(gameMode==1 || gameMode==2,"game mode not valid");
        require(rankAddr.length>0,"rankAddr length not valid");
        heroToken.releaseToken();
        uint bounsToken;
        uint256 rewardToken;
        MineInfo memory mineInfo = heroToken.getCurMinerInfo();
        LuckSpinRewardPumpSet memory pumpSet = getProportion(mineInfo.CurRound);
        if (gameMode == 1){ //mine mode
             bounsToken = mineInfo.CurMineAmount *  pumpSet.MineProportion/100;
             rewardToken = mineInfo.CurMineAmount - bounsToken;
        } else if (gameMode == 2) {
            bounsToken = mineInfo.CurMineAmount *  pumpSet.CompetitiveProportion/100;
             rewardToken = mineInfo.CurMineAmount - bounsToken;
        }
       
        spin.addBonusPool(bounsToken);

        uint256[] memory rewardList = new uint256[](rankAddr.length);
 
        SettleResult memory settleResult;
        settleResult.PlayerRewards = new PlayerReward[](rankAddr.length);
        settleResult.roomId = roomId;
        uint256 seed = block.timestamp;
        for (uint i = 0; i < rewardToken; ) {
            uint256 index = seed%rankAddr.length;
            uint256 amount = rewardToken/rankAddr.length;
            if ((i + amount) < rewardToken) {
                rewardList[index] += amount;
                i+= amount;
            }else{
                rewardList[index] += (rewardToken-i);
                break;
            }
            seed = uint256(keccak256(abi.encodePacked(seed)));
        }

        for (uint i = 0; i < rankAddr.length; i++) {
            if(rewardList[i] > 0){
                heroToken.transferFrom(owner(), rankAddr[i], rewardList[i]);
            }

            PlayerReward memory playerReward;
            playerReward.Rewards = new Reward[](2);
            playerReward.Addr = rankAddr[i];
            playerReward.Rewards[0].ItemId = gameData.HeroTokenItemId;
            playerReward.Rewards[0].Amount = rewardList[i];
            emit GameSettleLog(roomId,rankAddr[i],i+1,playerReward.Rewards[0].ItemId,playerReward.Rewards[0].Amount);
            if (i==0){
                playerReward.Rewards[1].ItemId = gameData.SpinCostItemId;
                playerReward.Rewards[1].Amount = 1;
                emit GameSettleLog(roomId,rankAddr[i],i+1,playerReward.Rewards[1].ItemId,playerReward.Rewards[1].Amount);
            }
            settleResult.PlayerRewards[i]=playerReward;
        }

        gameAsset.addAsset(rankAddr[0], gameData.SpinCostItemId, 1);

        //emit GameSettleLog(roomId,settleResult);
    }

    function exchangeEnergy(uint256 tokenId,uint256 amount) public {
        uint256 balance = gameAsset.getAsset(msg.sender, gameData.BatteryItemId); 
        require(balance>=amount,"balance is not enough");
        gameAsset.minusAsset(msg.sender,gameData.BatteryItemId,amount);
        roleNft.Empower(tokenId, amount);
    }

}