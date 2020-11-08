using System;
using System.Reflection;
using CommandTerminal;
using HarmonyLib;
using UnityEngine;
using UnityModManagerNet;
using Object = UnityEngine.Object;

namespace DVStokerMod
{
    [EnableReloading]
    public static class Main
    {
        private static Harmony? _harmonyInstance;

        public static bool Enabled = true;

        private static Settings _settings = new Settings();

        public static readonly Stoker Stoker = new Stoker();

        public static GameObject? BehaviourRoot;

        public static StatusDisplay? StatusDisplay;
        
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
                
                if (SaveLoadController.carsAndJobsLoadingFinished && WorldStreamingInit.IsLoaded)
                    OnLoadFinished();
                else
                    WorldStreamingInit.LoadingFinished += OnLoadFinished;
            }
            catch (Exception exc)
            {
                modEntry.Logger.LogException(exc);
            }

            return true;
        }

        private static void OnLoadFinished()
        {
            BehaviourRoot = new GameObject();
            BehaviourRoot.AddComponent<StatusDisplay>();
            StatusDisplay = BehaviourRoot.GetComponent<StatusDisplay>() ?? throw new ArgumentException("StatusDisplay not instantiated");
        }

        private static void OnUpdate(UnityModManager.ModEntry modeEntry, float deltaTime)
        {
            if (_settings.StokerMode.Down())
            {
                var newMode = Stoker.CycleMode();
                switch (newMode)
                {
                    case StokerMode.Low:
                    case StokerMode.Medium:
                    case StokerMode.High:
                        StatusDisplay?.Show($"Stoker {newMode}");
                        break;
                    default:
                        StatusDisplay?.Show(string.Empty);
                        break;
                }
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
            
            if (BehaviourRoot != null)
                Object.Destroy(BehaviourRoot);
            BehaviourRoot = null;
            
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
        static void Prefix(float delta)
        {
            if (Main.Enabled)
            {
                Main.Stoker.TimeElapsed(delta);
            }
        }
        
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