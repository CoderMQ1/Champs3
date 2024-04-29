// 
// 2023/12/04

using System;
using QFramework;
using UnityEngine;

namespace champs3.Hotfix.Player
{
    public class CharacterAnimatorController : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        #endregion

        #region FIELDS

        public Animator _animator;


        private int _animIdSpeed;
        private int _animIdMoveType;
        private int _animIdProp;
        private int _animIdMotionSpeed;
        #endregion

        #region PROPERTIES

        public Animator Animator
        {
            get { return _animator; }
            set
            {
                _animator = value;
                _hasAnimator = _animator;
            }
        }

        private bool _hasAnimator;

        #endregion
        #region EVENT FUNCTION

        private void Awake()
        {
            
        }
        
        
        

        #endregion

        #region METHODS

        public void Initialize()
        {
            RegisteAnimationId();
        }


        public void RegisteAnimationId()
        {
            _animIdSpeed = Animator.StringToHash("Speed");
            _animIdMoveType = Animator.StringToHash("MoveType");
            _animIdProp = Animator.StringToHash("Prop");
            _animIdMotionSpeed = Animator.StringToHash("MotionSpeed");
        }


        public void SetSpeed(float speed)
        {
            if (_hasAnimator)
                _animator.SetFloat(_animIdSpeed, speed);
        }

        public void SetMoveType(ActionType actionType)
        {
            if (actionType == ActionType.Swiming)
            {
                LogKit.I("Set Player Swim");
            }
            if (_hasAnimator)
                _animator.SetInteger(_animIdMoveType, (int)actionType);
        }

        public void AppleRootMotion(bool root)
        {
            if (_hasAnimator)
                _animator.applyRootMotion = root;
        }

        public void SetProp(bool isUsing)
        {
            if (_hasAnimator)
                _animator.SetBool(_animIdProp, isUsing);
        }

        public void PlayAnimation(string state)
        {
            if (_hasAnimator)
            {
                _animator.Play(state);
            }
        }

        public void SetMotionSeed(float motionSpeed)
        {
            if (_hasAnimator)
                _animator.SetFloat(_animIdMotionSpeed, motionSpeed); 
        }


        #endregion


    }



}