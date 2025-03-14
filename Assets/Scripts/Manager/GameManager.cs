using Entity;
using UnityEngine;
using Utils;

namespace Manager
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private TestEnemy enemyPrefab;

        private void Start()
        {
            PoolManager.CreatePool("Enemy", enemyPrefab);
            PoolManager.Get<TestEnemy>("Enemy");
        }
    }
}