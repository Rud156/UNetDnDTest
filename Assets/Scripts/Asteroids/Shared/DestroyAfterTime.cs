using UnityEngine;
using UnityEngine.Networking;

namespace UNetUI.Asteroids.Shared
{
    public class DestroyAfterTime : MonoBehaviour
    {
        public float destroyTime = 5;

        private void Start() => Invoke(nameof(DestroyGameObject), destroyTime);

        private void DestroyGameObject() => NetworkServer.Destroy(gameObject);
    }
}