using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.Bootstrap;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ModManager.UI
{
    /// <summary>
    /// A menu to bind settings to UI elements, and automatically update their config file.
    /// </summary>
    /// <example>
    /// <para>
    /// Creating a basic options menu:
    /// <code>
    /// ConfigEntry{bool} playSounds = Config.Bind("General", "PlaySounds", true);
    ///
    /// OptionsMenu optionsMenu = new OptionsMenu("Super Strength");
    /// optionsMenu.AddToggle("Play sounds", playSounds);
    /// </code>
    /// </para>
    /// </example>
    public class OptionsMenu
    {
        private static bool isReady = false;
        private static GameObject modsMenu;
        private static List<OptionsMenu> mods = new();

        private List<Action> callbacks = new();
        public GameObject modMenu;

        /// <summary>
        /// Sets up the main Mods menu in the pause screen.
        /// </summary>
        ///
        /// <param name="mainButtons">The container for the pause menu buttons.</param>
        internal static void Initialize(Transform mainButtons)
        {
            if (isReady) return;

            // Initialise Mods menu
            GameObject optionsButton = mainButtons.Find("Button - Options").gameObject;
            modsMenu = MenuManager.CreateView
            (
                name: "Mods",
                viewButton: out _,
                buttonParent: mainButtons.transform,
                buttonSiblingIndex: optionsButton.transform.GetSiblingIndex() + 1
            );

            // Bind an enable/disable setting for each existing plugin
            // TODO: move this logic to GameObject_Patch_AddComponent
            OptionsMenu           manageMenu = new OptionsMenu("Manage Mods");
            IEnumerable<string> dependencies = ModManager.instance.Info.Dependencies.Select(dep => dep.DependencyGUID);

            foreach (var plugin in UnityChainloader.Instance.Plugins.Concat(ModManager.disabledPlugins))
            {
                // kv destruct not implemented in netstandard2.0 ;-;
                string     GUID = plugin.Key;
                PluginInfo info = plugin.Value;

                // Don't add a setting for us or our dependencies
                if (GUID == Metadata.PLUGIN_ID || dependencies.Contains(GUID)) continue;

                ConfigEntry<bool> pluginEnabled = ModManager.instance.Config.Bind("Enabled", GUID, true);
                manageMenu.AddToggle(info.Metadata.Name, pluginEnabled);
            }

            // Setup pre-registered mods
            isReady = true;
            mods.ForEach(mod => mod.callbacks.ForEach(callback => callback()));
        }

        // Wish I had python decorators :(
        private void InvokeWhenReady(Action callback)
        {
            if (isReady) callback();
            else callbacks.Add(callback);
        }

        /// <summary>
        /// Registers for a new <see cref="OptionsMenu"/> instance.
        /// </summary>
        /// <param name="name">The text to display on the menu button.</param>
        public OptionsMenu(string name)
        {
            mods.Add(this);

            InvokeWhenReady(() =>
            {
                modMenu = MenuManager.CreateView
                (
                    name: name,
                    viewButton: out _,
                    buttonParent: modsMenu.transform
                );
            });
        }

        /// <summary>
        /// Adds a button to this <see cref="OptionsMenu"/>.
        /// </summary>
        /// <param name="text">The text to display on the button.</param>
        /// <param name="onClick">The action to perform when the button is clicked.</param>
        /// <returns>
        /// The <see cref="OptionsMenu"/> instance, for chaining.
        /// </returns>
        public OptionsMenu AddButton(string text, UnityAction onClick)
        {
            InvokeWhenReady(() =>
            {
                MenuManager.CreateButton
                (
                    text: text,
                    onClick: onClick,
                    parent: modMenu.transform
                );
            });

            return this;
        }

        /// <summary>
        /// Adds text to this <see cref="OptionsMenu"/>.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <returns>
        /// The <see cref="OptionsMenu"/> instance, for chaining.
        /// </returns>
        public OptionsMenu AddText(string text)
        {
            InvokeWhenReady(() =>
            {
                MenuManager.CreateText
                (
                    text: text,
                    parent: modMenu.transform
                );
            });

            return this;
        }

        /// <summary>
        /// Adds a toggle to this <see cref="OptionsMenu"/>.
        /// </summary>
        /// <param name="text">The text to display next to the toggle.</param>
        /// <param name="binding">The <see cref="ConfigEntry{}"/> to bind the toggle's value to.</param>
        /// <returns>
        /// The <see cref="OptionsMenu"/> instance, for chaining.
        /// </returns>
        public OptionsMenu AddToggle(string text, ConfigEntry<bool> binding)
        {
            InvokeWhenReady(() =>
            {
                MenuManager.CreateToggle
                (
                    text: text,
                    defaultValue: binding.Value,
                    onValueChanged: (value) => binding.Value = value,
                    parent: modMenu.transform
                );
            });

            return this;
        }
    }
}
