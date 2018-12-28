using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Asteroids.Shared;

namespace UNetUI.Asteroids.Enemies.Spaceship
{
    [RequireComponent(typeof(HealthSetter))]
    [RequireComponent(typeof(ScoreSetter))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class SpaceshipDestroy : NetworkBehaviour
    {
        public float animationTime;

        private HealthSetter _healthSetter;
        private Animator _shipAnimator;

        private static readonly int Dead = Animator.StringToHash("Dead");

        private void Start()
        {
            _healthSetter = GetComponent<HealthSetter>();
            _healthSetter.healthZero += DestroySpaceship;

            _shipAnimator = GetComponent<Animator>();
        }

        private void DestroySpaceship()
        {
            if (!isServer)
                return;

            _shipAnimator.SetBool(Dead, true);
            Invoke(nameof(SanityDestroyCheck), 1.2f * animationTime);
            
            RpcDisplayDamageOnClients(true);
        }

        [ClientRpc]
        private void RpcDisplayDamageOnClients(bool playDeadAnimation)
        {
            if(isServer)
                return;
            
            _shipAnimator.SetBool(Dead, playDeadAnimation);
            Invoke(nameof(RemoveSpaceship), animationTime);
        }

        private void RemoveSpaceship() => Destroy(gameObject);

        private void SanityDestroyCheck() => NetworkServer.Destroy(gameObject);
    }
}