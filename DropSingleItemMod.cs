using BepInEx;
using BepInEx.Configuration;
using DropSingleItem.Patch;
using HarmonyLib;
using UnityEngine;

namespace DropSingleItem
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInProcess("valheim.exe")]
    public class DropSingleItemMod : BaseUnityPlugin
    {
        const string NAME = "DropSingleItem";
        const string GUID = "TechGuard.DropSingleItem";
        const string VERSION = "1.2.5";

        readonly Harmony harmony = new Harmony(GUID);
        static HarmonyMethod useItemPostfix;
        static bool useItemPatched;

        public static KeyCode InventoryUseItemKeyCode { get; private set; } = KeyCode.Mouse1;

        static ConfigEntry<string> InventoryUseItemKeyCodeConfig;

        public void Awake()
        {
            InventoryUseItemKeyCodeConfig = Config.Bind("Input", "InventoryUseItem", "Mouse1",
                "Keybinding for using/consuming/equipping an item in the inventory (Mouse1 = right mouse button).\n" +
                "If this is set to anything other than Mouse1, you no longer have to hold shift to split a stack when right clicking.\n" +
                "Available KeyCodes can be found here: https://docs.unity3d.com/ScriptReference/KeyCode.html");

            CacheUseItemKey();
            InventoryUseItemKeyCodeConfig.SettingChanged += (_, __) =>
            {
                CacheUseItemKey();
                ApplyUseItemPatch();
            };

            harmony.PatchAll();
            ApplyUseItemPatch();
            Logger.LogInfo("Loaded " + VERSION + " — Harmony patches applied.");
        }

        static void CacheUseItemKey()
        {
            if (System.Enum.TryParse(InventoryUseItemKeyCodeConfig.Value, out KeyCode keycode))
                InventoryUseItemKeyCode = keycode;
            else
                InventoryUseItemKeyCode = KeyCode.Mouse1;
        }

        void ApplyUseItemPatch()
        {
            var target = AccessTools.Method(typeof(InventoryGrid), "UpdateInventory");
            if (target == null)
                return;

            if (useItemPostfix == null)
                useItemPostfix = new HarmonyMethod(typeof(InventoryGrid_UpdateInventory), nameof(InventoryGrid_UpdateInventory.Postfix));

            bool needPatch = InventoryUseItemKeyCode != KeyCode.Mouse1;
            if (needPatch && !useItemPatched)
            {
                harmony.Patch(target, postfix: useItemPostfix);
                useItemPatched = true;
            }
            else if (!needPatch && useItemPatched)
            {
                harmony.Unpatch(target, HarmonyPatchType.Postfix, GUID);
                useItemPatched = false;
            }
        }
    }
}
