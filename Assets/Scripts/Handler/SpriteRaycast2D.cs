using Manager;
using UnityEngine;

namespace Handler
{
    public class SpriteRaycast2D : MonoBehaviour
    {
        [Header("射线参数")]
        [Tooltip("检测使用的2D层级")]
        public LayerMask targetLayer;
        [Tooltip("检测距离限制")]
        public float maxDistance = 100f;
        
        [Header("坐标输出")]
        [SerializeField] private Vector3 hitPoint;
        [SerializeField] private float hitDistance;
        
        private Camera _mainCamera;

        private void Start()
        {
            Debug.Log(RDNAManager.Instance);//todo:当完成商店的对接后删除此行
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            HandleMouseInteraction();
        }

        private void HandleMouseInteraction()
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.GetRayIntersection(ray, maxDistance, targetLayer);

            if (Debugger.IsDebugging)
            {
                Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);
            }

            if (hit.collider)
            {
                var targetObject = hit.collider.gameObject;
                
                hitPoint = hit.point;
                
                var cameraPos = _mainCamera.transform.position;
                var hit3DPos = new Vector3(hitPoint.x, hitPoint.y, targetObject.transform.position.z);
                hitDistance = Vector3.Distance(cameraPos, hit3DPos);

                var spriteRenderer = targetObject.GetComponent<SpriteRenderer>();
                if (!spriteRenderer) return;
                HandleSpriteDetection(spriteRenderer);
                if (Debugger.IsDebugging)
                {
                    PrintDetectionInfo();
                }
                
            }
            else
            {
                ClearDetectionData();
            }
        }

        private void PrintDetectionInfo()
        {
            Debug.Log($"击中坐标：{hitPoint.ToString("F2")}\n" +
                      $"实际距离：{hitDistance.ToString("F2")}米");
        }

        private void ClearDetectionData()
        {
            hitPoint = Vector2.zero;
            hitDistance = 0f;
        }

        private void HandleSpriteDetection(SpriteRenderer detectedSprite)
        {
            if (Debugger.IsDebugging)
            {
                Debug.Log($"检测到Sprite: {detectedSprite.name}", detectedSprite.gameObject);
                detectedSprite.color = Color.yellow;
            }
            if (!Input.GetMouseButtonDown(0)) return;
            EventManager.Instance.TriggerEvent("MouseLeftClick",hitPoint);
            if (Debugger.IsDebugging)
            {
                Debug.Log($"点击了Sprite: {detectedSprite.name}");
            }
            
        }
    }
}
