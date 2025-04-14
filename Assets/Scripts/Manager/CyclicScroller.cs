using System.Collections;
using System.Linq;
using Entity;
using UnityEngine;

namespace Manager
{
    /// <summary>
    /// 差速背景
    /// 原理为调整不同层级的<c>scrollSpeed</c>后形成一种前后景观存在差速的立体感
    /// 要保证前方层级的<c>scrollSpeed</c>大于后方的<c>scrollSpeed</c>
    /// </summary>
    public class CyclicScroller : MonoBehaviour
    {
        [Header("Sprite Settings")] public Transform[] sprites = new Transform[5];
        public float failSpeed = 0.3f;
        public float scrollSpeed = 1f;
        public int lowFuelTime;//燃料耗尽事件触发次数
        private float _spriteWidth;//精灵图宽度

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
        /// 当列车降速事件触发后背景图降低到原来的0.9倍(多次触发可叠加)
        /// 并持续1s后恢复
        /// </summary>
        [EventSubscribe("TrainSlowDown")]
        public object OnTrainSlowDown(Train sender)
        {
            StartCoroutine(Slow());
            return this;
        }

        /// <summary>
        /// 当列车燃料耗尽触发后背景图降低到原来的0.9倍(多次触发可叠加)
        /// </summary>
        [EventSubscribe("TrainLowFuel")]
        public object OnTrainLowFuel(Train sender)
        {
            scrollSpeed *= 0.9f;
            lowFuelTime++;
            return this;
        }
        
        /// <summary>
        /// 当列车燃料充足事件触发后恢复速度
        /// todo:缓慢恢复速度
        /// </summary>
        [EventSubscribe("TrainEnoughFuel")]
        public object OnTrainEnoughFuel(Train sender)
        {
            for (var i = lowFuelTime; i > 0; i--)
            {
                scrollSpeed /= 0.9f;
            }
            return this;
        }
        
        private IEnumerator Slow()
        {
            scrollSpeed *= 0.9f;
            yield return new WaitForSeconds(1f);
            scrollSpeed /= 0.9f;
        }
        

        //根据宽度的位置计算来决定同一层级的五张图的位置
        private void Awake()
        {
            var sr = sprites[0].GetComponent<SpriteRenderer>();
            _spriteWidth = sr.bounds.size.x;

            var startPos = sprites[0].position;

            sprites[1].position = startPos - new Vector3(_spriteWidth, 0, 0);
            sprites[2].position = startPos + new Vector3(_spriteWidth, 0, 0);
            sprites[3].position = startPos + new Vector3(_spriteWidth * 3, 0, 0);
            sprites[4].position = startPos + new Vector3(_spriteWidth * 2, 0, 0);
        }
        
        private void FixedUpdate()
        {
            foreach (var sprite in sprites) sprite.Translate(Vector3.left * (scrollSpeed * Time.fixedDeltaTime));

            var leftmost = sprites.OrderBy(s => s.position.x).First();

            if (scrollSpeed < failSpeed)
            {
                //当背景速度触发阈值后执行游戏结束事件
                EventManager.Instance.TriggerEvent("GameFail", scrollSpeed);
            }

            if (!(leftmost.position.x < -_spriteWidth * 2)) return;
            var newPos = new Vector3(
                _spriteWidth * 3,
                leftmost.position.y,
                leftmost.position.z
            );
            leftmost.position = newPos;
        }
    }
}