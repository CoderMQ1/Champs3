// 
// 2023/12/08

using QFramework;
using SquareHero.Hotfix.Effect;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace SquareHero.Hotfix
{
    public abstract class AbstractParticleEffect : IEffect
    {
        public ParticleSystem Particle;
        protected string AssetLocation;
        protected Transform Parent;
        protected Vector3 Position;
        protected Quaternion Rotate;
        protected bool EnbaleOnLoad;
        protected bool IsLoaded;
        protected bool IsDestroyed;
        

        public AbstractParticleEffect(string assetLocation, Transform parent, Vector3 position = default,
            Quaternion rotate = default)
        {
            AssetLocation = assetLocation;
            Parent = parent;
            Position = position;
            Rotate = rotate;
            Load(AssetLocation);
        }


        public AbstractParticleEffect(string assetLocation, Vector3 position, Quaternion rotate)
        {
            AssetLocation = assetLocation;
            Position = position;
            Rotate = rotate;
            Load(AssetLocation);
        }


        protected void Load(string location)
        {
            AssetLocation = location;
            ResourceManager.Instance.GetAssetAsync<GameObject>(location, gameObject =>
            {
                
                IsLoaded = true;
                var instantiate = GameObject.Instantiate(gameObject, Parent);
                instantiate.transform.localPosition = Position;
                instantiate.transform.localRotation = Rotate;
                Particle = instantiate.GetComponent<ParticleSystem>();
                Particle.gameObject.SetActive(false);

                if (EnbaleOnLoad)
                {
                    EnableInternal();
                }
            });
        }

        protected void EnableInternal()
        {
            EnbaleOnLoad = true;
            Particle.gameObject.SetActive(true);
            Particle.Play();
            OnEnable();
        }


        public void Enable()
        {
            if (IsLoaded)
            {
                EnableInternal();
            }

            else
            {
                EnbaleOnLoad = true;
            }
        }

        public void Disable()
        {
            if (IsLoaded)
            {
                Particle.gameObject.SetActive(false);
                OnDisable();
            }
            else
            {
                EnbaleOnLoad = false;
            }
        }

        public void Destroy()
        {
            if (!IsDestroyed)
            {
                Particle.gameObject.DestroySelf();
                // ResourceManager.Instance.UnLoadAsset(AssetLocation);
                OnDestroy();
                IsDestroyed = true;
            }
        }

        public void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        public abstract void OnEnable();

        public abstract void OnDisable();

        public abstract void OnDestroy();

        public abstract void OnUpdate(float deltaTime);
    }
}