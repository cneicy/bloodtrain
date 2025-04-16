using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Manager
{
    public class RDNAManager : Singleton<RDNAManager>
    {
        [FormerlySerializedAs("RDNA")]
        [SerializeField]
        public List<GameObject> rdna = new();

        [EventSubscribe("MouseLeftClick")]
        public object OnMouseLeftClick(Vector3 hitPoint)
        {
            foreach (var item in rdna)
            {
                item.transform.position = hitPoint;
            }
            return this;
        }
    }
}