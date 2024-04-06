using UnityEngine;

namespace SquareHero.Hotfix
{
    public class CameraHelper
    {
        public static bool IsVisableInCamera(Camera camera, Vector3 position)
        {

            Vector3 viewPos = camera.WorldToViewportPoint(position);
            
            if (viewPos.z < 0)
            {
                return false;
            }

            if (viewPos.z > camera.farClipPlane)
            {
                return false;
            }

            if (viewPos.x < 0 || viewPos.y < 0 || viewPos.x > 1 || viewPos.y > 1)
            {
                return false;
            }

            return true;
        }
    }
}