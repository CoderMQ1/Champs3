// SPDX-License-Identifier: GPL-3.0
pragma solidity >=0.7.0 <0.9.0;
import "@openzeppelin/contracts/security/ReentrancyGuard.sol";
import "@chainlink/contracts/src/v0.8/vrf/VRFConsumerBaseV2.sol";
import "@chainlink/contracts/src/v0.8/interfaces/VRFCoordinatorV2Interface.sol";

struct SpinRewardSet {
        uint BlockSort; 
        uint MustTimes; 
        uint Weight; 
        uint CostPoolNum; 
        uint RewardEffect;
        uint RewardType;
        uint RewardID;
        uint RewardNum;
}

struct ContractAddrSet {
        address NftRoleContract;
        address HeroTokenContract;
        address SpinContract;
        address GameAssetContract;
        address HeroBoxContract;
        address GameLogicContract;
        address ShopContract;
}

struct ItemConfigSet {
    uint256 SpinCostItemId;
    uint256 BatteryItemId;
    uint256 PvpConsumeItemId;
    uint256 HeroTokenItemId;
}

abstract contract GameConfigInterface {

  function getLuckSpinSet() public virtual  view returns (SpinRewardSet[] memory);
  function getContractAddrSet() public virtual view returns (ContractAddrSet memory);
  function getItemConfigSet() public virtual view returns (ItemConfigSet memory);
}

abstract contract GameAssetInterface {
  function addAsset(address player,uint256 itemId,uint256 amount) public virtual;
  function getAsset(address player,uint256 itemId)  public virtual returns(uint256);
  function minusAsset(address,uint256 itemId,uint256 amount) public virtual;
}

abstract contract HeroTokenInterface {
  function transferFrom(address from, address to, uint256 tokens)  public virtual;
  function releaseToken() public virtual returns(uint256);
}

