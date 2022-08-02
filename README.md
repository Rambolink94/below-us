# Below Us
A cellular automata experiement

In this prototype the player is able to explore an endless procedurally generated world.
The world is generated with cellular automata and the material rendered with a triplanar shader.

Alongside world exploration, the player can also deform the terrain by both placing and removing walls.
The player is also able to break objects marked as breakable and each breakable has an associated loot table with various items.
The player has an inventory, equipment panel, stats panel, and crafting panel. Players can pick up items from the world and from containers.

Eventually I will need to move to deferred rendering to allow more lights to be placed. 

### Controls ###
* LMB: Mine / Break
* LMB + L-Shift: Place torch
* RMB: Equip / Move from Container (UI)
* I: Inventory
* C: Crafting
* E: Interact / Pickup

### Known Bugs ###
* If inventory isn't opened before you start picking things up, they won't get picked up.
* If you start to move at the start when the map is still loading, sometimes it breaks the map gen.
* Walls on edges of chunks don't break properly.
* Spider enemy forces us on top of him.

This project at least temperarily is no longer being worked on, but I'd like to continue it in the future or at least create a similar project.
![image](https://user-images.githubusercontent.com/31221007/182488574-69c9295c-977b-4d60-ac1c-7571d1702cab.png)
![image](https://user-images.githubusercontent.com/31221007/182488949-28d8d63b-bd5a-44ee-acfc-eba166354a65.png)
![image](https://user-images.githubusercontent.com/31221007/182489007-e7870172-3632-469b-8881-c631912e1459.png)
![below-us-mining-v2](https://user-images.githubusercontent.com/31221007/182493733-d2c41c58-6345-44df-854f-3411cbd6611a.gif)