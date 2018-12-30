using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UNetUI.Asteroids.Power_Ups
{
    public class PowerUpGetter : MonoBehaviour
    {
        #region Singleton

        public static PowerUpGetter instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        public List<PowerUpData> powerUps;

        public Sprite GetPowerUpImageByName(string powerUpName) =>
            powerUps.FirstOrDefault(_ => _.powerUpName == powerUpName)?.powerUpImage;

        public PowerUpData GetPowerUpByName(string powerUpName) =>
            powerUps.FirstOrDefault(_ => _.powerUpName == powerUpName);
    }
}