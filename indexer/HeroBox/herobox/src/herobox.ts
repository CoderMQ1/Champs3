import { Address, BigDecimal, BigInt, log } from "@graphprotocol/graph-ts"
import {
  InviteReward as InviteRewardEvent,
  OwnershipTransferred as OwnershipTransferredEvent,
  Paused as PausedEvent,
  RequestFulfilled as RequestFulfilledEvent,
  RequestSent as RequestSentEvent,
  Unpaused as UnpausedEvent
} from "../generated/herobox/herobox"
import {
  InviteRewardStatic,
  InviteReward,
  OwnershipTransferred,
  Paused,
  RequestFulfilled,
  RequestSent,
  Unpaused
} from "../generated/schema"

export function handleInviteReward(event: InviteRewardEvent): void {
  let underlyingAddress = Address.fromString('0x0000000000000000000000000000000000000000')
  if (underlyingAddress.toHexString() != event.params.inviter.toHex()) {
    let entity = InviteRewardStatic.load(event.params.inviter)
    if (!entity) {
     entity = new InviteRewardStatic(event.params.inviter)
     entity.totalRewardNum = BigInt.fromI32(0)
   }

   entity.inviter = event.params.inviter
   entity.totalRewardNum += event.params.rewardNum

   entity.blockNumber = event.block.number
   entity.blockTimestamp = event.block.timestamp
   entity.transactionHash = event.transaction.hash

   entity.save()
 }

  let inviteRecord = new InviteReward(event.transaction.hash.concatI32(event.logIndex.toI32()))

  inviteRecord.inviter = event.params.inviter
  inviteRecord.player = event.params.player
  inviteRecord.rewardNum = event.params.rewardNum

  inviteRecord.blockNumber = event.block.number
  inviteRecord.blockTimestamp = event.block.timestamp
  inviteRecord.transactionHash = event.transaction.hash

  inviteRecord.save()
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

export function handlePaused(event: PausedEvent): void {
  let entity = new Paused(
    event.transaction.hash.concatI32(event.logIndex.toI32())
  )
  entity.account = event.params.account

  entity.blockNumber = event.block.number
  entity.blockTimestamp = event.block.timestamp
  entity.transactionHash = event.transaction.hash

  entity.save()
}

export function handleRequestFulfilled(event: RequestFulfilledEvent): void {
  let entity = new RequestFulfilled(
    event.transaction.hash.concatI32(event.logIndex.toI32())
  )
  entity.requestId = event.params.requestId
  entity.randomWords = event.params.randomWords

  entity.blockNumber = event.block.number
  entity.blockTimestamp = event.block.timestamp
  entity.transactionHash = event.transaction.hash

  entity.save()
}

export function handleRequestSent(event: RequestSentEvent): void {
  let entity = new RequestSent(
    event.transaction.hash.concatI32(event.logIndex.toI32())
  )
  entity.requestId = event.params.requestId
  entity.numWords = event.params.numWords

  entity.blockNumber = event.block.number
  entity.blockTimestamp = event.block.timestamp
  entity.transactionHash = event.transaction.hash

  entity.save()
}

export function handleUnpaused(event: UnpausedEvent): void {
  let entity = new Unpaused(
    event.transaction.hash.concatI32(event.logIndex.toI32())
  )
  entity.account = event.params.account

  entity.blockNumber = event.block.number
  entity.blockTimestamp = event.block.timestamp
  entity.transactionHash = event.transaction.hash

  entity.save()
}
