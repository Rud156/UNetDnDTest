using UnityEngine;

namespace UNetUI.Asteroids.Shared
{
    public class DestroyAfterTime : MonoBehaviour
    {
        public float destroyTime = 5;

        private void Start() => Destroy(gameObject, destroyTime);
    }
}