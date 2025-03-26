using UnityEngine;

namespace Entity
{
    public class Train : MonoBehaviour
    {
        [SerializeField] public GameObject[] cannonSlot =  new GameObject[3];
        [SerializeField] public GameObject trainHead;
        [SerializeField] public GameObject trainBody;
        [SerializeField] public GameObject trainEnd;
        
    }
}