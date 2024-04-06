//------------------------------------------------------------------------------
// This code was generated by a tool.
//
//   Tool : MetaMask Unity SDK ABI Code Generator
//   Input filename:  .sol
//   Output filename: MineInfo.cs
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
	public class MineInfo
	{
		[EvmParameterInfo(Name = "CurRound", Type = "uint256")]
		public BigInteger CurRound;
		
		[EvmParameterInfo(Name = "CurMineAmount", Type = "uint256")]
		public BigInteger CurMineAmount;
		
	}
}
#endif