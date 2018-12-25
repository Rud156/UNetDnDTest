using UnityEngine;

namespace UNetUI.Asteroids.Shared
{
    public class ScreenWrapper : MonoBehaviour
    {
        #region Singleton

        public static ScreenWrapper instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            
            if(instance != this)
                Destroy(gameObject);
        }

        #endregion Singleton
        
        [SerializeField][Range(0, 1)]
        private float incrementAmount = 0.3f;
        
        private float _leftMostPoint;
        private float _rightMostPoint;
        private float _topMostPoint;
        private float _bottomMostPoint;

        public float BottomMostPoint => _bottomMostPoint - incrementAmount;
        public float TopMostPoint => _topMostPoint + incrementAmount;
        public float LeftMostPoint => _leftMostPoint - incrementAmount;
        public float RightMostPoint => _rightMostPoint + incrementAmount;

        private bool _isPointsSet;

        private void Update()
        {
            if (!_isPointsSet)
            {
                Camera mainCamera = Camera.main;

                Vector3 topLeft = mainCamera.ScreenToWorldPoint(new Vector2(0, mainCamera.pixelHeight));
                Vector3 bottomRight =
                    mainCamera.ScreenToWorldPoint(new Vector2(mainCamera.pixelWidth, 0));

                _leftMostPoint = topLeft.x;
                _topMostPoint = topLeft.y;
                _rightMostPoint = bottomRight.x;
                _bottomMostPoint = bottomRight.y;

                _isPointsSet = true;
            }
        }

        public void CheckObjectOutOfScreen(Transform refTransform)
        {
            Vector3 position = refTransform.position;
            Vector3 newPosition = position;

            if (_leftMostPoint - 2 > position.x)
                newPosition.x = RightMostPoint;
            else if (_rightMostPoint + 2 < position.x)
                newPosition.x = LeftMostPoint;

            if (_bottomMostPoint - 2 > position.y)
                newPosition.y = TopMostPoint;
            else if (_topMostPoint + 2 < position.y)
                newPosition.y = BottomMostPoint;

            refTransform.position = newPosition;
        }
    }
}