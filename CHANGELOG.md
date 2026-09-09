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
