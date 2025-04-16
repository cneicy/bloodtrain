using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine;

namespace Merge
{
    public class MergeBox : MonoBehaviour
    {
        [SerializeField] private Vector2 center, size;
        [SerializeField] private List<Collider2D> colliders = new();
        [SerializeField] private LayerMask layerMask;

        private void OnTriggerEnter2D(Collider2D other)
        {
            UpdateTotal();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            colliders = Physics2D.OverlapBoxAll(center, size, 0, layerMask).ToList();
            if (!RDNAManager.Instance) return;
            RDNAManager.Instance.total.Clear();
            if (colliders is null) return;
            foreach (var temp in colliders.Select(item =>
                         item.gameObject.name.Remove(item.gameObject.name.Length - 7, 7)))
            {
                if (!RDNAManager.Instance.total.ContainsKey(temp))
                {
                    RDNAManager.Instance.total.Add(temp, 1);
                }
                else
                {
                    RDNAManager.Instance.total[temp]++;
                }
            }
        }
    }
}