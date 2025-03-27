using Manager;
using UnityEngine;

namespace Entity
{
    public class Train : MonoBehaviour
    {
        [SerializeField] public GameObject[] cannonSlot = new GameObject[3];
        [SerializeField] public GameObject trainHead;
        [SerializeField] public GameObject trainBody;
        [SerializeField] public GameObject trainEnd;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<TestEnemy>(out var entityBase)) return;
            PoolManager.Release("Enemy", entityBase);
            EventManager.Instance.TriggerEvent("TrainSlowDown", this);
        }
    }
}