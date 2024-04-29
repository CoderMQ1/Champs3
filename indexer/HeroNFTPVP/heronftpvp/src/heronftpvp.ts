import { Address, BigDecimal, BigInt, log } from "@graphprotocol/graph-ts"
import {
  heronftpvp,
  Approval,
  ApprovalForAll,
  Mint,
  FulfillAttr,
  OwnershipTransferred,
  Paused,
  Transfer,
  Unpaused,
  Upgrade,
  Consume,
  Empower,
  ApproveGame,
} from "../generated/heronftpvp/heronftpvp"
import { ExampleEntity } from "../generated/schema"
import { mintTable } from "../generated/schema"
import { ownerTable } from "../generated/schema"
import { historyTable } from "../generated/schema"
import { consumeTable } from "../generated/schema"
import {approveGameTable} from "../generated/schema"
export function handleApproval(event: Approval): void {
  // Entities can be loaded from the store using a string ID; this ID
  // needs to be unique across all entities of the same type
  //let entity = ExampleEntity.load(event.transaction.from)

  // Entities only exist after they have been saved to the store;
  // `null` checks allow to create entities on demand
  //if (!entity) {
   // entity = new ExampleEntity(event.transaction.from)

    // Entity fields can be set using simple assignments
   // entity.count = BigInt.fromI32(0)
  //}

  // BigInt and BigDecimal math are supported
  //entity.count = entity.count + BigInt.fromI32(1)

  // Entity fields can be set based on event parameters
  //entity.owner = event.params.owner
  //entity.approved = event.params.approved

  // Entities can be written to the store with `.save()`
  //entity.save()

  // Note: If a handler doesn't require existing field values, it is faster
  // _not_ to load the entity from the store. Instead, create it fresh with
  // `new Entity(...)`, set the fields that should be updated and save the
  // entity back to the store. Fields that were not set or unset remain
  // unchanged, allowing for partial updates to be applied.

  // It is also possible to access smart contracts from mappings. For
  // example, the contract that has emitted the event can be connected to
  // with:
  //
  // let contract = Contract.bind(event.address)
  //
  // The following functions can then be called on this contract to access
  // state variables and other data:
  //
  // - contract._attrs(...)
  // - contract.balanceOf(...)
  // - contract.getApproved(...)
  // - contract.getAttrByTokenId(...)
  // - contract.getCurrentSupply(...)
  // - contract.getRarity(...)
  // - contract.getTotalSupply(...)
  // - contract.isApprovedForAll(...)
  // - contract.isMinter(...)
  // - contract.name(...)
  // - contract.owner(...)
  // - contract.ownerOf(...)
  // - contract.paused(...)
  // - contract.supportsInterface(...)
  // - contract.symbol(...)
  // - contract.tokenURI(...)
}

export function handleApprovalForAll(event: ApprovalForAll): void {}

export function handleMint(event: Mint): void {
        //mint event
    let mint_entity = new mintTable(event.params.tokenid.toHex())
    mint_entity.tokenid = event.params.tokenid
    mint_entity.rarity = BigInt.fromI32(0) //event.params.rarity
    mint_entity.talent = BigInt.fromI32(0) //event.params.talent
    mint_entity.level = BigInt.fromI32(0) //event.params.level
    mint_entity.RunningSpeed = BigInt.fromI32(0) //event.params.RunningSpeed
    mint_entity.SwimmingSpeed = BigInt.fromI32(0) //event.params.SwimmingSpeed
    mint_entity.ClimbingSpeed = BigInt.fromI32(0) //event.params.ClimbingSpeed
    mint_entity.FlightSpeed = BigInt.fromI32(0) //event.params.FlightSpeed
    mint_entity.energy = BigInt.fromI32(0) //event.params.energy
        mint_entity.time = event.block.timestamp
        mint_entity.owner = event.params.to
        mint_entity.contract = event.address
        mint_entity.save()

        let owner_entity = new ownerTable(event.params.tokenid.toHex())
        owner_entity.tokenid = event.params.tokenid
        owner_entity.owner = event.params.to
        owner_entity.contract = event.address
	owner_entity.rarity = BigInt.fromI32(0) //event.params.rarity
	owner_entity.level = BigInt.fromI32(0) //event.params.level
	owner_entity.talent = BigInt.fromI32(0) //event.params.talent
	owner_entity.RunningSpeed = BigInt.fromI32(0) //event.params.RunningSpeed
	owner_entity.SwimmingSpeed = BigInt.fromI32(0) //event.params.SwimmingSpeed
	owner_entity.ClimbingSpeed = BigInt.fromI32(0) //event.params.ClimbingSpeed
	owner_entity.FlightSpeed = BigInt.fromI32(0) //event.params.FlightSpeed
	owner_entity.energy = BigInt.fromI32(0) //event.params.energy
	owner_entity.upgradeBlockNumber = BigInt.fromI32(0)
        owner_entity.save()

        let history_entity = new historyTable(event.params.tokenid.toHex())
        history_entity.tokenid = event.params.tokenid
        history_entity.contract = event.address
        history_entity.from = Address.fromString('0x0000000000000000000000000000000000000000')
        history_entity.to = event.params.to
        history_entity.transfercount = BigInt.fromI32(0)
        history_entity.time = event.block.timestamp
        history_entity.save()
}

