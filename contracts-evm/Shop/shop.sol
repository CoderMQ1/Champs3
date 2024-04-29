// SPDX-License-Identifier: GPL-3.0
pragma solidity >=0.7.0 <0.9.0;
import "@openzeppelin/contracts/access/Ownable.sol";
import "@openzeppelin/contracts/security/ReentrancyGuard.sol";

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

struct ContractAddrSet {
    address NftRoleContract;
    address HeroTokenContract;
    address SpinContract;
    address GameAssetContract;
    address HeroBoxContract;
    address GameLogicContract;
}


abstract contract GameConfigInterface {

  function getShopSet() public virtual view returns (ShopSet memory);
  function getContractAddrSet() public virtual view returns (ContractAddrSet memory);
}


abstract contract HeroTokenInterface {
  function transferFrom(address from, address to, uint tokens)  public virtual;
}

abstract contract GameAssetInterface {
  function addAsset(address player,uint256 itemId,uint256 amount) public virtual;
  function getAsset(address player,uint256 itemId)  public virtual returns(uint256);
  function minusAsset(address,uint256 itemId,uint256 amount) public virtual;
}


contract Shop is Ownable,ReentrancyGuard {
    HeroTokenInterface public payToken;
    address public receiver;
    mapping(uint=>ShopGood) public shopGoodItems;
    mapping(uint=>bool) public goodExist;
    mapping(address => bool) internal admins;
    GameConfigInterface public gameConfg;
    GameAssetInterface public gameAsset;

    constructor() Ownable(msg.sender) {

    }

    function loadShopSet(address gameConfigContract) onlyOwner public {
        gameConfg = GameConfigInterface(gameConfigContract);
        ContractAddrSet memory contractInfos = gameConfg.getContractAddrSet();
        gameAsset = GameAssetInterface(contractInfos.GameAssetContract);
        ShopSet memory shopSet = gameConfg.getShopSet();
        payToken = HeroTokenInterface(shopSet.PayTokenContract);
        receiver = shopSet.Receiver;
        for (uint i = 0; i < shopSet.ShopGoodInfos.length; i++) {
            uint256 itemId = shopSet.ShopGoodInfos[i].GoodItemId;
            shopGoodItems[itemId] = shopSet.ShopGoodInfos[i];
            goodExist[itemId] = true;
        }
    }


    function buyGood(uint256 itemId,uint256 amount) public {
        require(goodExist[itemId] == true," good not exist");
        uint needPayAmount;
        needPayAmount = shopGoodItems[itemId].PriceAmount * amount;
        payToken.transferFrom(msg.sender, receiver, needPayAmount);
        gameAsset.addAsset(msg.sender, itemId, amount);
    }
}
