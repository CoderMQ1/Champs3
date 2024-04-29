using System;
using DG.Tweening;
using UnityEngine;

namespace champs3.Hotfix.UI
{
    public class PopHud : MonoBehaviour
    {

        private float _duration;
        private Vector3 _targetWorldPosition;
        private RectTransform _rectTransform;
        private bool isStart;

        private GameObject _targetObj;
        
        public void Pop(Vector3 position, Vector3 targetPosition, float duration)
        {
            _rectTransform = GetComponent<RectTransform>();
            _duration = duration;
            _targetObj = new GameObject();
            _targetObj.transform.position = position;
            _targetObj.transform.DOMove(targetPosition, duration).onComplete = () =>
            {
                Destroy(this.gameObject);
            };
        }


        private void Update()
        {
            if (_duration > 0)
            {
                _duration -= Time.deltaTime;
                
                var position = _targetObj.transform.position;
				
                if (CameraHelper.IsVisableInCamera(Global.Instance.MainCamera, position))
                {
                    var width = Screen.width;
                    var height = Screen.height;
                    var rect = transform.parent.GetComponent<RectTransform>().rect;

                    var rectWidth = rect.width;
                    var rectHeight = rect.height;
                    var screenPoint = Global.Instance.MainCamera.WorldToScreenPoint(position);
                    Vector3 pos = new Vector3(screenPoint.x  / width * rectWidth, screenPoint.y * rectHeight / height);
                    _rectTransform.anchoredPosition = pos;
                }
            }
        }

        private void OnDestroy()
        {
            Destroy(_targetObj);
        }
    }
}