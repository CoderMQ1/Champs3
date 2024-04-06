using System.Collections.Generic;
using QFramework;
using SquareHero.Hotfix.Generate;
using SquareHero.Hotfix.Map;
using UnityEngine;

namespace SquareHero.Hotfix
{
    public abstract class AbstractProp : IProp
    {
        public PropsConfig Config
        {
            get { return _config; }
        }

        public AssetConfig AssetConfig
        {
            get { return _assetConfig; }
        }

        private PropsConfig _config;
        private AssetConfig _assetConfig;

        protected GameObject User;
        //作用者
        protected GameObject Affecter;
        //已使用次数
        protected int UsedTimes;
        //可使用次数
        protected int UseTimes;
        protected float LeftDistance;
        protected List<GameObject> PropObjs;
        protected bool _isUsing;


        public OnUseCallback OnUseHandler;
        
        public delegate void OnUseCallback(int leftTimes);
        protected AbstractProp(long propsId)
        {
            _config = ExcelConfig.PropConfigTable.Data.Find(config =>
            {
                return config.Id == propsId;
            });
            if (_config != null)
            {
                _assetConfig = ExcelConfig.AssetConfigTable.Data.Find(config =>
                {
                    return config.Id == _config.AssetId;
                });
                UseTimes = _config.UsageTimes;
            }

            if (_config == null)
            {
                LogKit.E($"Not Find PropsConfig : {propsId}");
            }
            
            if (_assetConfig == null)
            {
                LogKit.E($"Not Find AssetConfig : {propsId}");
            }

           
        }

        public void Use(GameObject user)
        {
            Use(user, user);
        }
        
        public void Use(GameObject user, GameObject affecter)
        {
            if (CanUse())
            {
                User = user;
                Affecter = affecter;
                LeftDistance = _config.Distance;
                _isUsing = true;
                UsedTimes++;
                OnUseHandler?.Invoke(LeftUseTimes());
                OnUse();
            }
        }

        public float GetSpeedAddition()
        {
            return _config.SpeedIncrease / 100f;
        }

        public float GetLeftAffetDistance()
        {
            return LeftDistance;
        }

        public void Update(float distance)
        {
            if (LeftDistance > 0)
            {
                LeftDistance -= distance;
                
                
                if (LeftDistance <= 0)
                {
                    LeftDistance = 0;
                    Finish();
                }
            }
            OnUpdate(distance);
        }

        public void Finish()
        {
            if (_isUsing)
            {
                _isUsing = false;
                LeftDistance = 0;
                OnFinish();
            }

        }

        public bool CanUse(TileType moveType)
        {
            if (!IsValid())
            {
                return false;
            }
            
            int moveTypeValue = (int) moveType;

            for (int i = 0; i < Config.SpeedType.Length; i++)
            {
                if (Config.SpeedType[i] == moveTypeValue + 1)
                {
                    return true;
                }
            }

            return false;
        }

        public void BindGameObject(GameObject gameObject)
        {
            if (PropObjs == null)
            {
                PropObjs = new List<GameObject>();
            }
            
            PropObjs.Add(gameObject);
        }

        public bool IsValid()
        {
            if (_config == null || _assetConfig == null)
            {
                return false;
            }

            return true;
        }

        public bool CanUse()
        {   
            return (UseTimes == -1 || UsedTimes < UseTimes) && LeftDistance <= 0;
        }

        public int GetUseTimes()
        {
            return UseTimes;
        }

        public bool IsUsing()
        {
            return _isUsing;
        }

        public abstract int LeftUseTimes();
        protected abstract void OnUpdate(float distance);
        protected abstract void OnUse();

        protected abstract void OnFinish();
    }
}