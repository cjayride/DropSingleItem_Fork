# v1.2.5
- Verified patch targets against current Valheim (Unity 6000.0.75, assembly_valheim from 2026-10-28)
- Removed per-frame Harmony Traverse lookups; private game fields/methods are cached with AccessTools FieldRef/MethodDelegate
- Default Mouse1 no longer patches InventoryGrid.UpdateInventory every inventory refresh
- Inventory Update postfix returns immediately when nothing is dragged
- Cached the use-item KeyCode instead of parsing the config string on every access
- Reused a merge buffer and dropped LINQ on stack merge

# v1.2.4
- Verified against Valheim 1.0.16
- Right-click drop-one no longer launches items: extra clicks add to the last pile instead of spawning overlapping rigidbodies, and the toss is capped

# v1.2.3
- Restored place-one / drop-one (1.2.2 merge hook on InventoryGrid.OnLeftDown could prevent the plugin from patching)

# v1.2.2
- Fixed right-click place-one sometimes sucking placed items back onto the held stack (double-click merge was firing on rapid right clicks)

# v1.2.1
- Verified in-game on Valheim 1.0
- Shift + right click splits a stack in its slot (no pickup); uses MoveItemToThis so it is not flagged as a cheated item
- Double left click merges matching stacks into the clicked slot and does not pick them up
- Place one / drop one keep the remaining stack on the cursor

# v1.2.0
- Updated for Valheim 1.0 (Unity 6)
- Build now references Unity assemblies from valheim_Data/Managed
- BepInEx dependency updated to denikson-BepInExPack_Valheim-5.4.2350
- Keep the dragged stack on the cursor while right-clicking (ZInput.GetMouseButton in UpdateItemDrag)
- Place 1 item from a held stack with right click; remaining items stay on the cursor
- Right click outside the inventory grids drops 1 item from the held stack

# v1.1.1
- Updated to work with Mistlands

# v1.1.0
- Added config entry to override inventory "use" input (defaults to right-click)
- Shift + Right-click will pick up half of the stack
- Double Left-click will grab a full stack of the selected item
