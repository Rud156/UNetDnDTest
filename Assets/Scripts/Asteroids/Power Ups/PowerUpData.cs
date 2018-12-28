using UnityEngine.Networking;
using UNetUI.Asteroids.Enums;

namespace UNetUI.Asteroids.Power_Ups
{
    public class PowerUpData : NetworkBehaviour
    {
        public PowerUpType powerUpType;
    }
}