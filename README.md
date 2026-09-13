# Module 3: Digital Economy 

An item, currency, and trading system wired directly into the NPC behavior from Module 2: an NPC decides on its own to buy food when hungry, using the same trading mechanism as the player's possessed body

![NPC Trade demo](NPCTrade.gif)
![Player Trade demo](PlayerTrade.gif)

## What it demonstrates

- New items (Gold, Apple) are created in the editor as ScriptableObjects
- An NPC notices it is hungry, walks to the trader, buys food, and eats it, but only when it has no food in stock and enough coins
- The player's possessed body walks up to the trader and buys an item by pressing E
- The trader is a solid physical obstacle; the NPC paths around it on the NavMesh instead of walking through or pushing it

## Features

- **ScriptableObject items (`ItemData`)**: an item template (name, price, weight, icon) is created as an asset from the editor menu
- **`Wallet` and `Inventory`**: the wallet and inventory live on any body
- **`Trader.TryBuy(Wallet, Inventory)` with no hardcoded buyer**: the method takes the buyer's wallet and inventory as parameters, so the same code serves both the player and any NPC
- **"Buy, then eat" model**: buying places the item in `Inventory`, eating separately consumes it from there, splitting resource acquisition from resource consumption, instead of the earlier direct "reach the food, hunger drops" mechanic
- **Separate colliders on the trader**: a physical collider blocks movement, a larger trigger collider defines the interaction zone, giving a comfortable buying range without clipping into the model
- **`NavMeshModifier` (Not Walkable) on the trader**: excludes the trader's footprint from the baked NavMesh, so the NPC routes around it instead of through it

## Tech stack

- Unity 6.6 (Universal Render Pipeline)
- C#
- Input System (reused from Module 1: `Module1.inputactions`, `CharacterControls` action map, new `Interact` action)
- AI Navigation package (`NavMeshSurface`, `NavMeshModifier`)
- ScriptableObjects

## Project structure

```
Assets/
  Scripts/
    ItemData.cs                  ScriptableObject item template
    Wallet.cs                    coin storage, AddCoins/TrySpend
    Inventory.cs                 item storage, AddItem/RemoveItem/HasItem
    Trader.cs                    trade logic, TryBuy(Wallet, Inventory)
    PlayerTraderInteractor.cs    possessed-body buying via trigger and E key
    Npcbehaviour.cs              NPC FSM, extended with MovingToTrader and Buying states
    Npcneeds.cs                  unchanged, carried over from Module 2
    PossessionManager.cs         unchanged, carried over from Module 1
    FirstPersonController.cs     unchanged, carried over from Module 1
    NPCIdleController.cs         unchanged, carried over from Module 1

```

## How to run

1. Clone the repository
2. Open the project in Unity 6.6 (or later) with Universal Render Pipeline
3. Open `Assets/Scenes/Module3.unity`
4. Enter Play mode
5. Either wait for the NPC to get hungry and walk itself to the trader, or possess the body and walk up to the trader and press E to buy an item directly

## Known limitations (by design, for this module's scope)

- An NPC with no money and no food is not handled: it simply stays in Idle/Wandering while hunger keeps rising
- The trader's stock is unlimited, items never run out
- NPC income is not implemented: `Wallet.AddCoins()` is ready to be called by a future earning system, but that system does not exist yet


## Roadmap

- [ ] NPC earning system (labor, selling their own items)
- [ ] Limited trader stock
- [ ] Handling the "no money and no food" case (for example, the NPC seeking another income source)
