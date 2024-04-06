// SPDX-License-Identifier: UNLICENSED
pragma solidity ^0.8.0;

import "./EIP20Interface.sol";
import "@openzeppelin/contracts/access/Ownable.sol";

struct MineInfo {
    uint256 CurRound;
    uint256 CurMineAmount;
}

contract EIP20 is Ownable,EIP20Interface {

    uint256 constant private MAX_UINT256 = 2**256 - 1;
    mapping (address => uint256) public balances;
    mapping (address => mapping (address => uint256)) public allowed;
    mapping(address => bool) internal _admins;


    /*
    NOTE:
    The following variables are OPTIONAL vanities. One does not have to include them.
    They allow one to customise the token contract & in no way influences the core functionality.
    Some wallets/interfaces might not even bother to look at this information.
    */
    string public name;                   //fancy name: eg Simon Bucks
    uint8 public decimals;                //How many decimals to show.
    string public symbol;                 //An identifier: eg SBX
    //uint256 public mineBalance;
    //uint256 public nextMineAmount;
    uint256 public mineRound;
    uint256 public decay;
    uint256 public decayDiv;
    uint256 private firstMineAmount;
    uint256 public curMineAmount;
    uint256 public totalMineAmount;

    constructor(
        uint256 _initialAmount,
        string memory _tokenName,
        uint8 _decimalUnits,
        string memory _tokenSymbol
    )Ownable(msg.sender) {
        balances[msg.sender] = _initialAmount;               // Give the creator all initial tokens
        totalSupply = _initialAmount;                        // Update total supply
        decay = 999999669929969;
        decayDiv = 1000000000000000;
        firstMineAmount = _initialAmount*(decayDiv-decay)/decayDiv;
        totalMineAmount = 0;
        curMineAmount = firstMineAmount;
        mineRound = 0;
        name = _tokenName;                                   // Set the name for display purposes
        decimals = _decimalUnits;                            // Amount of decimals for display purposes
        symbol = _tokenSymbol;                               // Set the symbol for display purposes
    }



    function setAdmin(address user, bool enabled) onlyOwner public {
        _admins[user] = enabled;
    }

    function isAdmin(address user) public view returns (bool) {
        return _admins[user];
    }

    function transfer(address _to, uint256 _value) public override returns (bool success) {
        require(balances[msg.sender] >= _value);
        balances[msg.sender] -= _value;
        balances[_to] += _value;
        emit Transfer(msg.sender, _to, _value); //solhint-disable-line indent, no-unused-vars
        return true;
    }

    function transferFrom(address _from, address _to, uint256 _value) public override returns (bool success) {
        uint256 tmp_allowance = allowed[_from][msg.sender];
        require(balances[_from] >= _value && tmp_allowance >= _value);
        balances[_to] += _value;
        balances[_from] -= _value;
        if (tmp_allowance < MAX_UINT256) {
            allowed[_from][msg.sender] -= _value;
        }
        emit Transfer(_from, _to, _value); //solhint-disable-line indent, no-unused-vars
        return true;
    }

    function balanceOf(address _owner) public override view returns (uint256 balance) {
        return balances[_owner];
    }

    function approve(address _spender, uint256 _value) public override returns (bool success) {
        allowed[msg.sender][_spender] = _value;
        emit Approval(msg.sender, _spender, _value); //solhint-disable-line indent, no-unused-vars
        return true;
    }

    function allowance(address _owner, address _spender) public override view returns (uint256 remaining) {
        return allowed[_owner][_spender];
    }

    function releaseToken() public {
        require(
            _admins[msg.sender],
            "Only a admin can operator"
        );
        mineRound++;
        if(mineRound == 1) {
            curMineAmount = firstMineAmount;
        }else{
            curMineAmount = curMineAmount*decay/decayDiv;
        }

        totalMineAmount += curMineAmount;
    }

    function getCurMinerInfo() public view returns (MineInfo memory) {
        MineInfo memory mintInfo;
        mintInfo.CurRound = mineRound;
        mintInfo.CurMineAmount = curMineAmount;
        return mintInfo;
    }
}
