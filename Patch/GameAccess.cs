using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace DropSingleItem.Patch
{
    static class GameAccess
    {
        internal static readonly AccessTools.FieldRef<InventoryGui, GameObject> DragGo =
            AccessTools.FieldRefAccess<InventoryGui, GameObject>("m_dragGo");

        internal static readonly AccessTools.FieldRef<InventoryGui, ItemDrop.ItemData> DragItem =
            AccessTools.FieldRefAccess<InventoryGui, ItemDrop.ItemData>("m_dragItem");

        internal static readonly AccessTools.FieldRef<InventoryGui, Inventory> DragInventory =
            AccessTools.FieldRefAccess<InventoryGui, Inventory>("m_dragInventory");

        internal static readonly AccessTools.FieldRef<InventoryGui, int> DragAmount =
            AccessTools.FieldRefAccess<InventoryGui, int>("m_dragAmount");

        internal static readonly AccessTools.FieldRef<InventoryGui, Container> CurrentContainer =
            AccessTools.FieldRefAccess<InventoryGui, Container>("m_currentContainer");

        internal static readonly AccessTools.FieldRef<ItemDrop, float> SpawnTime =
            AccessTools.FieldRefAccess<ItemDrop, float>("m_spawnTime");

        internal static readonly AccessTools.FieldRef<List<ItemDrop>> ItemDropInstances =
            AccessTools.StaticFieldRefAccess<List<ItemDrop>>(AccessTools.Field(typeof(ItemDrop), "s_instances"));

        internal delegate void SetupDragItemDel(InventoryGui gui, ItemDrop.ItemData item, Inventory inventory, int amount);
        internal delegate void UpdateCraftingPanelDel(InventoryGui gui, bool focusView);
        internal delegate void OnSelectedItemDel(InventoryGui gui, InventoryGrid grid, ItemDrop.ItemData item, Vector2i pos, InventoryGrid.Modifier mod);
        internal delegate Vector2i FindEmptySlotDel(Inventory inventory, bool topFirst);

        internal static readonly SetupDragItemDel SetupDragItem =
            (SetupDragItemDel)System.Delegate.CreateDelegate(typeof(SetupDragItemDel), AccessTools.Method(typeof(InventoryGui), "SetupDragItem"));

        internal static readonly UpdateCraftingPanelDel UpdateCraftingPanel =
            (UpdateCraftingPanelDel)System.Delegate.CreateDelegate(typeof(UpdateCraftingPanelDel), AccessTools.Method(typeof(InventoryGui), "UpdateCraftingPanel"));

        internal static readonly OnSelectedItemDel OnSelectedItem =
            (OnSelectedItemDel)System.Delegate.CreateDelegate(typeof(OnSelectedItemDel), AccessTools.Method(typeof(InventoryGui), "OnSelectedItem"));

        internal static readonly FindEmptySlotDel FindEmptySlot =
            (FindEmptySlotDel)System.Delegate.CreateDelegate(typeof(FindEmptySlotDel), AccessTools.Method(typeof(Inventory), "FindEmptySlot"));
    }
}
