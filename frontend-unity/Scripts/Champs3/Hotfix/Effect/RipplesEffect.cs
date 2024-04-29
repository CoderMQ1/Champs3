using System.Reflection;
using UnityEngine;

namespace champs3.Hotfix.Effect
{
    public class RipplesEffect : AbstractParticleEffect
    {
        public Transform Binder
        {
            get { return _Binder; }
        }
        
        protected Transform _Binder;

        public RipplesEffect(string assetLocation, Transform parent, Vector3 position = default, Quaternion rotate = default) : base(assetLocation, parent, position, rotate)
        {
        }

        public RipplesEffect(string assetLocation, Vector3 position, Quaternion rotate) : base(assetLocation, position, rotate)
        {
        }


        public void BindTran(Transform transform)
        {
            _Binder = transform;
        }

        public override void OnEnable()
        {
            var position = Particle.transform.localPosition;
            Particle.transform.localPosition = new Vector3(position.x, 0, position.z);
        }

        public override void OnDisable()
        {
        }

        public override void OnDestroy()
        {
        }

        public override void OnUpdate(float deltaTime)
        {
            if (Particle != null)
            {
                Vector3 position = Particle.transform.position;
                Particle.transform.position = new Vector3(_Binder.position.x, position.y, _Binder.position.z);
            }

        }
    }
}