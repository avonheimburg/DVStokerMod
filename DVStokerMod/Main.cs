using System;
using System.Reflection;
using CommandTerminal;
using HarmonyLib;
using UnityModManagerNet;

namespace DVStokerMod
{
    [EnableReloading]
    public static class Main
    {
        private static Harmony? _harmonyInstance;

        public static bool Enabled = true;

        private static Settings _settings = new Settings();

        public static readonly Stoker Stoker = new Stoker();
        
        private static Harmony HarmonyInstance =>
            _harmonyInstance ??= new Harmony(nameof(DVStokerMod));

        public static bool Load(UnityModManager.ModEntry modEntry)
        {
            try
            {
                _settings = UnityModManager.ModSettings.Load<Settings>(modEntry);
                HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
                modEntry.OnToggle = Toggle;
                modEntry.OnUnload = Unload;
                modEntry.OnGUI = OnGui;
                modEntry.OnSaveGUI = OnSaveGui;
                modEntry.OnUpdate = OnUpdate;
            }
            catch (Exception exc)
            {
                modEntry.Logger.LogException(exc);
            }

            return true;
        }

        private static void OnUpdate(UnityModManager.ModEntry modeEntry, float deltaTime)
        {
            if (_settings.StokerMode.Down())
            {
                var newMode = Stoker.CycleMode();
                Terminal.Log("Stoker {0}", newMode);
            }
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
    class SimulateTickPatch
    {
        // ReSharper disable once InconsistentNaming
        static void Postfix(SteamLocoSimulation __instance)
        {
            if (Main.Enabled)
            {
                Main.Stoker.SimulateTick(__instance);
            }
        } 
    }
}