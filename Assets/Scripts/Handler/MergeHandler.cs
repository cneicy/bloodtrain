using Entity.Base;
using UnityEngine;

namespace Handler
{
    public static class MergeHandler
    {
        public static void Merge(RDNABase self, Collision2D collision2D)
        {
            if (self.recipe == null) return;
            if (!RDNABase.Half)
            {
                RDNABase.Half = true;
            }
            else
            {
                var newPosition = (collision2D.gameObject.transform.position + self.transform.position) / 2;
                Object.Destroy(self.gameObject);
                Object.Destroy(collision2D.gameObject);
                Object.Instantiate(self.recipe[collision2D.gameObject.name],newPosition,Quaternion.identity);
                RDNABase.Half = false;
            }
        }
    }
}