import {
  RequestFulfilled as RequestFulfilledEvent,
  RequestSent as RequestSentEvent,
  spinResult as spinResultEvent
} from "../generated/spin/spin"
import { RequestFulfilled, RequestSent, spinResult } from "../generated/schema"

export function handleRequestFulfilled(event: RequestFulfilledEvent): void {
  let entity = new RequestFulfilled(
    event.transaction.hash.concatI32(event.logIndex.toI32())
  )
  entity.requestId = event.params.requestId
  entity.randomWords = event.params.randomWords
  entity.payment = event.params.payment

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

export function handlespinResult(event: spinResultEvent): void {
  let entity = new spinResult(
    event.transaction.hash.concatI32(event.logIndex.toI32())
  )
  entity.spinId = event.params.spinId
  entity.player = event.params.player
  entity.random = event.params.random
  entity.rewardID = event.params.rewardID
  entity.rewardNum = event.params.rewardNum
  entity.bonusPool = event.params.bonusPool
  entity.jackpot = event.params.jackpot

  entity.blockNumber = event.block.number
  entity.blockTimestamp = event.block.timestamp
  entity.transactionHash = event.transaction.hash

  entity.save()
}
