using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ModManager
{
    // Dummy plugin which should (almost) always be loaded first due to the GUID prefix
    // Allows the main plugin to be loaded first without changing its GUID
    [BepInDependency(Metadata.PLUGIN_ID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin($"..{Metadata.PLUGIN_ID}lineskipper", $"{Metadata.PLUGIN_NAME}LineSkipper", Metadata.PLUGIN_VERSION)]
    [BepInProcess("SpiderHeckApp.exe")]
    internal class LineSkipper : BaseUnityPlugin { }

    [BepInPlugin(Metadata.PLUGIN_ID, Metadata.PLUGIN_NAME, Metadata.PLUGIN_VERSION)]
    [BepInProcess("SpiderHeckApp.exe")]
    internal class ModManager : BaseUnityPlugin
    {
        public static ModManager instance;
        internal static ManualLogSource logger;
        internal static Dictionary<string, PluginInfo> disabledPlugins = new();

        private ModManager() : base()
        {
            ModManager.instance = this;
        }

        private void Awake()
        {
            logger = Logger;

            try
            {
                Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), Metadata.PLUGIN_ID);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
    }
}
