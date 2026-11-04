using HarmonyLib;
using UnityEngine;

namespace DropSingleItem.Patch
{
    [HarmonyPatch(typeof(InventoryGui), "OnRightClickItem")]
    class InventoryGui_OnRightClickItem
    {
        public static bool Prefix(InventoryGui __instance, ref int ___m_dragAmount, InventoryGrid grid, ItemDrop.ItemData item, Vector2i pos)
        {
            var is_dragging_item = GameAccess.DragGo(__instance) != null;

            var default_use_keycode = DropSingleItemMod.InventoryUseItemKeyCode == KeyCode.Mouse1;
            var shift_split = default_use_keycode && Utils.IsShiftHeld();

            if (!default_use_keycode || shift_split)
            {
                if (!is_dragging_item && item != null && item.m_stack > 1)
                {
                    if (Utils.TrySplitStackIntoEmptySlot(grid.GetInventory(), item))
                        Utils.PlayMoveEffects(__instance);
                    return false;
                }

                if (shift_split)
                    return false;
            }

            if (!is_dragging_item)
            {
                return default_use_keycode;
            }

            var dragItem = GameAccess.DragItem(__instance);
            if (!Utils.CanStackItem(dragItem, item))
            {
                return false;
            }

            var dragInventory = GameAccess.DragInventory(__instance);
            var dragAmount = GameAccess.DragAmount(__instance);
            var dragStack = dragItem.m_stack;

            Utils.placingOneFromDrag = true;
            try
            {
                ___m_dragAmount = 1;
                GameAccess.OnSelectedItem(__instance, grid, item, pos, InventoryGrid.Modifier.Select);
            }
            finally
            {
                Utils.placingOneFromDrag = false;
            }

            dragAmount -= dragStack - dragItem.m_stack;
            if (dragAmount > 0 && dragItem != null && dragInventory != null && dragInventory.ContainsItem(dragItem))
            {
                GameAccess.SetupDragItem(__instance, dragItem, dragInventory, dragAmount);
            }
            return false;
        }
    }

    class InventoryGrid_UpdateInventory
    {
        public static void Postfix(InventoryGrid __instance, Inventory inventory, ItemDrop.ItemData dragItem)
        {
            if (dragItem != null)
                return;

            if (ZInput.GetKeyDown(DropSingleItemMod.InventoryUseItemKeyCode, true))
            {
                var selected_item = __instance.GetItem(new Vector2i(ZInput.pointerPosition));
                if (selected_item != null && Player.m_localPlayer)
                {
                    Player.m_localPlayer.UseItem(inventory, selected_item, true);
                }
            }
        }
    }
}
