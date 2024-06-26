//------------------------------------------------------------------------------
// This code was generated by a tool.
//
//   Tool : MetaMask Unity SDK ABI Code Generator
//   Input filename:  TokenContract.sol
//   Output filename: TokenContract.cs
//
// Changes to this file may cause incorrect behavior and will be lost when
// the code is regenerated.
// <auto-generated />
//------------------------------------------------------------------------------

#if MetamaskPlugin


using System;
using System.Numerics;
using System.Threading.Tasks;
using evm.net;
using evm.net.Models;

namespace TokenContract
{
	#if (UNITY_EDITOR || !ENABLE_MONO)
	[BackedType(typeof(TokenContractBacking))]
	#endif
	public interface TokenContract : IContract
	{
		[EvmConstructorMethod]
		Task<TokenContract> DeployNew(BigInteger _initialAmount, String _tokenName, [EvmParameterInfo(Type = "uint8", Name = "_decimalUnits")] UInt16 _decimalUnits, String _tokenSymbol, CallOptions options = default);
		
		[EvmMethodInfo(Name = "_approveInfos", View = true)]
		Task<Tuple<Boolean, BigInteger>> _approveInfos([EvmParameterInfo(Type = "address", Name = "")] EvmAddress address, CallOptions options = default);
		
		[EvmMethodInfo(Name = "allowance", View = true)]
		Task<BigInteger> Allowance(EvmAddress _owner, EvmAddress _spender, CallOptions options = default);
		
		[EvmMethodInfo(Name = "allowed", View = true)]
		Task<BigInteger> Allowed([EvmParameterInfo(Type = "address", Name = "")] EvmAddress address1, [EvmParameterInfo(Type = "address", Name = "")] EvmAddress address2, CallOptions options = default);
		
		[EvmMethodInfo(Name = "approve", View = false)]
		Task<Transaction> Approve(EvmAddress _spender, BigInteger _value, CallOptions options = default);
		
		[EvmMethodInfo(Name = "approveGame", View = false)]
		Task<Transaction> ApproveGame(Boolean en, CallOptions options = default);
		
		[EvmMethodInfo(Name = "balanceOf", View = true)]
		Task<BigInteger> BalanceOf(EvmAddress _owner, CallOptions options = default);
		
		[EvmMethodInfo(Name = "balances", View = true)]
		Task<BigInteger> Balances([EvmParameterInfo(Type = "address", Name = "")] EvmAddress address, CallOptions options = default);
		
		[EvmMethodInfo(Name = "curMineAmount", View = true)]
		Task<BigInteger> CurMineAmount();
		
		[EvmMethodInfo(Name = "decay", View = true)]
		Task<BigInteger> Decay();
		
		[EvmMethodInfo(Name = "decayDiv", View = true)]
		Task<BigInteger> DecayDiv();
		
		[EvmMethodInfo(Name = "decimals", View = true)]
		[return: EvmParameterInfo(Type = "uint8")]
		Task<UInt16> Decimals();
		
		[EvmMethodInfo(Name = "freezeDuration", View = true)]
		Task<BigInteger> FreezeDuration();
		
		[EvmMethodInfo(Name = "gameLogicContract", View = true)]
		Task<EvmAddress> GameLogicContract();
		
		[EvmMethodInfo(Name = "getCurMinerInfo", View = true)]
		Task<MineInfo> GetCurMinerInfo();
		
		[EvmMethodInfo(Name = "isAdmin", View = true)]
		Task<Boolean> IsAdmin(EvmAddress user, CallOptions options = default);
		
		[EvmMethodInfo(Name = "isApproveGameStop", View = true)]
		Task<Boolean> IsApproveGameStop();
		
		[EvmMethodInfo(Name = "mineRound", View = true)]
		Task<BigInteger> MineRound();
		
		[EvmMethodInfo(Name = "name", View = true)]
		Task<String> Name();
		
		[EvmMethodInfo(Name = "owner", View = true)]
		Task<EvmAddress> Owner();
		
		[EvmMethodInfo(Name = "releaseToken", View = false)]
		Task<Transaction> ReleaseToken();
		
		[EvmMethodInfo(Name = "renounceOwnership", View = false)]
		Task<Transaction> RenounceOwnership();
		
		[EvmMethodInfo(Name = "setAdmin", View = false)]
		Task<Transaction> SetAdmin(EvmAddress user, Boolean enabled, CallOptions options = default);
		
		[EvmMethodInfo(Name = "stopApproveGame", View = false)]
		Task<Transaction> StopApproveGame(Boolean stopApprove, CallOptions options = default);
		
		[EvmMethodInfo(Name = "symbol", View = true)]
		Task<String> Symbol();
		
		[EvmMethodInfo(Name = "totalMineAmount", View = true)]
		Task<BigInteger> TotalMineAmount();
		
		[EvmMethodInfo(Name = "totalSupply", View = true)]
		Task<BigInteger> TotalSupply();
		
		[EvmMethodInfo(Name = "transfer", View = false)]
		Task<Transaction> Transfer(EvmAddress _to, BigInteger _value, CallOptions options = default);
		
		[EvmMethodInfo(Name = "transferFrom", View = false)]
		Task<Transaction> TransferFrom(EvmAddress _from, EvmAddress _to, BigInteger _value, CallOptions options = default);
		
		[EvmMethodInfo(Name = "transferOwnership", View = false)]
		Task<Transaction> TransferOwnership(EvmAddress newOwner, CallOptions options = default);
		
		[EvmMethodInfo(Name = "updateFreezeInfo", View = false)]
		Task<Transaction> UpdateFreezeInfo(BigInteger duration, EvmAddress gameLogic, CallOptions options = default);
		
	}
}
#endif