export function handleFulfillAttr(event: FulfillAttr): void {
  let owner_entity = ownerTable.load(event.params.tokenid.toHex())
  if (owner_entity) {
     owner_entity.rarity = event.params.attr.rarity
     owner_entity.level = event.params.attr.level
     owner_entity.talent = event.params.attr.talent
     owner_entity.RunningSpeed = event.params.attr.runningSpeed
     owner_entity.SwimmingSpeed = event.params.attr.swimmingSpeed
     owner_entity.ClimbingSpeed = event.params.attr.climbingSpeed
     owner_entity.FlightSpeed = event.params.attr.flightSpeed
     owner_entity.energy = event.params.attr.energy
     owner_entity.save()
  }
  

  let mint_entity = mintTable.load(event.params.tokenid.toHex())
  if (mint_entity) {
     mint_entity.rarity = event.params.attr.rarity
     mint_entity.level = event.params.attr.level
     mint_entity.talent = event.params.attr.talent
     mint_entity.RunningSpeed = event.params.attr.runningSpeed
     mint_entity.SwimmingSpeed = event.params.attr.swimmingSpeed
     mint_entity.ClimbingSpeed = event.params.attr.climbingSpeed
     mint_entity.FlightSpeed = event.params.attr.flightSpeed
     mint_entity.energy = event.params.attr.energy
     mint_entity.save()
  }
}

export function handleOwnershipTransferred(event: OwnershipTransferred): void {}

export function handlePaused(event: Paused): void {}

//export function handleTransfer(event: Transfer): void {}
export function handleTransfer(event: Transfer): void {
  let underlyingAddress = Address.fromString('0x0000000000000000000000000000000000000000',)
  if(event.params.to.toHex() == underlyingAddress.toHexString()){
        //burn todo...
  }else{
        //transfer event
        //modify owner of ownerTable
        let owner = ownerTable.load(event.params.tokenId.toHex())
        if (owner) {
                owner.owner = event.params.to
                //owner.updatetime = BigInt.fromI64(Date.now())
                owner.save()
        }
        //modify historyTable
        let history = historyTable.load(event.params.tokenId.toHex())
        if(history) {
                history.from = event.params.from
                history.to = event.params.to
                history.transfercount = history.transfercount + BigInt.fromI32(1)
                history.time = event.block.timestamp
                //history.updatetime = BigInt.fromI64(Date.now())
                history.save()
                //let index = historyTable.indexOf(event.params.tokenId.toHex())
                //historyTable.splice(index,1)
        }
  }
}

export function handleUnpaused(event: Unpaused): void {}

export function handleUpgrade(event: Upgrade): void {
        let owner_entity = ownerTable.load(event.params.tokenid.toHex())
	if(owner_entity) {
		owner_entity.level = event.params.level
                owner_entity.RunningSpeed = event.params.runningSpeed
                owner_entity.SwimmingSpeed = event.params.swimmingSpeed
                owner_entity.ClimbingSpeed = event.params.climbingSpeed
		owner_entity.FlightSpeed = event.params.flightSpeed
		owner_entity.upgradeBlockNumber = event.block.number
                owner_entity.save()
        }
}

export function handleConsume(event: Consume): void {
	let owner_entity = ownerTable.load(event.params.tokenId.toHex())
        if(owner_entity) {
                owner_entity.energy -= BigInt.fromI32(1)
                owner_entity.save()
        }
}

export function handleEmpower(event: Empower): void {
        let owner_entity = ownerTable.load(event.params.tokenId.toHex())
        if(owner_entity) {
                owner_entity.energy += event.params.amount
                owner_entity.save()
        }
}

export function handleApproveGame(event: ApproveGame): void {
	let approve_entity = approveGameTable.load(event.params.playerAddr)
	if (!approve_entity){
		approve_entity = new approveGameTable(event.params.playerAddr)
		approve_entity.playerAddr = event.params.playerAddr
	}
	approve_entity.en = event.params.en
	approve_entity.lastApproveTime = event.params.lastApproveTime
	approve_entity.blockNumber = event.block.number
	approve_entity.blockTimestamp = event.block.timestamp
	approve_entity.transactionHash = event.transaction.hash
	approve_entity.save()
}
