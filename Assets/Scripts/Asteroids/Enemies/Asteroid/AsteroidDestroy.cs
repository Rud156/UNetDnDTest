﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UNetUI.Asteroids.Shared;

namespace UNetUI.Asteroids.Enemies.Asteroid
{
	[RequireComponent(typeof(HealthSetter))]
	public class AsteroidDestroy : MonoBehaviour
	{
		public bool spawnMiniAsteroid;
		public GameObject miniAsteroid;
		public int miniAsteroidCount;

		private HealthSetter _healthSetter;

		// Use this for initialization
		private void Start()
		{
			_healthSetter = GetComponent<HealthSetter>();
			_healthSetter.healthZero += DestroyAsteroid;
		}

		private void DestroyAsteroid()
		{
			if (spawnMiniAsteroid)
			{
				for (int i = 0; i < miniAsteroidCount; i++)
					Instantiate(miniAsteroid, transform.position, Quaternion.Euler(0, 0, Random.value * 360));
			}
			
			Destroy(gameObject);
		}
	}
}