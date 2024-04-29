using System;
using System.Collections.Generic;
using DG.Tweening;
using QFramework;
using TMPro;
using UnityEngine;

namespace champs3.Hotfix.Toast
{
    public class ToastManager : MonoSingleton<ToastManager>, IObjectFactory<Toast>
    {
        #region EDITOR EXPOSED FIELDS

        private ToastPanel _toastPanel;

        #endregion

        #region FIELDS

        private SimpleObjectPool<Toast> _toastPool;
        public Dictionary<string, Toast> PlayingToast = new Dictionary<string, Toast>();

        #endregion

        #region PROPERTIES

        #endregion

        #region EVENT FUNCTION

        public ToastManager()
        {
            mInstance = this;
        }

        private void Awake()
        {
            _toastPanel = GetComponent<ToastPanel>();
            _toastPool = new SimpleObjectPool<Toast>(Create, ResetToast, 0);
        }

        #endregion

        #region METHODS

        public void ToastMsg(string msg)
        {

            if (!PlayingToast.ContainsKey(msg))
            {
                var toast = _toastPool.Allocate();
                PlayingToast.Add(msg, toast);
                toast.Msg = msg;
                toast.ToastItem.Msg.text = msg;
                toast.Sequence.Play();
                var sequence = DOTween.Sequence();
                sequence.InsertCallback(0, () =>
                {
                    toast.ToastItem.Msg.text = toast.Msg;
                    toast.ToastItem.Root.anchoredPosition = Vector2.zero;
                    toast.ToastItem.Views.alpha = 1;
                    toast.ToastItem.Root.gameObject.SetActive(true);
                });
                sequence.Append(toast.ToastItem.Root.DOAnchorPosY(600, 0.4f));
                sequence.AppendInterval(0.2f);
                sequence.Append(toast.ToastItem.Views.DOFade(0, 0.2f));
                sequence.AppendCallback(() =>
                {
                    toast.ToastItem.Root.gameObject.SetActive(false);
                    _toastPool.Recycle(toast);
                    PlayingToast.Remove(msg);
                    LogKit.I("Recycle");
                });
                toast.Sequence = sequence;
            }
        }

        #endregion

        public Toast Create()
        {
            var toast = new Toast();
            var instantiate = Instantiate(_toastPanel.Toast, _toastPanel.Toast.transform.parent);
            var toastItem = instantiate.GetComponent<ToastItem>();
            toast.ToastItem = toastItem;
            

            return toast;
        }

        public void  ResetToast(Toast toast)
        {
            
        }
    }

    public struct Toast
    {
        public string Msg { get; set; }
        public ToastItem ToastItem { get; set; }
        public Sequence Sequence { get; set; }
        public float Time { get; set; }
    }
}