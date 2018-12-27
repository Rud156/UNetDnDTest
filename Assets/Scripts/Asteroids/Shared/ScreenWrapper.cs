using UnityEngine;

namespace UNetUI.Asteroids.Shared
{
    public class ScreenWrapper : MonoBehaviour
    {
        [SerializeField] [Range(0, 5)] private float incrementAmount = 0.3f;
        [SerializeField] private float screenOffsetRange;

        private float _leftMostPoint;
        private float _rightMostPoint;
        private float _topMostPoint;
        private float _bottomMostPoint;

        public float BottomMostPoint => _bottomMostPoint - incrementAmount;
        public float TopMostPoint => _topMostPoint + incrementAmount;
        public float LeftMostPoint => _leftMostPoint - incrementAmount;
        public float RightMostPoint => _rightMostPoint + incrementAmount;

        private void Start()
        {
            Camera mainCamera = Camera.main;

            Vector3 topLeft = mainCamera.ScreenToWorldPoint(new Vector2(0, mainCamera.pixelHeight));
            Vector3 bottomRight =
                mainCamera.ScreenToWorldPoint(new Vector2(mainCamera.pixelWidth, 0));

            _leftMostPoint = topLeft.x;
            _topMostPoint = topLeft.y;
            _rightMostPoint = bottomRight.x;
            _bottomMostPoint = bottomRight.y;
        }

        public void CheckObjectOutOfScreen()
        {
            Vector3 position = transform.position;
            Vector3 newPosition = position;

            if (_leftMostPoint - screenOffsetRange > position.x)
                newPosition.x = RightMostPoint;
            else if (_rightMostPoint + screenOffsetRange < position.x)
                newPosition.x = LeftMostPoint;

            if (_bottomMostPoint - screenOffsetRange > position.y)
                newPosition.y = TopMostPoint;
            else if (_topMostPoint + screenOffsetRange < position.y)
                newPosition.y = BottomMostPoint;

            transform.position = newPosition;
        }
    }
}