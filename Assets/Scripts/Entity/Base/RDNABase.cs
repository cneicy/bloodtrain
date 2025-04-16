using System.Collections;
using Cinemachine;
using Handler;
using UnityEngine;
using Utils;

namespace Entity.Base
{
    public abstract class RDNABase : MonoBehaviour
    {
        public static bool Half = false; //对半
        [SerializeField] public SerializableDictionary<string, GameObject> recipe = new();
        [SerializeField] public int phrase;
        [SerializeField] public int level;

        private void Awake()
        {
            recipe.SyncDictionary();
        }

        private IEnumerator Yee()
        {
            GetComponent<AudioSource>().Play();
            GameObject.FindWithTag("MergeCamera").GetComponent<CinemachineVirtualCamera>().LookAt = transform;
            yield return new WaitForSeconds(1.5f);
            GameObject.FindWithTag("MergeCamera").GetComponent<CinemachineVirtualCamera>().LookAt =
                GameObject.FindWithTag("RDNAPanel").transform;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Debug.Log(collision.collider.name);
            if(collision.gameObject.name.Remove(collision.gameObject.name.Length - 7, 7).Equals("")) return;//防止空值通过给槽给合了
            if (recipe.ContainsKey(collision.gameObject.name.Remove(collision.gameObject.name.Length - 7, 7)))//删除(Clone)
            {
                MergeHandler.Merge(this, collision);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.name == "GG")
            {
                StartCoroutine(Yee());
            }
        }
    }
}