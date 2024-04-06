// 
// 2023/12/08

using UnityEngine.PlayerLoop;

namespace SquareHero.Hotfix.Effect
{
    public interface IEffect
    {
        void Enable();
        void Disable();
        void Destroy();

        void Update(float deltaTime);
    }
}