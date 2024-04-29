// SPDX-License-Identifier: UNLICENSED
pragma solidity >=0.7.0 <0.9.0;
import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/security/ReentrancyGuard.sol";
import "@openzeppelin/contracts/security/Pausable.sol";
import "@chainlink/contracts/src/v0.8/vrf/VRFConsumerBaseV2.sol";
import "@chainlink/contracts/src/v0.8/interfaces/VRFCoordinatorV2Interface.sol";

abstract contract ERC20Interface {
  function transferFrom(address from, address to, uint tokens)  public virtual;
}

abstract contract HeroInterface {
  function mint(address to, uint256 tokenid)  public virtual;
  function fulfillAttr(uint256 tokenid, uint256 rarity, uint256 talent,uint256 RunningSpeed,uint256 SwimmingSpeed,uint256 ClimbingSpeed,uint256 FlightSpeed,uint256 energy) public virtual;
  function getCurrentSupply() public virtual view returns(uint256);
}

abstract contract AssetInterface{
    function addAsset(address user,uint256 itemId,uint256 amount) public virtual ;
}

contract HeroOperator is Ownable, Pausable, ReentrancyGuard, VRFConsumerBaseV2{
    using AddressUtils for address;
    
    event RequestSent(uint256 requestId, uint32 numWords);
    
    event RequestFulfilled(uint256 requestId,uint256[] randomWords);

    struct RequestStatus {
        bool fulfilled; // whether the request has been successfully fulfilled
        bool exists; // whether a requestId exists
        uint256[] tokenIds;
        uint256[] randomWords;
    }

    mapping(uint256 => RequestStatus) public s_requests; /* requestId --> requestStatus */

    VRFCoordinatorV2Interface _COORDINATOR;
    
    //Polygon Mainnet 500 gwei KeyHash
    //bytes32 _keyHash = 0xcc294a196eeeb44da2888d17c0625cc88d70d9760a69d58d853ba6581a9ab0cd;

    //Polygon Mumbai 500 gwei KeyHah
    bytes32 _keyHash = 0x4b09e658ed251bcafeebbc69400383d49f344ace09b9576fe248bb02c003fe9f;

    // Your subscription ID.
    uint64 s_subscriptionId;
    
    uint32 public _callbackGasLimit = 2000000;

    uint16 _requestConfirmations = 3;

    uint32 _numWords = 1;

    ERC20Interface  public _USDT;
    ERC20Interface  public _RTC;
    HeroInterface public _NFT;
    AssetInterface public _ASSET;

    uint256 public _priceInUSDT = 0;
    uint256 public _priceInRTC = 50;
    
    uint256 public _totalSupply;
    uint256 public  _currentSupply;

    uint256 public _whitelistTotalSupply;
    uint256 public  _whitelistCurrentSupply;

    uint256 public _baseTokenId;
    
    uint256 public _rarity1Supply;
    uint256 public _rarity2Supply;
    uint256 public _rarity3Supply;
    uint256 public _rarity4Supply;
    uint256 public _rarity5Supply;

    uint256[2] _energyGenRange1 = [0,90];
    uint256[2] _energyGenRange2 = [100,200];
    uint256[2] _energyGenRange3 = [150,300];
    uint256[2] _energyGenRange4 = [200,400];
    uint256[2] _energyGenRange5 = [400,800];
    uint256 [2][5] public _energyGenRange = [ _energyGenRange1, _energyGenRange2, _energyGenRange3, _energyGenRange4, _energyGenRange5];

    uint256 public _talentsPerRarity = 4;

    // whitelist 
    // 0 not whitelisted
    // 1 whitelisted but not mint yet
    // 2 whitelisted and minted
    mapping(address => uint8) public _whitelist;
    address private _lister;

    address payable public _wallet1;
    address payable public _wallet2;

    uint256 public _invitationRewardAssetId = 10010;

    constructor(uint64 subscriptionId, address payable wallet1, address payable wallet2) 
        Ownable(msg.sender) 
        VRFConsumerBaseV2(0x7a1BaC17Ccc5b313516C5E16fb24f7659aA5ebed)
    {
        _COORDINATOR = VRFCoordinatorV2Interface(0x7a1BaC17Ccc5b313516C5E16fb24f7659aA5ebed);
        _USDT = ERC20Interface(0xc2132D05D31c914a87C6611C10748AEb04B58e8F);

        s_subscriptionId = subscriptionId;

        _wallet1 = wallet1;
        _wallet2 = wallet2;
    }

    function configMint(address nft, uint256 rarity1Supply, uint256 rarity2Supply, uint256 rarity3Supply, uint256 rarity4Supply, uint256 rarity5Supply, uint256 whitelistSupply) onlyOwner external{
        require(nft.isContract(), "The nftContract Address should be a contract");
        
        _NFT = HeroInterface(nft);
        _rarity1Supply = rarity1Supply;
        _rarity2Supply = rarity2Supply;
        _rarity3Supply = rarity3Supply;
        _rarity4Supply = rarity4Supply;
        _rarity5Supply = rarity5Supply;

        _totalSupply = _rarity1Supply + _rarity2Supply + _rarity3Supply + _rarity4Supply + _rarity5Supply;
        _whitelistTotalSupply = whitelistSupply;
        _currentSupply = 0;
        _whitelistCurrentSupply = 0;

        _baseTokenId = _NFT.getCurrentSupply();
    }

    function updateEnergyGenRange(uint256 rarity,uint256 begin,uint256 end) onlyOwner external {
        require(rarity<=4,"input error");
        _energyGenRange[rarity] = [begin,end];
    }

    function requestRandomWords(uint256[] memory tokenIds)
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
            tokenIds: tokenIds,
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
    ) internal  override {
        require(s_requests[_requestId].exists, "request not found");
        s_requests[_requestId].fulfilled = true;
        s_requests[_requestId].randomWords = _randomWords;
        emit RequestFulfilled(
            _requestId,
            _randomWords
        );

        uint256 seed = _randomWords[0];
        for (uint256 i = 0; i < s_requests[_requestId].tokenIds.length; i++){
            uint256 rarity = generateRarity(seed);
            uint256 talent = generateTalent(seed, rarity);
            uint256 energy = generateEnergy(seed, rarity);
            uint256[] memory speed = generateSpeed(seed);

            _NFT.fulfillAttr(s_requests[_requestId].tokenIds[i], rarity, talent, speed[0], speed[1], speed[2], speed[3],energy);
            
            seed = uint256(keccak256(abi.encodePacked(seed)));
        }
    }

    function buyWhitelistBox() external whenNotPaused() nonReentrant() {
        require(whitelisted(msg.sender) == 0,"not whitelisted or already minted"); // just for test
        require(_whitelistCurrentSupply < _whitelistTotalSupply, "whitelist box sold out");
        require(_currentSupply + 1 <= _totalSupply, "exceed total supply");

        uint256 tokenId = _baseTokenId + _currentSupply;

        _NFT.mint(msg.sender, tokenId);

        _currentSupply++;
        _whitelistCurrentSupply++;
        _whitelist[msg.sender] = 2;

        // request random word to generate nft attr
        uint256[] memory tokenIds = new uint256[](1);
        tokenIds[0] = tokenId;
        requestRandomWords(tokenIds);
    }

    function buyHeroBoxesWithUSDT(address inviterAddress, uint256 num) external whenNotPaused() nonReentrant(){
        require(num>0 && num<=10, "num error");
        require(_currentSupply + num <= _totalSupply, "exceed total supply");
        require(!msg.sender.isContract(), "contract not allowed");
        require(msg.sender != inviterAddress, "can not invite yourself");

        if(inviterAddress != 0x0000000000000000000000000000000000000000)
        {
            _ASSET.addAsset(inviterAddress, _invitationRewardAssetId, num);
        }

        if(_priceInUSDT > 0)
        {
            uint256 needPay = _priceInUSDT * num * 10 **6;
            ERC20Interface(_USDT).transferFrom(msg.sender, _wallet1, needPay / 2);
            ERC20Interface(_USDT).transferFrom(msg.sender, _wallet2, needPay / 2);
        }
        

        uint256[] memory tokenIds = new uint256[](num);
        for(uint256 i =0; i < num; i++)
        {
            tokenIds[i] = _baseTokenId + _currentSupply;
            _NFT.mint(msg.sender, tokenIds[i]);
            _currentSupply++;
        }
        
        requestRandomWords(tokenIds);
    }

    function buyHeroBoxesWithRTC(address inviterAddress, uint256 num) external whenNotPaused nonReentrant{
        require(num>0 && num<=10, "num error");
        require(_currentSupply+num <= _totalSupply, "exceed total supply");
        require(!msg.sender.isContract(), "contract not allowed");
        require(msg.sender != inviterAddress, "can not invite yourself");
        
        if(inviterAddress != 0x0000000000000000000000000000000000000000)
        {
            _ASSET.addAsset(inviterAddress, _invitationRewardAssetId, num);
        }

        if(_priceInRTC > 0)
        {
            uint256 needPay = _priceInRTC * num * 10 **18;    //wei
            ERC20Interface(_RTC).transferFrom(msg.sender, _wallet1, needPay / 2);
            ERC20Interface(_RTC).transferFrom(msg.sender, _wallet2, needPay / 2);
        }

        uint256[] memory tokenIds = new uint256[](num);
        for(uint256 i =0; i < num; i++)
        {
            tokenIds[i] = _baseTokenId + _currentSupply;
            _NFT.mint(msg.sender, tokenIds[i]);
            _currentSupply++;
        }
        
        requestRandomWords(tokenIds);
    }

    function pause() external onlyOwner {
        _pause();
    }

    function unpause() external onlyOwner {
        _unpause();
    }

    function setTalentPerRarity(uint256 talents) external onlyOwner{
        _talentsPerRarity = talents;
    }

    function setWallet1( address payable w) external onlyOwner{
        _wallet1 = w;
    }

    function setWallet2( address payable w) external onlyOwner{
        _wallet2 = w;
    }

    function setCallbackGasLimit(uint32 gas) external onlyOwner{
        _callbackGasLimit = gas;
    }

    function setRTCContract(address rtc) external onlyOwner{
        require(rtc.isContract(), "The rtc Address should be a contract");
        _RTC = ERC20Interface(rtc);
    }

    function setUSDTPrice(uint256 price) external onlyOwner{
        _priceInUSDT = price;
    }

    function setRTCPrice(uint256 price) external onlyOwner{
        _priceInRTC = price;
    }

    function setAssetContract(address asset) external onlyOwner{
        _ASSET = AssetInterface(asset);
    }

    function setWhitelister(address lister) external onlyOwner{
        _lister = lister;
    }

    function setInvitationRewardAssetId(uint256 id) external onlyOwner{
        _invitationRewardAssetId = id;
    }

    function addToWhitelist(address[] memory a) external{
        require(_lister == msg.sender, "not allowed");
        for(uint256 i = 0; i < a.length; i++){
            if(_whitelist[a[i]] != 1)
            {
                _whitelist[a[i]] = 1;
            }
        }
    }

    function removeFromWhitelist(address[] memory a) external{
        require(_lister == msg.sender, "not allowed");
        for(uint256 i = 0; i < a.length; i++){
            delete _whitelist[a[i]];
        }
    }

    function whitelisted(address a) public view returns(uint8){
        return _whitelist[a] ;
    }

    function generateRandomNumber(uint256 seed, uint256 range1, uint256 range2) pure private returns (uint256) {
        require(range1 <= range2, "Invalid range");
        return range1 + seed % (range2 - range1 + 1);
    }

    function generateRarity(uint256 seed) private returns(uint256){
        uint256 randNum = generateRandomNumber(seed, 1, _rarity1Supply + _rarity2Supply + _rarity3Supply + _rarity4Supply + _rarity5Supply);

        uint256 L1MAX = _rarity1Supply; 
        uint256 L2MAX = _rarity1Supply + _rarity2Supply;
        uint256 L3MAX = L2MAX + _rarity3Supply;
        uint256 L4MAX = L3MAX + _rarity4Supply;
        uint256 L5MAX = L4MAX + _rarity5Supply;
        
        uint256 rarity = 0;

        // fulfill rarity and update raritySupply
        if (randNum <= L1MAX)
        {
            rarity = 1;
            _rarity1Supply --;
        } 
        else if (randNum <= L2MAX)
        {
            rarity = 2;
            _rarity2Supply --;
        }
        else if (randNum <= L3MAX){
            rarity = 3;
            _rarity3Supply --;
        } 
        else if (randNum <= L4MAX)
        {
             rarity = 4;
             _rarity4Supply --;
        }
        else if (randNum <= L5MAX)
        {
            rarity = 5;
            _rarity5Supply --;
        } 
        
        return rarity;
    }

    function generateTalent(uint256 seed, uint256 rarity) view private returns (uint256) {
        if(rarity == 1) return 0;
        return (rarity - 2) * _talentsPerRarity + generateRandomNumber(seed, 1, _talentsPerRarity);
    }

    function generateEnergy(uint256 seed, uint256 rarity) private view returns (uint256) {
        uint256 begin = _energyGenRange[rarity-1][0];
        uint256 end = _energyGenRange[rarity-1][1];
        return generateRandomNumber(seed, begin, end);
    }

    function generateSpeed(uint256 seed) private pure returns(uint256[] memory){
        uint256 R  = seed / (10 ** 40);
        int256 Rs = (int256)(R);
        int256 A = (Rs % 9) + 1;
        Rs /= 10;
        int256 B = (Rs % 9) + 1;
        Rs /= 10;
        int256 C = (Rs % 9) + 1;
        Rs /= 10;
        int256 D = 20 - A - B - C;

        if (D <= 0) {
            A += D;
            D = 20 - A - B - C;
        }

        uint256[] memory values = new uint256[](4);
        values[0] = uint256(A);
        values[1] = uint256(B);
        values[2] = uint256(C);
        values[3] = uint256(D);
        return values;
    }
}

library AddressUtils {

  /**
   * Returns whether the target address is a contract
   * @dev This function will return false if invoked during the constructor of a contract,
   *  as the code is not actually created until after the constructor finishes.
   * @param addr address to check
   * @return whether the target address is a contract
   */
  function isContract(address addr) internal view returns (bool) {
    uint256 size;
    // XXX Currently there is no better way to check if there is a contract in an address
    // than to check the size of the code at that address.
    // See https://ethereum.stackexchange.com/a/14016/36603
    // for more details about how this works.
    // TODO Check this again before the Serenity release, because all addresses will be
    // contracts then.
    assembly { size := extcodesize(addr) }  // solium-disable-line security/no-inline-assembly
    return size > 0;
  }

}