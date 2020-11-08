using UnityEngine;

namespace DVStokerMod
{
    public class StatusDisplay : MonoBehaviour
    {
        private string _message = "";


        void OnGUI()
        {
            GUI.Label(new Rect(10, Screen.height - 30, 200, 20), _message);
        }

        public void Show(string message)
        {
            _message = message;
        }
    }
}