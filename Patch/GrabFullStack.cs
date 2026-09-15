using HarmonyLib;
using UnityEngine;

namespace DropSingleItem.Patch
{
    [HarmonyPatch(typeof(InventoryGrid), "OnLeftDown")]
    class InventoryGrid_OnLeftDown_GrabFullStack
    {
        const float DoubleClickThreshold = 0.3f;

        static float LastItemClickedTime = -1f;

        public static bool Prefix(InventoryGrid __instance, UIInputHandler handler)
        {
            if (handler == null)
                return true;

            Vector2i pos = Traverse.Create(__instance).Method("GetButtonPos", new System.Type[] { typeof(GameObject) }).GetValue<Vector2i>(handler.gameObject);
            var inventory = __instance.GetInventory();
            ItemDrop.ItemData item = inventory != null ? inventory.GetItemAt(pos.x, pos.y) : null;

            bool doubleClicked = LastItemClickedTime >= 0f && (Time.unscaledTime - LastItemClickedTime) < DoubleClickThreshold;
            LastItemClickedTime = Time.unscaledTime;

            if (!doubleClicked || item == null || InventoryGui.instance == null)
                return true;

            Utils.MergeMatchingIntoSlot(InventoryGui.instance, inventory, item);
            return false;
        }
    }
}
