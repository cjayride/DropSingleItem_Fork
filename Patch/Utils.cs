using HarmonyLib;
using UnityEngine;

namespace DropSingleItem.Patch
{
    class Utils
    {
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
    }
}
