using Entity;
using UnityEngine;
using Utils;

namespace Manager
{
    //测试用，暂时没啥用
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] private TestEnemy enemyPrefab;

        private void Start()
        {
            
        }
    }
}