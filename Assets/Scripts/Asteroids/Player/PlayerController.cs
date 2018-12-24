using UnityEngine;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Velocities")] public float movementSpeed;
        public float rotationSpeed;

        private Rigidbody _playerRb;
        private Animator _playerAnim;
        private float _roll;

        private void Start()
        {
            _playerRb = GetComponent<Rigidbody>();
            _playerAnim = GetComponent<Animator>();

            _roll = transform.rotation.eulerAngles.z;
        }

        private void Update()
        {
            RotatePlayer();
            MovePlayerForwardBackward();
        }

        private void RotatePlayer()
        {
            float moveH = Input.GetAxis(Controls.PlayerHM);

            _roll += -moveH * rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Euler(0, 0, _roll);
        }

        private void MovePlayerForwardBackward()
        {
            float moveZ = Input.GetAxis(Controls.PlayerVM);

            if (moveZ > 0)
            {
                Vector3 velocityV = transform.up * moveZ * movementSpeed * Time.deltaTime;
                _playerRb.AddForce(velocityV, ForceMode.VelocityChange);

                _playerAnim.SetBool(PlayerConstantData.MovementAnimParam, true);
            }
            else
                _playerAnim.SetBool(PlayerConstantData.MovementAnimParam, false);
        }
    }
}