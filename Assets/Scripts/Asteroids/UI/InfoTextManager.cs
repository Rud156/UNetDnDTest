using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UNetUI.Asteroids.UI
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Text))]
    public class InfoTextManager : NetworkBehaviour
    {
        #region Singleton

        public static InfoTextManager instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            if (instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton

        private Animator infoTextAnimator;
        private Text infoText;

        private void Start()
        {
            infoTextAnimator = GetComponent<Animator>();
            infoText = GetComponent<Text>();
        }

        private const string FadeInOutAnimParam = "Display Text";

        [ClientRpc]
        public void RpcDisplayText(string text, Color textColor, bool useLocalPlayer)
        {
            if (useLocalPlayer && !isLocalPlayer)
                return;

            if (isServer)
                return;

            infoText.text = text;
            infoText.color = textColor;

            infoTextAnimator.SetTrigger(FadeInOutAnimParam);
        }
    }
}