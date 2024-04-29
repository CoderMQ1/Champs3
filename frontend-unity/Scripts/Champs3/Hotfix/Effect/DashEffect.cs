// 
// 2023/12/08

using UnityEngine;

namespace champs3.Hotfix.Effect
{
    public class DashEffect : AbstractParticleEffect
    {
        
        public DashEffect(string assetLocation, Transform parent, Vector3 position = default, Quaternion rotate = default) : base(assetLocation, parent, position, rotate)
        {
        }

        public DashEffect(string assetLocation, Vector3 position, Quaternion rotate) : base(assetLocation, position, rotate)
        {
        }

        public override void OnEnable()
        {
            
        }

        public override void OnDisable()
        {
        }

        public override void OnDestroy()
        {
        }

        public override void OnUpdate(float deltaTime)
        {
        }
    }
}