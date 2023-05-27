using HarmonyLib;
using ModManager.UI;

namespace ModManager.Patches
{
    // Initialize MenuManager as soon as the GameController is ready
    [HarmonyPatch(typeof(GameController), "Start")]
    class GameController_Patch_Start
    {
        [HarmonyPostfix]
        static void Postfix()
        {
            MenuManager.Initialize(GameController.instance.hud.transform);
        }
    }
}
