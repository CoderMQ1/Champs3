// SPDX-License-Identifier: UNLICENSED
pragma solidity >=0.7.0 <0.9.0;


import "@openzeppelin/contracts/token/ERC721/extensions/ERC721Pausable.sol";
import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/utils/Strings.sol";

contract RunXHero is Ownable, ERC721Pausable {
    using Strings for uint256;

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

    uint256 public  _totalSupply = 210000;

    uint256 public _batchNum = 10000;
    
    uint256 public _currentSupply;
    
    string public baseURI = "https://resource.runx.xyz/runxheronft/metadata_";

    mapping(address => bool) internal _operators;
    
    mapping (uint256 => Attr) public _attrs;

    event Mint(address indexed to, uint256 indexed tokenid);

    event FulfillAttr(uint256 tokenid, Attr attr);
    
    event Upgrade(uint256 tokenid, uint256 level,uint256 runningSpeed,uint256 swimmingSpeed,uint256 flightSpeed,uint256 climbingSpeed);
    
    event Consume(address playerAddr,uint256 tokenId);
    
    event Empower(address playerAddr,uint256 tokenId, uint256 amount);

    constructor(string memory name, string memory symbol)  ERC721(name, symbol) Ownable(msg.sender){}

    function pause() external onlyOwner {
        _pause();
    }

    function unpause() external onlyOwner {
        _unpause();
    }

    function setOperator(address operator, bool enabled) onlyOwner public {
        _operators[operator] = enabled;
    }

    function setBatchNum(uint256 batchNum) onlyOwner public {
        _batchNum = batchNum;
    }

    function isOperator(address who) public view returns (bool) {
        return _operators[who];
    }

    function getCurrentSupply() public view returns (uint256) {
        return _currentSupply;
    }

    function mint(address to, uint256 tokenid)  public  {
        require(isOperator(msg.sender), "not operator");
        require(_currentSupply < _totalSupply, "exceed totalSupply");

        _safeMint(to, tokenid);
        _currentSupply++;

        emit Mint(to, tokenid);
    }

    function fulfillAttr(uint256 tokenid, uint256 rarity, uint256 talent,uint256 runningSpeed,uint256 swimmingSpeed,uint256 climbingSpeed,uint256 flightSpeed,uint256 energy) public{
        require(isOperator(msg.sender), "not operator");
        _attrs[tokenid].rarity  = rarity;
        _attrs[tokenid].talent = talent;
        _attrs[tokenid].runningSpeed = runningSpeed;
        _attrs[tokenid].swimmingSpeed = swimmingSpeed;
        _attrs[tokenid].flightSpeed = flightSpeed;
        _attrs[tokenid].climbingSpeed = climbingSpeed;
        _attrs[tokenid].energy = energy;
        _attrs[tokenid].level = 0;
        
        emit FulfillAttr(tokenid, _attrs[tokenid]);
    }

    function upgrade(uint256 tokenid,uint256 plusRun,uint256 plusSwim,uint256 plusFlight,uint256 plusClimb) external {
        require(isOperator(msg.sender), "not operator");
        
        _attrs[tokenid].runningSpeed += plusRun;
        _attrs[tokenid].swimmingSpeed += plusSwim;
        _attrs[tokenid].flightSpeed += plusFlight;
        _attrs[tokenid].climbingSpeed += plusClimb;
        _attrs[tokenid].level += 1;

        emit Upgrade(
            tokenid,
            _attrs[tokenid].level,
            _attrs[tokenid].runningSpeed,
            _attrs[tokenid].swimmingSpeed,
            _attrs[tokenid].flightSpeed,
            _attrs[tokenid].climbingSpeed
        );
    }

    function batchTransfer(
        address from,
        address to,
        uint256[] calldata tokenidlist
    ) public {
        for (uint256 i = 0; i < tokenidlist.length; i++) {
            uint256 tokenid = tokenidlist[i];
            transferFrom(from, to,tokenid);
        }
    }

    function tokenURI(uint256 tokenId) public view override returns (string memory) {
        _requireOwned(tokenId);
        uint256 batch = tokenId/_batchNum;
        return string(abi.encodePacked(baseURI, batch.toString(), "_", _attrs[tokenId].rarity.toString(), ".json"));
    }

    function setBaseUri(string memory uri) public onlyOwner {
        baseURI = uri;
    }

    function consume(uint256 tokenId) external {
        require(isOperator(msg.sender),"not operator");
        _attrs[tokenId].energy -= 1;
        emit Consume(msg.sender,tokenId);
    }

    function empower(uint256 tokenId, uint256 amount) external {
        require(isOperator(msg.sender), "not operator");
        _attrs[tokenId].energy += amount;
        emit Empower(msg.sender,tokenId,amount);
    }
}