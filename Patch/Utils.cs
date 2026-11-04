using UnityEngine;
using System.Collections.Generic;

namespace DropSingleItem.Patch
{
    class Utils
    {
        public static bool placingOneFromDrag;

        static readonly List<ItemDrop.ItemData> mergeBuffer = new List<ItemDrop.ItemData>(16);

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
            var effects = instance.m_moveItemEffects;
            if (effects == null)
                return;
            effects.Create(instance.transform.position, Quaternion.identity, null, 1f, -1, ZDOID.None);
        }

        public static bool TrySplitStackIntoEmptySlot(Inventory inventory, ItemDrop.ItemData item)
        {
            if (inventory == null || item == null || item.m_stack < 2 || !inventory.HaveEmptySlot())
                return false;

            Vector2i empty = GameAccess.FindEmptySlot(inventory, true);
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

            var currentContainer = GameAccess.CurrentContainer(gui);
            if (currentContainer != null)
                MergeMatchingStacksInto(targetInventory, target, currentContainer.GetInventory());

            MergeMatchingStacksInto(targetInventory, target, Player.m_localPlayer.GetInventory());

            if (target.m_stack != before)
                PlayMoveEffects(gui);

            GameAccess.SetupDragItem(gui, null, null, 1);
            GameAccess.UpdateCraftingPanel(gui, false);
        }

        static void MergeMatchingStacksInto(Inventory targetInventory, ItemDrop.ItemData target, Inventory source)
        {
            if (source == null || target == null || target.m_shared == null)
                return;

            mergeBuffer.Clear();
            source.GetAllItems(target.m_shared.m_name, mergeBuffer);

            for (int i = mergeBuffer.Count - 1; i >= 0 && target.m_stack < target.m_shared.m_maxStackSize; i--)
            {
                var item = mergeBuffer[i];
                if (CanStackItem(target, item))
                    targetInventory.MoveItemToThis(source, item, item.m_stack, target.m_gridPos.x, target.m_gridPos.y);
            }

            mergeBuffer.Clear();
        }

        public static bool PointerOverInventoryGrid(InventoryGui gui)
        {
            return ContainsPointer(gui.m_playerGrid) || ContainsPointer(gui.ContainerGrid);
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

        static ItemDrop lastSingleDrop;
        static float lastSingleDropTime;
        const float SingleDropMergeSeconds = 2f;
        const float SingleDropMergeRange = 4f;
        const float SingleDropMergeRangeSq = SingleDropMergeRange * SingleDropMergeRange;

        public static void DropOneOutside(InventoryGui gui)
        {
            if (gui == null || GameAccess.DragGo(gui) == null)
                return;

            var dragItem = GameAccess.DragItem(gui);
            var dragInventory = GameAccess.DragInventory(gui);
            if (dragItem == null || dragInventory == null || Player.m_localPlayer == null)
                return;

            var dragAmount = GameAccess.DragAmount(gui);
            var dragStack = dragItem.m_stack;

            if (!dragInventory.ContainsItem(dragItem))
            {
                GameAccess.SetupDragItem(gui, null, null, 1);
                return;
            }

            if (TryAddOneToLastDrop(dragInventory, dragItem))
            {
                PlayMoveEffects(gui);
                FinishDropOneDrag(gui, dragItem, dragInventory, dragAmount, dragStack);
                return;
            }

            if (!Player.m_localPlayer.DropItem(dragInventory, dragItem, 1))
                return;

            var drop = FindNewestNearbyDrop(dragItem, Player.m_localPlayer.transform.position);
            SoftenDropPhysics(drop);
            RememberSingleDrop(drop);

            PlayMoveEffects(gui);
            FinishDropOneDrag(gui, dragItem, dragInventory, dragAmount, dragStack);
        }

        static void FinishDropOneDrag(InventoryGui gui, ItemDrop.ItemData dragItem, Inventory dragInventory, int dragAmount, int dragStack)
        {
            dragAmount -= dragStack - dragItem.m_stack;
            if (dragInventory.ContainsItem(dragItem) && dragAmount > 0)
                GameAccess.SetupDragItem(gui, dragItem, dragInventory, dragAmount);
            else
                GameAccess.SetupDragItem(gui, null, null, 1);

            GameAccess.UpdateCraftingPanel(gui, false);
        }

        static bool TryAddOneToLastDrop(Inventory dragInventory, ItemDrop.ItemData dragItem)
        {
            if (lastSingleDrop == null || Time.time - lastSingleDropTime > SingleDropMergeSeconds)
                return false;

            if (lastSingleDrop.m_itemData == null || lastSingleDrop.m_itemData.m_shared == null)
                return false;

            if (!CanStackItem(lastSingleDrop.m_itemData, dragItem))
                return false;

            if (lastSingleDrop.m_itemData.m_stack >= lastSingleDrop.m_itemData.m_shared.m_maxStackSize)
                return false;

            if ((lastSingleDrop.transform.position - Player.m_localPlayer.transform.position).sqrMagnitude > SingleDropMergeRangeSq)
                return false;

            if (!dragInventory.RemoveItem(dragItem, 1))
                return false;

            lastSingleDrop.SetStack(lastSingleDrop.m_itemData.m_stack + 1);
            lastSingleDropTime = Time.time;
            return true;
        }

        static void RememberSingleDrop(ItemDrop drop)
        {
            lastSingleDrop = drop;
            lastSingleDropTime = drop != null ? Time.time : 0f;
        }

        static ItemDrop FindNewestNearbyDrop(ItemDrop.ItemData item, Vector3 origin)
        {
            if (item == null || item.m_shared == null)
                return null;

            var instances = GameAccess.ItemDropInstances();
            if (instances == null)
                return null;

            ItemDrop best = null;
            float bestSpawn = -1f;
            string name = item.m_shared.m_name;

            for (int i = 0, n = instances.Count; i < n; i++)
            {
                var drop = instances[i];
                if (drop == null || drop.m_itemData == null || drop.m_itemData.m_shared == null)
                    continue;
                if (drop.m_itemData.m_shared.m_name != name)
                    continue;
                if ((drop.transform.position - origin).sqrMagnitude > SingleDropMergeRangeSq)
                    continue;

                float spawn = GameAccess.SpawnTime(drop);
                if (spawn >= bestSpawn)
                {
                    bestSpawn = spawn;
                    best = drop;
                }
            }

            return best;
        }

        static void SoftenDropPhysics(ItemDrop drop)
        {
            if (drop == null)
                return;

            var body = drop.GetComponent<Rigidbody>();
            if (body == null)
                return;

            Vector3 v = body.linearVelocity;
            if (v.y > 1.25f)
                v.y = 1.25f;
            if (v.sqrMagnitude > 9f)
                v = v.normalized * 3f;
            body.linearVelocity = v;
        }
    }
}
