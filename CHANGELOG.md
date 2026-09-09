# v1.2.0
- Updated for Valheim 1.0 (Unity 6)
- Build now references Unity assemblies from valheim_Data/Managed
- BepInEx dependency updated to denikson-BepInExPack_Valheim-5.4.2350
- Fixed EffectList.Create signature (restores drop-one-outside and grab-full-stack)
- Updated InventoryGrid.UpdateInventory to the 1.0 (Inventory, Player, ItemData) signature
- Added null checks around drag-item drop and stack merge

# v1.1.1
- Updated to work with Mistlands

# v1.1.0
- Added config entry to override inventory "use" input (defaults to right-click)
- Shift + Right-click will pick up half of the stack
- Double Left-click will grab a full stack of the selected item
