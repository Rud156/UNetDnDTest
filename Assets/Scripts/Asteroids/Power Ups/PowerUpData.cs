using UnityEngine;
using UNetUI.Asteroids.Enums;

namespace UNetUI.Asteroids.Power_Ups
{
    [CreateAssetMenu(menuName = "PowerUp", fileName = "PowerUp")]
    public class PowerUpData : ScriptableObject
    {
        [Header("Basic Stats")] public string powerUpName;
        public PowerUpType powerUpType;
        public float powerUpAffectTime;
        public Sprite powerUpImage;

        [Header("Optional Requirements")] public GameObject powerUpPrefab;

        [Header("Bullet Increase Damage")] public float damageAmount;
    }
}