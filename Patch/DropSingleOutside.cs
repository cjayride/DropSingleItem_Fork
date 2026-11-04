using HarmonyLib;
using UnityEngine;

namespace DropSingleItem.Patch
{
    [HarmonyPatch(typeof(InventoryGui), "Update")]
    class InventoryGui_Update_DropSingleOutside
    {
        public static void Postfix(InventoryGui __instance, GameObject ___m_dragGo)
        {
            if (___m_dragGo == null)
                return;

            if (!InventoryGui.IsVisible())
                return;

            if (!ZInput.GetMouseButtonDown(1))
                return;

            if (Utils.PointerOverInventoryGrid(__instance))
                return;

            Utils.DropOneOutside(__instance);
        }
    }
}
