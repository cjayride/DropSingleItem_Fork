using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace DropSingleItem.Patch
{
    [HarmonyPatch(typeof(InventoryGrid), "OnLeftClick")]
    class InventoryGrid_OnLeftClick
    {
        const float DoubleClickThreshold = 0.2f;

        static float LastItemClickedTime = 0f;

        public struct State
        {
            public bool DoubleClicked;
        }

        public static bool Prefix(ref State __state)
        {
            __state.DoubleClicked = (Time.unscaledTime - LastItemClickedTime) < DoubleClickThreshold;
            LastItemClickedTime = Time.unscaledTime;

            var __instance = InventoryGui.instance;
            if (__state.DoubleClicked && __instance != null && __instance.dragGo())
            {
                __state.DoubleClicked = false;
                GrabFullStackItemDrag(__instance);
                return false;
            }
            return true;
        }

        public static void Postfix(State __state)
        {
            var __instance = InventoryGui.instance;
            if (__state.DoubleClicked && __instance != null && __instance.dragGo())
            {
                GrabFullStackItemDrag(__instance);
            }
        }

        static void GrabFullStackItemDrag(InventoryGui __instance)
        {
            var dragItem = __instance.dragItem();
            if (dragItem == null || Player.m_localPlayer == null)
                return;

            var drag_amount = __instance.dragAmount();

            var currentContainer = __instance.currentContainer();
            if (currentContainer != null)
            {
                GrabFullStackItemDrag(__instance, currentContainer.GetInventory());
            }

            GrabFullStackItemDrag(__instance, Player.m_localPlayer.GetInventory());

            dragItem = __instance.dragItem();
            if (dragItem != null && drag_amount != dragItem.m_stack)
            {
                Utils.PlayMoveEffects(__instance);
                __instance.SetupDragItem(dragItem, __instance.dragInventory(), dragItem.m_stack);
                __instance.UpdateCraftingPanel();
            }
        }

        static void GrabFullStackItemDrag(InventoryGui __instance, Inventory inventory)
        {
            if (inventory == null)
                return;

            var dragItem = __instance.dragItem();
            var dragInventory = __instance.dragInventory();
            if (dragItem == null || dragInventory == null || dragItem.m_shared == null)
                return;

            List<ItemDrop.ItemData> items = new List<ItemDrop.ItemData>();
            inventory.GetAllItems(dragItem.m_shared.m_name, items);

            while (items.Any() && dragItem.m_stack < dragItem.m_shared.m_maxStackSize)
            {
                var item = items.Last();
                items.RemoveAt(items.Count - 1);

                if (Utils.CanStackItem(dragItem, item))
                {
                    dragInventory.MoveItemToThis(inventory, item, item.m_stack, dragItem.m_gridPos.x, dragItem.m_gridPos.y);
                }
            }
        }
    }
}
