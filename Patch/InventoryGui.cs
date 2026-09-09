using HarmonyLib;
using UnityEngine;

namespace DropSingleItem.Patch
{
    [HarmonyPatch(typeof(InventoryGui), "OnRightClickItem")]
    class InventoryGui_OnRightClickItem
    {
        public static bool Prefix(InventoryGui __instance, ref int ___m_dragAmount, InventoryGrid grid, ItemDrop.ItemData item, Vector2i pos)
        {
            var is_dragging_item = __instance.dragGo() != null;

            var default_use_keycode = DropSingleItemMod.InventoryUseItemKeyCode == KeyCode.Mouse1;
            var shift_split = default_use_keycode && Input.GetKey(KeyCode.LeftShift);

            if (!default_use_keycode || shift_split)
            {
                if (!is_dragging_item && item != null && item.m_stack > 1)
                {
                    __instance.SetupDragItem(item, grid.GetInventory(), item.m_stack / 2);
                    return false;
                }

                if (shift_split)
                    return false;
            }

            if (!is_dragging_item)
            {
                return default_use_keycode;
            }

            var dragItem = __instance.dragItem();
            if (!Utils.CanStackItem(dragItem, item))
            {
                return false;
            }

            var dragInventory = __instance.dragInventory();
            var dragAmount = __instance.dragAmount();
            var dragStack = dragItem.m_stack;

            ___m_dragAmount = 1;
            __instance.OnSelectedItem(grid, item, pos, InventoryGrid.Modifier.Select);

            dragAmount -= dragStack - dragItem.m_stack;
            if (dragAmount > 0)
            {
                __instance.SetupDragItem(dragItem, dragInventory, dragAmount);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(InventoryGrid), "UpdateInventory")]
    class InventoryGrid_UpdateInventory
    {
        public static void Postfix(InventoryGrid __instance, Inventory inventory, Player player, ItemDrop.ItemData dragItem)
        {
            var default_use_keycode = DropSingleItemMod.InventoryUseItemKeyCode == KeyCode.Mouse1;
            if (default_use_keycode || dragItem != null)
            {
                return;
            }

            if (Input.GetKeyDown(DropSingleItemMod.InventoryUseItemKeyCode))
            {
                var selected_item = __instance.GetItem(new Vector2i(Input.mousePosition));
                if (selected_item != null && Player.m_localPlayer)
                {
                    Player.m_localPlayer.UseItem(inventory, selected_item, true);
                }
            }
        }
    }
}
