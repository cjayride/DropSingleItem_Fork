using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DropSingleItem.Patch
{
    class RightClickButtonComponent : MonoBehaviour
    {
        public Button.ButtonClickedEvent OnRightClick = new Button.ButtonClickedEvent();
    }

    [HarmonyPatch(typeof(Button), "OnPointerClick")]
    class Button_OnPointerClick
    {
        public static void Postfix(Button __instance, PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (__instance.IsActive() && __instance.IsInteractable())
                {
                    var right_click_component = __instance.GetComponent<RightClickButtonComponent>();
                    if (right_click_component)
                    {
                        right_click_component.OnRightClick.Invoke();
                    }
                }
            }
        }
    }
}
