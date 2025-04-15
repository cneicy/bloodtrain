using UnityEngine;

namespace Handler
{
    public class Debugger : MonoBehaviour
    {
        public static bool IsDebugging = false;

        public void Switch()
        {
            IsDebugging = !IsDebugging;
        }
    }
}