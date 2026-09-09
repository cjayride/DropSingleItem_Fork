using HarmonyLib;
using UnityEngine;

namespace DropSingleItem.Patch
{
    [HarmonyPatch(typeof(InventoryGui), "OnSelectedItem")]
    class InventoryGui_OnSelectedItem_GrabFullStack
    {
        const float DoubleClickThreshold = 0.3f;

        static float LastItemClickedTime = -1f;

        public static bool Prefix(InventoryGui __instance, InventoryGrid grid, ItemDrop.ItemData item, Vector2i pos, InventoryGrid.Modifier mod)
        {
            if (mod != InventoryGrid.Modifier.Select)
                return true;

            bool doubleClicked = LastItemClickedTime >= 0f && (Time.unscaledTime - LastItemClickedTime) < DoubleClickThreshold;
            LastItemClickedTime = Time.unscaledTime;

            if (!doubleClicked)
                return true;

            ItemDrop.ItemData target = item;
            Inventory targetInventory = grid != null ? grid.GetInventory() : null;

            if (target == null && __instance.dragItem() != null)
            {
                target = __instance.dragItem();
                targetInventory = __instance.dragInventory();
            }

            if (target == null || targetInventory == null)
                return true;

            Utils.MergeMatchingIntoSlot(__instance, targetInventory, target);
            return false;
        }
    }
}
