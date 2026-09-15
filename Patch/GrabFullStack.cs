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
            if (Utils.placingOneFromDrag)
                return true;

            if (ZInput.GetMouseButton(1) || ZInput.GetMouseButtonDown(1) || ZInput.GetMouseButtonUp(1))
                return true;

            if (mod != InventoryGrid.Modifier.Select || item == null || grid == null)
                return true;

            bool doubleClicked = LastItemClickedTime >= 0f && (Time.unscaledTime - LastItemClickedTime) < DoubleClickThreshold;
            LastItemClickedTime = Time.unscaledTime;

            if (!doubleClicked)
                return true;

            Utils.MergeMatchingIntoSlot(__instance, grid.GetInventory(), item);
            return false;
        }
    }
}
