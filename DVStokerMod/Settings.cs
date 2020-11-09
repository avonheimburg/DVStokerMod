using UnityEngine;
using UnityModManagerNet;

namespace DVStokerMod
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Cycle Stoker Up", DrawType.KeyBinding)]
        public KeyBinding StokerUp = new KeyBinding()
        {
            keyCode = KeyCode.Quote
        };

        [Draw("Cycle Stoker Down", DrawType.KeyBinding)]
        public KeyBinding StokerDown = new KeyBinding()
        {
            keyCode = KeyCode.Semicolon
        };
        public void OnChange() { }
        
        public override void Save(UnityModManager.ModEntry modEntry) => Save<Settings>(this, modEntry);
    }
}