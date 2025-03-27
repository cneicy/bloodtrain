using System.Collections;
using Manager;
using UnityEngine;
using Utils;

//todo:对象池暂不支持传入抽象类以释放子类且外部Release后不会回到设定的transform上
namespace Entity
{
    public class MonsterSpawner : Singleton<MonsterSpawner>
    {
        [SerializeField] private TestEnemy enemyPrefab;
        public float spawnInterval = 3f;
        public int spawnCount = 1;
        public float spawnRadius = 5f;

        private void OnEnable()
        {
            EventManager.Instance.RegisterEventHandlersFromAttributes(this);
        }

        private void OnDisable()
        {
            if (!EventManager.Instance) return;
            EventManager.Instance.UnregisterAllEventsForObject(this);
        }

        [EventSubscribe("GameStart")]
        public object OnGameStart(Object obj)
        {
            PoolManager.CreatePool("Enemy", enemyPrefab, 100, 200);
            StartCoroutine(SpawnEnemiesCoroutine());
            return null;
        }

        private IEnumerator SpawnEnemiesCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnInterval);
                SpawnEnemies();
            }
        }

        private void SpawnEnemies()
        {
            for (var i = 0; i < spawnCount; i++)
            {
                var rand = Random.insideUnitCircle;
                var spawnPosition = transform.position + new Vector3(rand.x, 0, rand.y) * spawnRadius;
                PoolManager.Get<TestEnemy>("Enemy").transform.position = spawnPosition;
            }
        }
    }
}