using UnityEngine;
using UnityEngine.Experimental.UIElements.StyleEnums;
using UnityEngine.Networking;

namespace UNetUI.Asteroids.Shared
{
    public class ScreenWrapper : NetworkBehaviour
    {
        [HideInInspector] public float leftMostPoint;
        [HideInInspector] public float rightMostPoint;
        [HideInInspector] public float topMostPoint;
        [HideInInspector] public float bottomMostPoint;

        private bool _isPointsSet;

        private void Update()
        {
            if (isLocalPlayer && !_isPointsSet)
            {
                Vector3 topLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
                Vector3 bottomRight =
                    Camera.main.ScreenToWorldPoint(new Vector2(Camera.main.pixelWidth, Camera.main.pixelHeight));

                leftMostPoint = topLeft.x;
                topMostPoint = topLeft.y;
                rightMostPoint = bottomRight.x;
                bottomMostPoint = bottomRight.y;

                Debug.Log("Setting Values");
                Debug.Log($"Left: {leftMostPoint}, Right: {rightMostPoint}");
                Debug.Log($"Top: {topMostPoint}, Bottom: {bottomMostPoint}");

                _isPointsSet = true;
            }
        }

        public void CheckObjectOutOfScreen(float leftPoint, float rightPoint, float topPoint, float bottomPoint)
        {
            Vector3 position = transform.position;
            Vector3 newPosition = position;

            if (leftPoint - 2 > position.x)
                newPosition.x = rightPoint + 1;
            else if (rightPoint + 2 < position.x)
                newPosition.x = leftPoint - 1;

            if (bottomPoint + 2 < position.y)
                newPosition.y = topPoint - 1;
            else if (topPoint - 2 > position.y)
                newPosition.y = bottomPoint + 1;

            transform.position = newPosition;
        }
    }
}