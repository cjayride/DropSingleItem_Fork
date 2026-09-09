using HarmonyLib;
using UnityEngine;

namespace DropSingleItem.Patch
{
    [HarmonyPatch(typeof(InventoryGui), "Update")]
    class InventoryGui_Update_DropSingleOutside
    {
        public static void Postfix(InventoryGui __instance)
        {
            if (!InventoryGui.IsVisible() || __instance.dragGo() == null)
                return;

            if (!ZInput.GetMouseButtonDown(1))
                return;

            if (Utils.PointerOverInventoryGrid(__instance))
                return;

            Utils.DropOneOutside(__instance);
        }
    }
}
