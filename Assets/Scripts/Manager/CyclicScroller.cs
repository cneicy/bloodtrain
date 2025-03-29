using System.Collections;
using System.Linq;
using UnityEngine;

namespace Manager
{
    public class CyclicScroller : MonoBehaviour
    {
        [Header("Sprite Settings")] public Transform[] sprites = new Transform[5];
        public float failSpeed = 0.3f;
        public float scrollSpeed = 1f;

        private float _spriteWidth;

        private void OnEnable()
        {
            EventManager.Instance.RegisterEventHandlersFromAttributes(this);
        }

        private void OnDisable()
        {
            if (!EventManager.Instance) return;
            EventManager.Instance.UnregisterAllEventsForObject(this);
        }

        //todo:列车降速动态背景降速
        [EventSubscribe("TrainSlowDown")]
        public object OnTrainSlowDown(Object entity)
        {
            StartCoroutine(Slow());
            return this;
        }

        //todo:列车降速
        private IEnumerator Slow()
        {
            scrollSpeed *= 0.9f;
            yield return new WaitForSeconds(1f);
            scrollSpeed /= 0.9f;
        }

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

        private void Update()
        {
            foreach (var sprite in sprites) sprite.Translate(Vector3.left * (scrollSpeed * Time.deltaTime));

            var leftmost = sprites.OrderBy(s => s.position.x).First();

            if (scrollSpeed < failSpeed)
            {
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