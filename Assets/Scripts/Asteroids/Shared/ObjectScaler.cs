using UnityEngine;

namespace UNetUI.Asteroids.Shared
{
    public class ObjectScaler : MonoBehaviour
    {
        public Vector2 refResolution;
        public Vector2 refScale;

        private void Start()
        {
            if(refResolution.x == 0 || refResolution.y == 0)
                return;
            
            float refRatio = refResolution.x / refResolution.y;
            float currentRatio = (float) Screen.width / Screen.height;

            Vector2 updatedScale = refScale / refRatio * currentRatio;
            transform.localScale = updatedScale;
        }
    }
}