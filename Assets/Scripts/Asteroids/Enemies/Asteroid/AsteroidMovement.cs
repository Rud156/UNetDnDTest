using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Asteroids.Shared;

namespace UNetUI.Asteroids.Enemies.Asteroid
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class AsteroidMovement : NetworkBehaviour
	{
		public float launchVelocity;
		public float rotationSpeed;

		private Rigidbody2D _asteroidRb;

		private void Start()
		{
			_asteroidRb = GetComponent<Rigidbody2D>();

			int launchAngle = Random.Range(0, 360);
			Vector2 launchVector = new Vector2(
				Mathf.Cos(launchAngle * Mathf.Deg2Rad) * launchVelocity,
				Mathf.Sin(launchAngle * Mathf.Deg2Rad) * launchVelocity
				);

			_asteroidRb.AddForce(launchVector);
		}

		private void Update()
		{
			ServerUpdate();
			RemoteClientUpdate();
		}

		private void ServerUpdate()
		{
			if(!isServer)
				return;
			
			ScreenWrapper.instance.CheckObjectOutOfScreen(transform);
			transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
		}

		private void RemoteClientUpdate()
		{
			
		}
	}
}
