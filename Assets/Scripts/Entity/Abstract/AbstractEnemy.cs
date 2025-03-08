using System;
using Entity.Interface;
using Manager;
using UnityEngine;

namespace Entity.Abstract
{
    public class AbstractEnemy : MonoBehaviour,IEnemy
    {
        public int Health { get; set; }
        public float Speed { get; set; }
        
        // todo:此处参数应该为IAttack之类的可获取伤害的对象
        public int GetHurt(int damage)
        {
            Health -= damage;
            return Health;
        }
        public void Die()
        {
            /*var temp = (AbstractEnemy)EventManager.Instance.TriggerEvent("EnemyDie", this);
            print(temp.Health);*/
        }
        public void Attack()
        {
            throw new NotImplementedException();
        }
    }
}