import {
  OwnershipTransferred as OwnershipTransferredEvent,
  assetChange as assetChangeEvent
} from "../generated/asset/asset"
import { OwnershipTransferred, assetTable } from "../generated/schema"
import { Address, BigDecimal, BigInt, log } from "@graphprotocol/graph-ts"
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

export function handleassetChange(event: assetChangeEvent): void {
  let index = event.params.player.concatI32(event.params.itemId.toI32())
  let entity = assetTable.load(index)
  if (entity == null){
    entity = new assetTable(index)
    entity.player = event.params.player
    entity.itemId = event.params.itemId
    entity.amount = BigInt.fromI32(0)
  } 
  if (event.params.addOrMinus == BigInt.fromI32(1)) {
    entity.amount += event.params.amount
  }else if(event.params.addOrMinus == BigInt.fromI32(2)) {
    entity.amount -= event.params.amount
  }
  entity.updateTimestamp = event.block.timestamp
  entity.save()
}
