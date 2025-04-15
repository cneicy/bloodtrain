using System.Collections;
using Cinemachine;
using Entity.Interface;
using Handler;
using UnityEngine;
using Utils;

namespace Entity.Base
{
    public abstract class RDNABase : MonoBehaviour, IShootable
    {
        public static bool Half = false;//对半
        public int ammoCount;
        [SerializeField]public SerializableDictionary<string,GameObject> recipe = new();
        //公有方法 射击
        public void Shoot()
        {
        }

        private void Awake()
        {
            recipe.SyncDictionary(); 
        }

        private IEnumerator Yee()
        {
            GetComponent<AudioSource>().Play();
            GameObject.FindWithTag("MergeCamera").GetComponent<CinemachineVirtualCamera>().LookAt = transform;
            yield return new WaitForSeconds(1.5f);
            GameObject.FindWithTag("MergeCamera").GetComponent<CinemachineVirtualCamera>().LookAt = GameObject.FindWithTag("RDNAPanel").transform;
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (recipe.ContainsKey(collision.gameObject.name))
            {
                MergeHandler.Merge(this,collision);
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