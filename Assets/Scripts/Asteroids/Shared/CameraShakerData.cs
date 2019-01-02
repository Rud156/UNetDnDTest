using UnityEngine;
using UnityEngine.Networking;

namespace UNetUI.Asteroids.Shared
{
    [CreateAssetMenu(menuName = "Camera/Shaker", fileName = "CameraShaker")]
    public class CameraShakerData : ScriptableObject
    {
        public float magnitude;
        public float roughness;
        public float fadeInTime;
        public float fadeOutTime;
    }
}