// SPDX-License-Identifier: GPL-3.0
pragma solidity >=0.7.0 <0.9.0;

abstract contract NftRoleInterface {
  function setAdmin(address user, bool enabled) public virtual;
}

abstract contract HeroTokenInterface {
  function setAdmin(address user, bool enabled) public virtual;
}

abstract contract SpinInterface {
  function setAdmin(address user, bool enabled) public virtual;
  function loadLuckSpinSet(address gameConfigContract) public virtual;
}

abstract contract GameAssetInterface {
  function setAdmin(address user, bool enabled) public virtual;
}

abstract contract GameLogicInterface {
  function setAdmin(address user, bool enabled) public virtual;
  function setGameData(address gameConfigContract) public virtual;
}

abstract contract ShopInterface {
  function loadShopSet(address gameConfigContract)  public virtual;
}



contract GameConfig {

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

    struct LuckSpinRewardPumpSet {
        uint MineaAcumulate;
        uint MineProportion;
        uint CompetitiveProportion;
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

    struct ShopGood {
        uint GoodItemId;
        uint PriceItemId;
        uint PriceAmount;
    }

    struct ShopSet {
        address PayTokenContract;
        address Receiver;
        ShopGood[] ShopGoodInfos;
    }
    
    

    address public owner;
    ContractAddrSet public ContractAddrInfo;
    ItemConfigSet public ItemConfigInfo;
    uint256 GamePlayerNum;
    NftRoleInterface NftRole;
    HeroTokenInterface HeroToken;
    SpinInterface Spin;
    GameAssetInterface GameAsset;
    GameLogicInterface GameLogic;
    ShopInterface Shop;

    SpinRewardSet[] public SpinRewards;
    LuckSpinRewardPumpSet[] public LuckSpinRewardPumps;
    RoleUpgradeSt public RoleUpgradeInfo;
    ShopSet public  ShopInfo;

    modifier onlyOwner() {
        require(msg.sender == owner, "Only contract owner can call this function");
        _;
    }

    constructor() {
        owner = msg.sender;
    }

    function setContractAddrSet(address nftRoleContract,address heroTokenContract,address spinContract,address gameAssetContract,address heroBoxContract,address gameLogicContract,address shopContract) public  onlyOwner{
        ContractAddrInfo.NftRoleContract = nftRoleContract;
        ContractAddrInfo.HeroTokenContract = heroTokenContract;
        ContractAddrInfo.SpinContract = spinContract;
        ContractAddrInfo.GameAssetContract = gameAssetContract;
        ContractAddrInfo.HeroBoxContract = heroBoxContract;
        ContractAddrInfo.GameLogicContract = gameLogicContract;
        ContractAddrInfo.ShopContract = shopContract;


        NftRole = NftRoleInterface(nftRoleContract);
        HeroToken = HeroTokenInterface(heroTokenContract);
        Spin = SpinInterface(spinContract);
        GameAsset = GameAssetInterface(gameAssetContract);
        GameLogic = GameLogicInterface(gameLogicContract);
        Shop = ShopInterface(shopContract);
        
        //NftRole.setAdmin(heroBoxContract,true);
        //NftRole.setAdmin(gameLogicContract, true);
        //HeroToken.setAdmin(gameLogicContract, true);
        //Spin.setAdmin(gameLogicContract, true);
        //GameAsset.setAdmin(heroBoxContract, true);
        //GameAsset.setAdmin(spinContract, true);
        //GameAsset.setAdmin(gameLogicContract, true);
        //GameAsset.setAdmin(shopContract, true);
    }



    function getContractAddrSet() public view returns (ContractAddrSet memory) {
        return ContractAddrInfo;
    }

    function setGamePlayerNum(uint256 playerNum) public  onlyOwner {
        GamePlayerNum = playerNum;
    }

    function getGamePlayerNum() public view returns (uint256) {
        return GamePlayerNum;
    }

    function setItemConfigSet(uint256 spinCostItemId,uint256 batteryItemId,uint256 pvpConsumeItemId,uint256 heroTokenItemId) public  onlyOwner{
        ItemConfigInfo.SpinCostItemId = spinCostItemId;
        ItemConfigInfo.BatteryItemId = batteryItemId;
        ItemConfigInfo.PvpConsumeItemId = pvpConsumeItemId;
        ItemConfigInfo.HeroTokenItemId = heroTokenItemId;
    }

    function getItemConfigSet() public view returns (ItemConfigSet memory) {
        return ItemConfigInfo;
    }

    function setLuckSpinReward(uint256[][] memory array)  public  onlyOwner {
        delete SpinRewards;
        for (uint i = 0; i < array.length; i++) {  
            SpinRewardSet memory set;
            set.BlockSort = array[i][0];
            set.MustTimes = array[i][1];
            set.Weight = array[i][2];
            set.CostPoolNum = array[i][3];
            set.RewardEffect = array[i][4];
            set.RewardType = array[i][5]; //1:res 2:pool
            set.RewardID = array[i][6];
            set.RewardNum = array[i][7];
            SpinRewards.push(set);
        }
    }

    function getLuckSpinSet() public view returns (SpinRewardSet[] memory spinRewards)
    {
            spinRewards =  SpinRewards;
    }

    function setLuckSpinRewardPumps(uint256[][] memory array)  public  onlyOwner {
        delete LuckSpinRewardPumps;
        for (uint i = 0; i < array.length; i++){
            LuckSpinRewardPumpSet memory set;
            set.MineaAcumulate = array[i][0];
            set.MineProportion = array[i][1];
            set.CompetitiveProportion = array[i][2];
            LuckSpinRewardPumps.push(set);
        }
    }

    function getLuckSpinRewardPumps() public view returns(LuckSpinRewardPumpSet[] memory){
            return LuckSpinRewardPumps;
    }

    function roleUpgradeSet(address payTokenContract,uint256[][]memory array) public onlyOwner {
        RoleUpgradeInfo.PayTokenContract = payTokenContract;
        delete RoleUpgradeInfo.RoleUpgradeItems;
        for (uint i = 0; i < array.length; i++) {
            RoleUpgrade memory set;
            set.CurrentLevel = array[i][0];
            set.UpgradeLevel = array[i][1];
            set.NeedMoney = array[i][2];
            set.UpgradeAttribute = array[i][3];
            RoleUpgradeInfo.RoleUpgradeItems.push(set);
        }
    }

    function getRoleUpgradeSet() public view returns (RoleUpgradeSt memory) {
        return RoleUpgradeInfo;
    }

    function shopSet(address payTokenContract,address receiver,uint256[][]memory array) public onlyOwner {
        ShopInfo.PayTokenContract = payTokenContract;
        ShopInfo.Receiver = receiver;
        for (uint i = 0; i < array.length; i++) {
            ShopGood memory good;
            good.GoodItemId = array[i][0];
            good.PriceItemId = array[i][1];
            good.PriceAmount = array[i][2];
            ShopInfo.ShopGoodInfos.push(good);
        }
    }

    function getShopSet() public view returns (ShopSet memory) {
        return ShopInfo;
    }

    function ReloadContractConfig() public  onlyOwner{
        //GameLogic.setGameData(address(this));
        //Spin.loadLuckSpinSet(address(this));
        //Shop.loadShopSet(address(this));
    }
}