contract Spin is ReentrancyGuard,VRFConsumerBaseV2 {

    event RequestSent(uint256 requestId, uint32 numWords);
    
    event RequestFulfilled(uint256 requestId,uint256[] randomWords);

    struct RequestStatus {
        bool fulfilled; // whether the request has been successfully fulfilled
        bool exists; // whether a requestId exists
        uint256 spinId;
        address playerAddress;
        uint256[] randomWords;
    }


    mapping(uint256 => RequestStatus) public s_requests; /* requestId --> requestStatus */

    VRFCoordinatorV2Interface _COORDINATOR;
    
    //Polygon Mainnet 500 gwei KeyHash
    //bytes32 _keyHash = 0xcc294a196eeeb44da2888d17c0625cc88d70d9760a69d58d853ba6581a9ab0cd;

    //Polygon Mumbai 500 gwei KeyHah
    bytes32 _keyHash = 0x4b09e658ed251bcafeebbc69400383d49f344ace09b9576fe248bb02c003fe9f;

    // The default is 3, but you can set this higher.
    uint16 _requestConfirmations = 3;

    // For this example, retrieve 2 random values in one request.
    // Cannot exceed VRFV2Wrapper.getConfig().maxNumWords.
    uint32 _numWords = 1;

    // Your subscription ID.
    uint64 s_subscriptionId;

    uint32 public _callbackGasLimit = 2000000;


    GameConfigInterface public GameConfig;
    GameAssetInterface public GameAsset;
    HeroTokenInterface public HeroToken;
    
    uint256 public SpinCostItemId;
    uint256 public HeroTokenItemId;
    address public owner;
    SpinRewardSet[] public SpinRewards;

    uint public SpinTotalTimes;
    uint public BonusPool;
    uint public JackPot;
    uint public TotalWeight;
    uint public JackId;
    bool public spinSetInit = false;

    mapping(address => bool) internal _admins;

    //Polygon Mumbai : 0x7a1BaC17Ccc5b313516C5E16fb24f7659aA5ebed
    //Polygon Mainnet: 0xAE975071Be8F8eE67addBC1A82488F1C24858067
    constructor(uint64 subscriptionId)
        VRFConsumerBaseV2(0x7a1BaC17Ccc5b313516C5E16fb24f7659aA5ebed){
        owner = msg.sender;
        BonusPool = 0;
        TotalWeight = 0;

        s_subscriptionId = subscriptionId;
        _COORDINATOR = VRFCoordinatorV2Interface(0x7a1BaC17Ccc5b313516C5E16fb24f7659aA5ebed);
    }

    function setAdmin(address user, bool enabled) onlyOwner public {
        _admins[user] = enabled;
    }

    function isAdmin(address user) public view returns (bool) {
        return _admins[user];
    }

     function requestRandomWords(uint256 spinId)
        public 
        returns (uint256 requestId)
    {
        requestId = _COORDINATOR.requestRandomWords(
            _keyHash,
            s_subscriptionId,
            _requestConfirmations,
            _callbackGasLimit,
            _numWords
        );

        s_requests[requestId] = RequestStatus({
            playerAddress: msg.sender,
            spinId: spinId,
            randomWords: new uint256[](0),
            exists: true,
            fulfilled: false
        });
       
        emit RequestSent(requestId, _numWords);
        return requestId;
    }

    function fulfillRandomWords(
        uint256 _requestId,
        uint256[] memory _randomWords
    ) internal   override{
        require(s_requests[_requestId].exists, "request not found");
        s_requests[_requestId].fulfilled = true;
        s_requests[_requestId].randomWords = _randomWords;

        emit RequestFulfilled(
            _requestId, _randomWords
        );
        doSpin(s_requests[_requestId].spinId,s_requests[_requestId].playerAddress ,_randomWords[0]);
    }

    modifier onlyOwner() {
        require(msg.sender == owner, "Only contract owner can call this function");
        _;
    }

    event spinResult(uint256 spinId,address player,uint random, uint rewardID,uint rewardNum, uint bonusPool,uint jackpot);

    function addBonusPool(uint256 amount) public {
        require(
            _admins[msg.sender],
            "Only a admin can operator"
        );
        BonusPool += amount;
    }

    function loadLuckSpinSet(address gameConfigContract)  public  onlyOwner {
        GameConfig = GameConfigInterface(gameConfigContract);
        SpinRewardSet[] memory list = GameConfig.getLuckSpinSet();
        require(list.length>=1,"luck spin set is null");
        TotalWeight = 0;
        delete SpinRewards;
        for (uint i = 0; i < list.length; i++) {
            SpinRewards.push(list[i]);
            if (list[i].BlockSort==8) { //JackPot
                JackId = i;
                JackPot = (BonusPool * list[i].RewardNum)/1000; 
            }
            TotalWeight += list[i].Weight;
        }
        ContractAddrSet memory contractInfos = GameConfig.getContractAddrSet();
        ItemConfigSet memory itemConfigInfos = GameConfig.getItemConfigSet();
        GameAsset = GameAssetInterface(contractInfos.GameAssetContract);
        HeroToken = HeroTokenInterface(contractInfos.HeroTokenContract);
        SpinCostItemId = itemConfigInfos.SpinCostItemId;
        HeroTokenItemId = itemConfigInfos.HeroTokenItemId;
        spinSetInit = true;
    }


    function doSpin(uint256 spinId,address player,uint256 vrf) internal {
        require(spinSetInit == true && JackId > 0, "spin luck reward is not set");
        uint256 balance = GameAsset.getAsset(player, SpinCostItemId);
        require(balance >= 1, "spin ticket is not enough");
        GameAsset.minusAsset(player, SpinCostItemId, 1);
        uint random = vrf%TotalWeight;
        uint offset = 0;
        for (uint i = 0; i < SpinRewards.length; i++) {
            if(offset <= random && (offset + SpinRewards[i].Weight) > random){
                uint rewardId = SpinRewards[i].RewardID;
                uint rewardNum = SpinRewards[i].RewardNum;
                uint rewardType = SpinRewards[i].RewardType;
                
                if (rewardType == 2){ //pool
                    rewardNum = (BonusPool * rewardNum)/1000;
                    BonusPool -= rewardNum;
                    JackPot =  (BonusPool * SpinRewards[JackId].RewardNum)/1000;   
                    HeroToken.transferFrom(owner, player, rewardNum);
                }else if (rewardType == 1){ //res
                    GameAsset.addAsset(player, rewardId, rewardNum);
                }
                emit spinResult(spinId,player,SpinRewards[i].BlockSort, rewardId,rewardNum,BonusPool,JackPot);
                break;
            }
            offset += SpinRewards[i].Weight;
        }
        
        SpinTotalTimes += 1;   
    }
}