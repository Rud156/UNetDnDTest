using UnityEngine;
using UnityEngine.Networking;
using UNetUI.Asteroids.Shared;
using UNetUI.Extras;

namespace UNetUI.Asteroids.Shared
{
    [RequireComponent(typeof(ScreenWrapper))]
    public class BulletController : NetworkBehaviour
    {
        private ScreenWrapper _screenWrapper;

        private void Start() => _screenWrapper = GetComponent<ScreenWrapper>();

        private void Update()
        {
            if(!isServer)
                return;
            
            Debug.Log("Checking Positions");
            
            _screenWrapper.CheckObjectOutOfScreen();
            
            Vector3 position = transform.position;

            float percentX = ExtensionFunctions.Map(position.x, _screenWrapper.LeftMostPoint,
                _screenWrapper.RightMostPoint, -1, 1);
            float percentY = ExtensionFunctions.Map(position.y, _screenWrapper.TopMostPoint,
                _screenWrapper.BottomMostPoint, 1, -1);
            
            RpcSendPositionToClients(percentX, percentY);
        }

        [ClientRpc]
        private void RpcSendPositionToClients(float percentX, float percentY)
        {
            if (isServer)
                return;

            transform.position = new Vector2(
                ExtensionFunctions.Map(percentX, -1, 1,
                    _screenWrapper.LeftMostPoint, _screenWrapper.RightMostPoint),
                ExtensionFunctions.Map(percentY, 1, -1,
                    _screenWrapper.TopMostPoint, _screenWrapper.BottomMostPoint)
            );
        }
    }
}