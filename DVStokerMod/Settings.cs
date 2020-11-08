using UnityEngine;
using UnityModManagerNet;

namespace DVStokerMod
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Toggle Stoker Mode", DrawType.KeyBinding)]
        public KeyBinding StokerMode = new KeyBinding()
        {
            keyCode = KeyCode.Semicolon
        };


        public void OnChange()
        {
            // 
        }
    }
}