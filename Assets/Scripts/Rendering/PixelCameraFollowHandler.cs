using Common.Rendering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Rendering
{
    public class PixelCameraFollowHandler : MonoBehaviour
    {
        // const
        private const float POSITION_EPSILON = 0.0001f;
        private const float ROTATION_EPSILON = 0.0001f;

        // public

        // protected

        // private
        [SerializeField] private float _cameraPositionSmoothTime = 0.1f;
        [SerializeField] private float _cameraRotationSmoothTime = 0.15f;
        [SerializeField] private int _roundingValue = 10;

        // TODO: For camera centering.
        private Transform _playerTransform = null;
        private Camera _camera = null;
        private Bounds _mapBounds;
        private Vector2 _cameraBoundedMaxPosition = Vector2.zero;
        private Vector2 _cameraBoundedMinPosition = Vector2.zero;

        private Vector2 _currentCameraPositionVelocity = Vector2.zero;
        private Vector2 _currentPosition = Vector2.zero;

        private float _currentCameraRotationVelocity = 0.0f;
        private float _currentRotation = 0.0f;

        private bool _lockHorizontal = false;
        private bool _lockVertical = false;

        private Vector3 _cameraInitialPosition = Vector3.zero;
        private Vector3 _cameraInitialAngles = Vector3.zero;

        // properties
        public bool IsActive
        {
            get; set;
        }

        #region Unity Methods
        private void FixedUpdate()
        {
            if (!IsActive)
            {
                return;
            }

            TargetPlayer(Time.smoothDeltaTime);
        }
        #endregion

        #region Public Methods
        public void Initialize(Camera camera, Transform playerTransform, Bounds mapBounds)
        {
            _playerTransform = playerTransform;
            _mapBounds = mapBounds;
            _camera = camera;

            if (_camera != null)
            {
                var bottomLeft = camera.ViewportToWorldPoint(Vector3.zero);
                var topRight = camera.ViewportToWorldPoint(Vector3.one);
                var aspect = (topRight.x - bottomLeft.x) / (topRight.y - bottomLeft.y);

                var cameraWidth = _camera.orthographicSize * aspect * 2.0f;
                var cameraHeight = _camera.orthographicSize * 2.0f;

                if (_mapBounds.size.x != 0 && _mapBounds.size.y != 0)
                {
                    if (_mapBounds.size.x < cameraWidth)
                    {
                        _lockHorizontal = true;
                    }
                    if (_mapBounds.size.y < cameraHeight)
                    {
                        _lockVertical = true;
                    }

                    _cameraBoundedMaxPosition = new Vector2(_mapBounds.max.x - (_camera.orthographicSize * aspect), _mapBounds.max.y - _camera.orthographicSize);
                    _cameraBoundedMinPosition = new Vector2(_mapBounds.min.x + (_camera.orthographicSize * aspect), _mapBounds.min.y + _camera.orthographicSize);
                }
                else
                {
                    _cameraBoundedMaxPosition = Vector2.zero;
                    _cameraBoundedMinPosition = Vector2.zero;
                }
            }

            _cameraInitialPosition = _camera.transform.position;
            _cameraInitialAngles = _camera.transform.eulerAngles;

            TargetPlayer(Time.smoothDeltaTime, true);
        }

        public void Snap()
        {
            TargetPlayer(Time.smoothDeltaTime, true);
        }

        public void Reset(bool isActive)
        {
            IsActive = isActive;

            _camera.transform.position = _cameraInitialPosition;
            _camera.transform.eulerAngles = _cameraInitialAngles;
        }
        #endregion

        #region Protected Methods
        #endregion

        #region Private Methods
        private void TargetPlayer(float deltaTime, bool snap = false)
        {
            if (_camera != null && _playerTransform != null)
            {
                Vector2 targetCameraPosition = IncludeMapBoundsInTargetPosition(_playerTransform.position);
                if (_currentPosition != targetCameraPosition)
                {
                    if (!snap && Mathf.Abs((targetCameraPosition - _currentPosition).magnitude) > POSITION_EPSILON)
                    {
                        _currentPosition = Vector2.SmoothDamp(_currentPosition, targetCameraPosition, ref _currentCameraPositionVelocity, _cameraPositionSmoothTime, Mathf.Infinity, deltaTime);
                    }
                    else
                    {
                        _currentPosition = targetCameraPosition;
                    }
                    _camera.transform.position = new Vector3(SnapToNearest(_currentPosition.x), SnapToNearest(_currentPosition.y), _camera.transform.position.z);
                }

                float targetCameraRotation = _playerTransform.eulerAngles.z;
                if (_currentRotation != targetCameraRotation)
                {
                    var deltaAngle = Mathf.DeltaAngle(_currentRotation, targetCameraRotation);
                    if (!snap && Mathf.Abs(deltaAngle) > ROTATION_EPSILON)
                    {
                        _currentRotation = Mathf.SmoothDampAngle(_currentRotation, targetCameraRotation, ref _currentCameraRotationVelocity, _cameraRotationSmoothTime, Mathf.Infinity, deltaTime);
                    }
                    else
                    {
                        _currentRotation = targetCameraRotation;
                    }
                    _camera.transform.eulerAngles = new Vector3(_camera.transform.eulerAngles.x, _camera.transform.eulerAngles.y, _currentRotation);
                }
            }
        }

        private Vector2 IncludeMapBoundsInTargetPosition(Vector2 targetPosition)
        {
            var boundedTargetPosition = Vector2.zero;

            if (_cameraBoundedMinPosition != Vector2.zero && _cameraBoundedMaxPosition != Vector2.zero)
            {
                if (_lockHorizontal)
                {
                    boundedTargetPosition.x = _mapBounds.center.x;
                }
                else
                {
                    boundedTargetPosition.x = Mathf.Clamp(targetPosition.x, _cameraBoundedMinPosition.x, _cameraBoundedMaxPosition.x);
                }

                if (_lockVertical)
                {
                    boundedTargetPosition.y = _mapBounds.center.y;
                }
                else if (_cameraBoundedMinPosition != Vector2.zero && _cameraBoundedMaxPosition != Vector2.zero)
                {
                    boundedTargetPosition.y = Mathf.Clamp(targetPosition.y, _cameraBoundedMinPosition.y, _cameraBoundedMaxPosition.y);
                }
            }
            else
            {
                boundedTargetPosition = targetPosition;
            }

            return boundedTargetPosition;
        }

        private float SnapToNearest(float value)
        {
            return Mathf.RoundToInt(value) - Utils.MathfPlus.Modulo(Mathf.RoundToInt(value), _roundingValue);
        }
        #endregion
    }
}
