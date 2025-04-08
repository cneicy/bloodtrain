using System.Collections;
using Manager;
using UnityEngine;
using Utils;

namespace Entity
{
    /// <summary>
    /// 刷怪
    /// todo:实现可设置多波次生成不同的敌人
    /// </summary>
    public class MonsterSpawner : Singleton<MonsterSpawner>
    {
        [SerializeField] private TestEnemy enemyPrefab;//敌人预制体
        public float spawnInterval = 3f;//刷怪时间间隔
        public int spawnCount = 1;//刷怪数量
        public float spawnRadius = 5f;//刷怪半径

        private void OnEnable()
        {
            EventManager.Instance.RegisterEventHandlersFromAttributes(this);//处理[EventSubscribe()]特性标注的事件订阅
        }

        private void OnDisable()
        {
            if (!EventManager.Instance) return;
            EventManager.Instance.UnregisterAllEventsForObject(this);//事件取消订阅
        }

        /// <summary>
        /// 在触发游戏开始事件后根据<c>enemyPrefab</c>创建对象池
        /// 并启动刷怪协程
        /// </summary>
        [EventSubscribe("GameStart")]
        public object OnGameStart(Object obj)
        {
            PoolManager.CreatePool("Enemy", enemyPrefab, 100, 2000);
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

        /// <summary>
        /// 刷怪具体逻辑
        /// </summary>
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