using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UNetUI.Asteroids.UI
{
    [RequireComponent(typeof(RawImage))]
    public class ScreenFlasher : NetworkBehaviour
    {
        #region Singleton

        public static ScreenFlasher instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        public int flashTimes;

        private Color _flashColor;
        private RawImage _image;

        private void Start()
        {
            _image = GetComponent<RawImage>();
            _image.enabled = false;
        }

        [ClientRpc]
        public void RpcSetColorAndFlash(Color color)
        {
            if (isServer)
                return;

            _flashColor = color;
            StartFlash();
        }

        private void StartFlash() => StartCoroutine(Flash());

        private IEnumerator Flash()
        {
            _image.color = _flashColor;

            for (int i = 0; i < flashTimes; i++)
            {
                _image.enabled = true;
                yield return new WaitForSeconds(0.1f);

                _image.enabled = false;
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}