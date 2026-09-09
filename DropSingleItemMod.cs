using BepInEx;
using BepInEx.Configuration;
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
        const string VERSION = "1.2.1";

        readonly Harmony harmony = new Harmony(GUID);

        public static KeyCode InventoryUseItemKeyCode
        {
            get
            {
                if (System.Enum.TryParse(InventoryUseItemKeyCodeConfig.Value, out KeyCode keycode))
                {
                    return keycode;
                }
                return KeyCode.Mouse1;
            }
        }
        static ConfigEntry<string> InventoryUseItemKeyCodeConfig;

        public void Awake()
        {
            InventoryUseItemKeyCodeConfig = Config.Bind("Input", "InventoryUseItem", "Mouse1",
                "Keybinding for using/consuming/equipping an item in the inventory (Mouse1 = right mouse button).\n" +
                "If this is set to anything other than Mouse1, you no longer have to hold shift to split a stack when right clicking.\n" +
                "Available KeyCodes can be found here: https://docs.unity3d.com/ScriptReference/KeyCode.html");

            harmony.PatchAll();
        }
    }
}
