using Entity;
using UnityEngine;
using Utils;

namespace Manager
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private TestEnemy _playerPrefab;

        private void Start()
        {
            PoolManager.CreatePool("Enemy", _playerPrefab);
            PoolManager.Get<TestEnemy>("Enemy").Health = 100;
        }
    }
}