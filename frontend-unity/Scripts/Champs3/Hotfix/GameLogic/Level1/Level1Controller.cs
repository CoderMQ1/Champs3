using System;
using QFramework;
using champs3.Hotfix.Map;
using champs3.Hotfix.Model;
using champs3.Hotfix.System;
using UnityEngine;

namespace champs3.Hotfix.GameLogic.Level1
{
    public class Level1Controller : LevelController        
    {
        #region EDITOR EXPOSED FIELDS

        #endregion

        #region FIELDS

        #endregion

        #region PROPERTIES

        #endregion

        #region EVENT FUNCTION

        private void Awake()
        {
        }

        private void Start()
        {
            LevelManager.Instance.StartLevel(this);
        }

        #endregion

        #region METHODS

        private void OnDestroy()
        {
            
        }

        #endregion


        public override IArchitecture GetArchitecture()
        {
            return Level1.Interface;
        }
    }
    
    public class Level1 : Architecture<Level1>
    {
        protected override void Init()
        {
            this.RegisterModel(new MapModel());
            this.RegisterModel(new PlayerModel());
            this.RegisterSystem(new CoinRankSystem());
        }
    }
}