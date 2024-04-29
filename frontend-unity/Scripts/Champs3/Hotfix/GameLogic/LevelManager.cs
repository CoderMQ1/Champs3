using System;
using QFramework;
using champs3.Hotfix.GameLogic.Level1;
using UnityEngine;

namespace champs3.Hotfix.GameLogic
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        #region EDITOR EXPOSED FIELDS
        #endregion

        #region FIELDS
        private LevelController _currentLevelController;

        #endregion

        #region PROPERTIES

        public LevelController CurrentLevelController {
            get
            {
                return _currentLevelController;
            }
        }

        #endregion

        #region EVENT FUNCTION

        private void Awake()
        {
            mInstance = this;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            mInstance = null;
        }

        #endregion

        #region METHODS

        public void StartLevel(LevelController levelController)
        {
            _currentLevelController = levelController;
        }
        
        

        #endregion

        
    }
}