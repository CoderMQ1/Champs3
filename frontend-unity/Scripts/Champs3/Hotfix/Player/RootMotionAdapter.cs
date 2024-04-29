// 
// 2023/12/05

using System;
using UnityEngine;

namespace champs3.Hotfix.Player
{
    public class RootMotionAdapter : MonoBehaviour
    {
        #region EDITOR EXPOSED FIELDS

        public Transform Root;
        
        #endregion

        #region FIELDS

        private Animator _animator;
        #endregion

        #region PROPERTIES

        #endregion

        #region EVENT FUNCTION

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _animator.applyRootMotion = true;
        }

        private void OnAnimatorMove()
        {
            if (Root)
            {
                Root.position = _animator.rootPosition;
                Root.rotation = _animator.rootRotation;
            }

        }
        

        #endregion

        #region METHODS
        
        #endregion


    }

}