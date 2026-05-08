using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyBelt : MonoBehaviour
{
    // const

    // public

    // protected

    // private
    private float _currentSpeed = 0.0f;
    private float _currentAccelerationTarget = 0.0f;
    private float _currentSpeedTarget = 0.0f;

    [Header("Sticker")]
    [SerializeField] private StickerPool _stickerPool = null;

    [Header("Rendering")]
    [SerializeField] private Renderer _beltRenderer = null;

    [Header("Speed")]
    [SerializeField] private float _acceleration = 0.1f;
    [SerializeField] private float _topSpeed = 1.0f;
    [SerializeField] private float _stickerSpeedFactor = 1.0f;

    // properties
    public bool IsActive
    {
        get; private set;
    }

    #region Unity Methods
    public void FixedUpdate()
    {
        if (!IsActive)
        {
            return;
        }

        Accelerate(Time.fixedDeltaTime);
    }
    #endregion

    #region Public Methods
    public void Begin()
    {
        IsActive = true;

        _currentSpeed = 0.0f;

        _currentAccelerationTarget = _acceleration;
        _currentSpeedTarget = _topSpeed;
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void Accelerate(float deltaTime)
    {
        // Move belt.
        if (_currentSpeed < _currentSpeedTarget)
        {
            _currentSpeed = Mathf.Clamp(_currentSpeed + (_currentAccelerationTarget * deltaTime), 0.0f, _currentSpeedTarget);
        }
        else if (_currentSpeed > _currentSpeedTarget)
        {
            _currentSpeed = Mathf.Clamp(_currentSpeed - (_currentAccelerationTarget * deltaTime), _currentSpeedTarget, _currentSpeed);
        }

        float offset = _currentSpeed * deltaTime;
        if (_beltRenderer != null && _beltRenderer.material != null)
        {
            var textureOffset = _beltRenderer.material.mainTextureOffset;
            textureOffset.y = (textureOffset.y + offset) % 1.0f; ;
            _beltRenderer.material.SetTextureOffset("_MainTex", textureOffset);
        }

        // Move stickers.
        if (_stickerPool != null)
        {
            _stickerPool.MoveStickers(offset * _stickerSpeedFactor);
        }
    }
    #endregion
}
