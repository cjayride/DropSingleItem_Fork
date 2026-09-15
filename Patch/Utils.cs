using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace DropSingleItem.Patch
{
    class Utils
    {
        public static bool placingOneFromDrag;

        public static bool IsShiftHeld()
        {
            return ZInput.GetKey(KeyCode.LeftShift, true) || ZInput.GetKey(KeyCode.RightShift, true);
        }

        public static bool CanStackItem(ItemDrop.ItemData a, ItemDrop.ItemData b)
        {
            if (a == null || b == null)
                return true;

            if (a == b)
                return false;

            if (a.m_shared.m_name != b.m_shared.m_name)
                return false;

            if (a.m_shared.m_maxStackSize == 1 || b.m_shared.m_maxStackSize == 1)
                return false;

            if (a.m_shared.m_maxQuality > 1 || b.m_shared.m_maxQuality > 1)
            {
                if (a.m_quality != b.m_quality)
                    return false;
            }

            return true;
        }

        public static void PlayMoveEffects(InventoryGui instance)
        {
            var effects = instance.moveItemEffects();
            if (effects == null)
                return;
            effects.Create(instance.transform.position, Quaternion.identity, null, 1f, -1, ZDOID.None);
        }

        public static bool TrySplitStackIntoEmptySlot(Inventory inventory, ItemDrop.ItemData item)
        {
            if (inventory == null || item == null || item.m_stack < 2 || !inventory.HaveEmptySlot())
                return false;

            Vector2i empty = Traverse.Create(inventory).Method("FindEmptySlot", new System.Type[] { typeof(bool) }).GetValue<Vector2i>(true);
            if (empty.x < 0 || empty.y < 0)
                return false;

            int half = item.m_stack / 2;
            return inventory.MoveItemToThis(inventory, item, half, empty.x, empty.y);
        }

        public static void MergeMatchingIntoSlot(InventoryGui gui, Inventory targetInventory, ItemDrop.ItemData target)
        {
            if (gui == null || targetInventory == null || target == null || Player.m_localPlayer == null || placingOneFromDrag)
                return;

            int before = target.m_stack;

            var currentContainer = gui.currentContainer();
            if (currentContainer != null)
                MergeMatchingStacksInto(targetInventory, target, currentContainer.GetInventory());

            MergeMatchingStacksInto(targetInventory, target, Player.m_localPlayer.GetInventory());

            if (target.m_stack != before)
                PlayMoveEffects(gui);

            gui.SetupDragItem(null, null, 1);
            gui.UpdateCraftingPanel();
        }

        static void MergeMatchingStacksInto(Inventory targetInventory, ItemDrop.ItemData target, Inventory source)
        {
            if (source == null || target == null || target.m_shared == null)
                return;

            List<ItemDrop.ItemData> items = new List<ItemDrop.ItemData>();
            source.GetAllItems(target.m_shared.m_name, items);

            while (items.Any() && target.m_stack < target.m_shared.m_maxStackSize)
            {
                var item = items.Last();
                items.RemoveAt(items.Count - 1);

                if (CanStackItem(target, item))
                    targetInventory.MoveItemToThis(source, item, item.m_stack, target.m_gridPos.x, target.m_gridPos.y);
            }
        }

        public static bool PointerOverInventoryGrid(InventoryGui gui)
        {
            return ContainsPointer(gui.playerGrid()) || ContainsPointer(gui.containerGrid());
        }

        static bool ContainsPointer(Component component)
        {
            if (component == null)
                return false;

            var rt = component.transform as RectTransform;
            if (rt == null)
                return false;

            var canvas = component.GetComponentInParent<Canvas>();
            Camera cam = null;
            if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                cam = canvas.worldCamera;

            return RectTransformUtility.RectangleContainsScreenPoint(rt, ZInput.pointerPosition, cam);
        }

        public static void DropOneOutside(InventoryGui gui)
        {
            if (gui == null || gui.dragGo() == null)
                return;

            var dragItem = gui.dragItem();
            var dragInventory = gui.dragInventory();
            if (dragItem == null || dragInventory == null || Player.m_localPlayer == null)
                return;

            var dragAmount = gui.dragAmount();
            var dragStack = dragItem.m_stack;

            if (!dragInventory.ContainsItem(dragItem))
            {
                gui.SetupDragItem(null, null, 1);
                return;
            }

            if (!Player.m_localPlayer.DropItem(dragInventory, dragItem, 1))
                return;

            PlayMoveEffects(gui);

            dragAmount -= dragStack - dragItem.m_stack;
            if (dragInventory.ContainsItem(dragItem) && dragAmount > 0)
                gui.SetupDragItem(dragItem, dragInventory, dragAmount);
            else
                gui.SetupDragItem(null, null, 1);

            gui.UpdateCraftingPanel();
        }
    }

    static class InventoryGui_Extensions
    {
        public static void SetupDragItem(this InventoryGui instance, ItemDrop.ItemData item, Inventory inventory, int amount)
        {
            Traverse.Create(instance).Method("SetupDragItem", new System.Type[] {
                typeof(ItemDrop.ItemData), typeof(Inventory), typeof(int)
            }).GetValue(item, inventory, amount);
        }

        public static void UpdateCraftingPanel(this InventoryGui instance, bool focusView = false)
        {
            Traverse.Create(instance).Method("UpdateCraftingPanel", new System.Type[] {
                typeof(bool)
            }).GetValue(focusView);
        }

        public static void OnSelectedItem(this InventoryGui instance, InventoryGrid grid, ItemDrop.ItemData item, Vector2i pos, InventoryGrid.Modifier mod)
        {
            Traverse.Create(instance).Method("OnSelectedItem", new System.Type[] {
                typeof(InventoryGrid), typeof(ItemDrop.ItemData), typeof(Vector2i), typeof(InventoryGrid.Modifier)
            }).GetValue(grid, item, pos, mod);
        }

        public static EffectList moveItemEffects(this InventoryGui instance)
        {
            return Traverse.Create(instance).Field("m_moveItemEffects").GetValue<EffectList>();
        }

        public static GameObject dragGo(this InventoryGui instance)
        {
            return Traverse.Create(instance).Field("m_dragGo").GetValue<GameObject>();
        }

        public static ItemDrop.ItemData dragItem(this InventoryGui instance)
        {
            return Traverse.Create(instance).Field("m_dragItem").GetValue<ItemDrop.ItemData>();
        }

        public static Inventory dragInventory(this InventoryGui instance)
        {
            return Traverse.Create(instance).Field("m_dragInventory").GetValue<Inventory>();
        }

        public static int dragAmount(this InventoryGui instance)
        {
            return Traverse.Create(instance).Field("m_dragAmount").GetValue<int>();
        }

        public static Container currentContainer(this InventoryGui instance)
        {
            return Traverse.Create(instance).Field("m_currentContainer").GetValue<Container>();
        }

        public static InventoryGrid playerGrid(this InventoryGui instance)
        {
            return Traverse.Create(instance).Field("m_playerGrid").GetValue<InventoryGrid>();
        }

        public static InventoryGrid containerGrid(this InventoryGui instance)
        {
            return Traverse.Create(instance).Field("m_containerGrid").GetValue<InventoryGrid>();
        }
    }
}
