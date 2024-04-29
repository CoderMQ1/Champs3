using System;
using DG.Tweening;
using QFramework;
using champs3.Hotfix.Events;
using champs3.Hotfix.Model;
using champs3.Hotfix.Player;
using UnityEngine;

namespace champs3.Hotfix.Map
{
    public class Gift : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        public Transform Render;

        public ParticleSystem RenderEffect;
        
        public ParticleSystem DisableEffect;

        public GiftModel GiftModel
        {
            get { return _model; }
            set
            {
                SetMode(value);
                
            }
        }

        public readonly float[] TrackX = new[] {-1.7f, -0.8f, 0 , 0.8f, 1.7f };

        private GiftModel _model;
        private Sequence _rotateSequence;
        private Sequence _moveSequence;
        private Vector3 _startPosition;
        private bool _canTrigger = true;
        #endregion

        #region FIELDS

        #endregion

        #region PROPERTIES

        #endregion

        #region EVENT FUNCTION

        private void Start()
        {
            _startPosition = transform.localPosition;
        }

        private void OnTriggerEnter(Collider other)
        {
            this.Log($"OnTriggerEnter {other.name}");
            if (other.CompareTag("Player"))
            {
                if (!_canTrigger)
                {
                    return;
                }
                
                var shPlayerController = other.gameObject.GetComponent<SHPlayerController>();

                if (shPlayerController.Data.Track - 1 != _model.Track)
                {
                    LogKit.E($"player [{shPlayerController.name}] track {shPlayerController.Data.Track} not same with {transform.parent.parent.name}.{name} {_model.Track}");
                    return;
                }

                _canTrigger = false;
                Render.gameObject.SetActive(false);
                RenderEffect.gameObject.SetActive(false);
                DisableEffect.gameObject.SetActive(true);
                DisableEffect.Play();

                // int coin = 0;
                //
                // if (_model.RewardType == RewardType.Lv1)
                // {
                //     coin = 99;
                // }
                //
                // if (_model.RewardType == RewardType.Lv2)
                // {
                //     coin = 19;
                // }
                //
                // if (_model.RewardType == RewardType.Lv3)
                // {
                //     coin = 9;
                // }
                //
                TypeEventSystem.Global.Send(new PlayerEvents.GetCoin()
                {
                    UserId = shPlayerController.Data.UserId,
                    Coin = _model.CoinCount
                });
                AudioKit.PlaySound(SoundName.CollideGift.ToString());
            }
        }

        private void OnDestroy()
        {
            _rotateSequence.Kill();
        }

        #endregion

        #region METHODS


        private void SetMode(GiftModel model)
        {
            _model = model;

            if (_rotateSequence != null)
            {
                _rotateSequence.Kill();
            }

            if (_moveSequence != null)
            {
                _moveSequence.Kill();
            }


            transform.localPosition = new Vector3(TrackX[model.Track], _startPosition.y, _startPosition.z);;
            var pos = Render.transform.position;
            DOTween.Init();
            
            _rotateSequence = DOTween.Sequence();
            _rotateSequence.Append(Render.transform.DOLocalRotate(new Vector3(0, 180, 0), 5, RotateMode.FastBeyond360).SetEase(Ease.Linear));
            _rotateSequence.SetLoops(-1, LoopType.Incremental);
            _rotateSequence.Play();

            _moveSequence = DOTween.Sequence();

            _moveSequence.Append(Render.transform.DOMoveY(pos.y + 0.2f, 1f).SetEase(Ease.Linear));
            _moveSequence.SetLoops(-1, LoopType.Yoyo);
            _moveSequence.Play();
        }

        #endregion


    }
}