import {
  GameSettleLog as GameSettleLogEvent,
  MatchProgress as MatchProgressEvent,
  MatchSuccess as MatchSuccessEvent,
  OwnershipTransferred as OwnershipTransferredEvent,
  PvpFeeUpdate as PvpFeeUpdateEvent,
  StartPvpGameLog as StartPvpGameLogEvent
} from "../generated/gamelogic/gamelogic"
import {
  GameSettleLog,
  MatchProgress,
  MatchSuccess,
  OwnershipTransferred,
  PvpFeeUpdate,
  StartPvpGameLog
} from "../generated/schema"

export function handleGameSettleLog(event: GameSettleLogEvent): void {
  let entity = new GameSettleLog(
    event.transaction.hash.concatI32(event.logIndex.toI32())
  )
  entity.roomId = event.params.roomId
  entity.player = event.params.player
  entity.rank = event.params.rank
  entity.itemId = event.params.itemId
  entity.amount = event.params.amount

  entity.blockNumber = event.block.number
  entity.blockTimestamp = event.block.timestamp
  entity.transactionHash = event.transaction.hash

  entity.save()
}

export function handleMatchProgress(event: MatchProgressEvent): void {
  let entity = new MatchProgress(
    event.transaction.hash.concatI32(event.logIndex.toI32())
  )
  entity.roomId = event.params.roomId
  entity.player = event.params.player
  entity.tokenId = event.params.tokenId
  entity.targetNum = event.params.targetNum

  entity.blockNumber = event.block.number
  entity.blockTimestamp = event.block.timestamp
  entity.transactionHash = event.transaction.hash

  entity.save()
}

export function handleMatchSuccess(event: MatchSuccessEvent): void {
  let entity = new MatchSuccess(
    event.transaction.hash.concatI32(event.logIndex.toI32())
  )
  entity.roomId = event.params.roomId

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

export function handlePvpFeeUpdate(event: PvpFeeUpdateEvent): void {
  let entity = new PvpFeeUpdate(
    event.transaction.hash.concatI32(event.logIndex.toI32())
  )
  entity.pvpFee = event.params.pvpFee
  entity.updateTime = event.params.updateTime

  entity.blockNumber = event.block.number
  entity.blockTimestamp = event.block.timestamp
  entity.transactionHash = event.transaction.hash

  entity.save()
}

export function handleStartPvpGameLog(event: StartPvpGameLogEvent): void {
  let entity = new StartPvpGameLog(
    event.transaction.hash.concatI32(event.logIndex.toI32())
  )
  entity.player = event.params.player

  entity.blockNumber = event.block.number
  entity.blockTimestamp = event.block.timestamp
  entity.transactionHash = event.transaction.hash

  entity.save()
}
