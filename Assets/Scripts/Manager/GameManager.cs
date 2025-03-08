/*using Entity;
using Entity.Abstract;
using UnityEngine;*/
using Utils;

namespace Manager
{
    public class GameManager : Singleton<GameManager>
    {
        //private GameObject _testEnemy;
        
        private void Start()
        {
            /*EventManager.Instance.RegisterEventHandlersFromAttributes(this);
            _testEnemy = Instantiate(new GameObject("test"));
            _testEnemy.AddComponent<TestEnemy>();
            _testEnemy.GetComponent<TestEnemy>().Die();*/
        }

        /*[EventSubscribe("EnemyDie")]
        public object OnEnemyDie(AbstractEnemy abstractEnemy)
        {
            print(abstractEnemy.Health);
            abstractEnemy.Health = 100;
            return _testEnemy.GetComponent<TestEnemy>();
        }*/
    }
}