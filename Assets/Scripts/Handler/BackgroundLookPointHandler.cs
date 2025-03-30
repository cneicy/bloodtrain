using UnityEngine;

namespace Handler
{
    public class BackgroundLookPointHandler : MonoBehaviour
    {
        void Update()
        {
            transform.LookAt(Camera.main.transform.position);
        }
    }
}
