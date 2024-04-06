// SPDX-License-Identifier: UNLICENSED
pragma solidity >=0.7.0 <0.9.0;
import "@openzeppelin/contracts/token/ERC721/extensions/ERC721Pausable.sol";
import "@openzeppelin/contracts/access/Ownable.sol";

struct UserAsset {
    uint256 ItemId;
    uint256 Amount;
}
contract Asset is Ownable{

    mapping(address=>mapping(uint256=>uint256)) public _assets;
    mapping(address => bool) internal _admins;
    uint256[] internal _assetIds;
    constructor() Ownable(msg.sender){
    }

    event assetChange(address player,uint256 itemId,uint256 amount,uint256 addOrMinus);

    function setAdmin(address user, bool enabled) onlyOwner public {
        _admins[user] = enabled;
    }

    function isAdmin(address user) public view returns (bool) {
        return _admins[user];
    }

    function setAssetIds(uint256[] memory assetIds) public {
         require(
            _admins[msg.sender],
            "Only a admin can operator"
        );
        _assetIds = assetIds;
    }

    function getAssetIds()public view returns(uint256[] memory) {
        return _assetIds;
    }

    function addAsset(address player,uint256 itemId,uint256 amount) public {
        //require(!player.isContract(), "The  Address should not be a contract");
        require(
            _admins[msg.sender],
            "Only a admin can operator"
        );
        _assets[player][itemId] += amount;
        emit assetChange(player,itemId,amount,1);
    }

    function minusAsset(address player,uint256 itemId,uint256 amount) external {
        //require(!player.isContract(), "The  Address should not be a contract");
        require(
            _admins[msg.sender],
            "Only a admin can operator"
        );
        require(_assets[player][itemId]>=amount,"balance is not enough");
        _assets[player][itemId] -= amount;
        emit assetChange(player,itemId,amount,2);
    }

    function getAsset(address player,uint256 itemId) external view returns(uint256){
        return _assets[player][itemId];
    }

    function getAssets(address player) external view returns(UserAsset[] memory ){
        UserAsset[] memory assets = new UserAsset[](_assetIds.length);
        for (uint i = 0; i < _assetIds.length; i++) {
            uint256 itemId = _assetIds[i];
            assets[i] = UserAsset(itemId,_assets[player][itemId]);
        }
        return assets;
    }
}
