using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.Bootstrap;
using HarmonyLib;
using ModManager.Extensions;
using System;
using System.Linq;
using UnityEngine;

namespace ModManager.Patches
{
    // Hook into the GameObject.AddComponent(Type) method, and listen for any plugin components being added to the chainloader
    // If the plugin is disabled in config, block the component from being added
    // TODO: See if using a preloader patcher to hook into BaseChainloader is possible instead
    [HarmonyPatch(typeof(GameObject), nameof(GameObject.AddComponent), new Type[] { typeof(Type) })]
    internal class GameObject_Patch_AddComponent
    {
        [HarmonyPrefix]
        static bool Prefix(GameObject __instance, Type componentType)
        {
            if (__instance != UnityChainloader.ManagerObject) return true;
            if (componentType == typeof(LineSkipper)) throw new MessageOnlyException("ModManager cleanup; safe to ignore (☆´▿`)b");

            // Check if the plugin should be disabled
            PluginInfo info = UnityChainloader.Instance.Plugins.Last().Value;
            ConfigEntry<bool> pluginEnabled = ModManager.instance.Config.Bind<bool>("Enabled", info.Metadata.GUID, true);
            if (!pluginEnabled.Value)
            {
                // Block the component from being added
                // Not exactly elegant, but it lets BepInEx handle the cleanup in a try/catch
                // Since this is an intentional exception, we don't want to cause a stack trace
                ModManager.disabledPlugins.Add(info.Metadata.GUID, info);
                throw new MessageOnlyException("Plugin disabled by ModManager; safe to ignore (☆´▿`)b");
            }

            return true;
        }
    }
}
