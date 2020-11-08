using System;
using System.Collections.Generic;
using System.Reflection;
using CommandTerminal;
using Harmony12;
using UnityModManagerNet;

namespace DVStokerMod
{
    public static class Main
    {
        private static HarmonyInstance? _harmonyInstance = null;

        public static bool Enabled;

        private static Settings _settings = new Settings();

        public static readonly Stoker Stoker = new Stoker();

        private static HarmonyInstance HarmonyInstance =>
            _harmonyInstance ??= HarmonyInstance.Create(nameof(DVStokerMod));

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            try { _settings = UnityModManager.ModSettings.Load<Settings>(modEntry); } catch {}

            HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
            modEntry.OnToggle = Toggle;
            modEntry.OnUnload = Unload;
            modEntry.OnGUI = OnGui;
            modEntry.OnSaveGUI = OnSaveGui;
            modEntry.OnUpdate = OnUpdate;
            
            Terminal.Log("Loaded DVStoker");
            
            return true;
        }

        private static void OnUpdate(UnityModManager.ModEntry modeEntry, float deltaTime)
        {
            if (_settings.StokerMode.Down())
                Stoker.CycleMode();
        }

        private static void OnGui(UnityModManager.ModEntry modEntry)
        {
            _settings.Draw(modEntry);
        }

        private static void OnSaveGui(UnityModManager.ModEntry modEntry)
        {
            _settings.Save(modEntry);
        }

        public static bool Unload(UnityModManager.ModEntry modEntry)
        {
            HarmonyInstance.UnpatchAll(nameof(DVStokerMod));
            return true;
        }

        public static bool Toggle(UnityModManager.ModEntry modEntry, bool enabled)
        {
            Main.Enabled = enabled;
            return true;
        }
    }

    [HarmonyPatch(typeof(SteamLocoSimulation), "SimulateTick")]
    public static class SimulateTickPatch
    {
        public static void Postfix(SteamLocoSimulation instance, float deltaTime)
        {
            if (Main.Enabled)
            {
                Main.Stoker.SimulateTick(instance, deltaTime);
            }
        } 
    }
}