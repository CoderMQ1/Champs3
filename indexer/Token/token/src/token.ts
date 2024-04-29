import { Address, BigDecimal, BigInt, log } from "@graphprotocol/graph-ts"
import {
  Approval as ApprovalEvent,
  ApproveGame as ApproveGameEvent,
  Log as LogEvent,
  OwnershipTransferred as OwnershipTransferredEvent,
  Transfer as TransferEvent
  } from "../generated/token/token"
import {
  Approval,
  ApproveGame,
  Log,
  OwnershipTransferred,
  Transfer,
  OwnerTable
} from "../generated/schema"

export function handleApproval(event: ApprovalEvent): void {
  let entity = new Approval(
    event.transaction.hash.concatI32(event.logIndex.toI32())
  )
  entity._owner = event.params._owner
  entity._spender = event.params._spender
  entity._value = event.params._value

  entity.blockNumber = event.block.number
  entity.blockTimestamp = event.block.timestamp
  entity.transactionHash = event.transaction.hash

  entity.save()
}

export function handleApproveGame(event: ApproveGameEvent): void {
  let entity = ApproveGame.load(event.params.playerAddr)
  if(!entity){
    entity = new ApproveGame(event.params.playerAddr)
    entity.playerAddr = event.params.playerAddr
  }
  
  entity.en = event.params.en
  entity.gameLogic = event.params.gameLogic
  entity.lastApproveTime = event.params.lastApproveTime

  entity.blockNumber = event.block.number
  entity.blockTimestamp = event.block.timestamp
  entity.transactionHash = event.transaction.hash

  entity.save()
}

export function handleLog(event: LogEvent): void {
  let entity = new Log(event.transaction.hash.concatI32(event.logIndex.toI32()))
  entity.msgsender = event.params.msgsender
  entity._value = event.params._value

  entity.blockNumber = event.block.number
  entity.blockTimestamp = event.block.timestamp
  entity.transactionHash = event.transaction.hash

  entity.save()
}

export function handleOwnershipTransferred(
  event: OwnershipTransferredEvent
): void {
  let entity = new OwnershipTransferred(
    event.transaction.hash.concatI32(event.logIndex.toI32())
  )
  entity.previousOwner = event.params.previousOwner
  entity.newOwner = event.params.newOwner

  entity.blockNumber = event.block.number
  entity.blockTimestamp = event.block.timestamp
  entity.transactionHash = event.transaction.hash

  entity.save()
}

export function handleTransfer(event: TransferEvent): void {
let underlyingAddress = Address.fromString('0x0000000000000000000000000000000000000000')
if (event.params._from.toHex() != underlyingAddress.toHexString()){
    let fromOwner = OwnerTable.load(event.params._from)
    if (!fromOwner){
      fromOwner = new OwnerTable(event.params._from)
      fromOwner.balance = BigInt.fromI32(0)
    }
    fromOwner.owner = event.params._from
    if(fromOwner.balance >= event.params._value){
     fromOwner.balance -= event.params._value
    }
    fromOwner.blockNumber = event.block.number
    fromOwner.blockTimestamp = event.block.timestamp
    fromOwner.transactionHash = event.transaction.hash
    fromOwner.save()
  }
  //return
if (event.params._to.toHex() != underlyingAddress.toHexString()){
    let toOwner = OwnerTable.load(event.params._to)
    if (!toOwner){
       toOwner = new OwnerTable(event.params._to)
       toOwner.balance = BigInt.fromI32(0)
    }
    toOwner.owner = event.params._to
    toOwner.balance += event.params._value
    toOwner.blockNumber = event.block.number
    toOwner.blockTimestamp = event.block.timestamp
    toOwner.transactionHash = event.transaction.hash
    toOwner.save()
    }
}
