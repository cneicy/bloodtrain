using System;
using Handler;
using Manager;
using UnityEngine;

namespace Entity.Base
{
    public abstract class AttackOrganBase : AmmoShooter
    {
        [SerializeField] public GameObject cannon;
        [SerializeField] public GameObject cannonBase;
        [SerializeField] public float radius;
        [SerializeField] private LayerMask targetLayer;
        [SerializeField] private bool isPassive;
        private GameObject _temp;
        private Camera _camera;
        private CameraManager _cameraManager;

        private void Awake()
        {
            _camera = Camera.main;
            _cameraManager = _camera.GetComponent<CameraManager>();
        }

        public void FixedUpdate()
        {
            _temp = FindNearestObject();
            if (_temp && isPassive)
                cannon.transform.LookAt(_temp.transform);
            if (_temp && !isPassive && _cameraManager.firstPerson.Priority > _cameraManager.freeLook.Priority)
            {
                cannon.transform.eulerAngles = _camera.transform.eulerAngles;
            }
        }

        public GameObject FindNearestObject()
        {
            var hitColliders = Physics.OverlapSphere(transform.position, radius, targetLayer);
            GameObject nearestObject = null;
            var minSqrDistance = Mathf.Infinity;

            foreach (var collider1 in hitColliders)
            {
                var offset = collider1.transform.position - transform.position;
                var sqrDistance = offset.sqrMagnitude;

                if (!(sqrDistance < minSqrDistance)) continue;
                minSqrDistance = sqrDistance;
                nearestObject = collider1.gameObject;
            }

            if (nearestObject && Debugger.IsDebugging)
                Debug.Log(nearestObject.transform.position);
            return nearestObject;
        }
    }
}