// SPDX-License-Identifier: UNLICENSED
pragma solidity ^0.8.0;


abstract contract EIP20Interface {
    /* This is a slight change to the ERC20 base standard.
    function totalSupply() constant returns (uint256 supply);
    is replaced with:
    uint256 public totalSupply;
    This automatically creates a getter function for the totalSupply.
    This is moved to the base contract since public getter functions are not
    currently recognised as an implementation of the matching abstract
    function by the compiler.
    */
    /// total amount of tokens
    uint256 public totalSupply;

    function balanceOf(address _owner) public virtual view returns (uint256 balance);


    function transfer(address _to, uint256 _value) public virtual returns (bool success);


    function transferFrom(address _from, address _to, uint256 _value) public virtual returns (bool success);

    function approve(address _spender, uint256 _value) public virtual returns (bool success);


    function allowance(address _owner, address _spender) public virtual view returns (uint256 remaining);

    // solhint-disable-next-line no-simple-event-func-name
    event Transfer(address indexed _from, address indexed _to, uint256 _value);
    event Approval(address indexed _owner, address indexed _spender, uint256 _value);
    event Log(address indexed msgsender, uint256 _value);
}
