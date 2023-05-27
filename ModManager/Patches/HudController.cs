using HarmonyLib;
using ModManager.UI;
using UnityEngine.UI;

namespace ModManager.Patches
{
    // Hook our Mods menu into the game's pause screen
    [HarmonyPatch(typeof(HudController), "Start")]
    class HudController_Patch_Start
    {
        [HarmonyPrefix]
        static bool Prefix(VerticalLayoutGroup ___PauseLayout)
        {
            OptionsMenu.Initialize(___PauseLayout.transform);
            return true;
        }
    }
}
