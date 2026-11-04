using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace DropSingleItem.Patch
{
    [HarmonyPatch(typeof(InventoryGui), "UpdateItemDrag")]
    class InventoryGui_UpdateItemDrag
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var GetMouseButtonMethod = typeof(ZInput).GetMethod("GetMouseButton", new System.Type[] { typeof(int) });

            var codes = new List<CodeInstruction>(instructions);
            for (var i = 1; i < codes.Count; ++i)
            {
                if (codes[i - 1].opcode == OpCodes.Ldc_I4_1 && codes[i].Calls(GetMouseButtonMethod))
                {
                    codes[i - 1] = new CodeInstruction(OpCodes.Nop);
                    codes[i] = new CodeInstruction(OpCodes.Ldc_I4_S, 0);
                }
            }
            return codes;
        }
    }
}
