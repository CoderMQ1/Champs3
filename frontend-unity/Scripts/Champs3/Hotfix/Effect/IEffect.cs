// 
// 2023/12/08

using UnityEngine.PlayerLoop;

namespace champs3.Hotfix.Effect
{
    public interface IEffect
    {
        void Enable();
        void Disable();
        void Destroy();

        void Update(float deltaTime);
    }
}