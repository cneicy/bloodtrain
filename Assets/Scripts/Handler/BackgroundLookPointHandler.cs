using UnityEngine;

namespace Handler
{
    //暂时没用
    public class BackgroundLookPointHandler : MonoBehaviour
    {
        void Update()
        {
            transform.LookAt(Camera.main.transform.position);
        }
    }
}
