using Common.Audio;
using Common.Joystick;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Utils;

public class PlayerController : MonoBehaviour
{
    // const

    // public

    // protected

    // private
    private Sticker _sticker = null;
    private Action<PlayerController> _onReady = null;

    [Header("Player")]
    [SerializeField] protected Transform _player = null;
    [SerializeField] private float _movementVelocity = 1000.0f;
    [SerializeField] private Transform _stickerAnchor = null;
    [SerializeField] private BoxCollider2D _playerBounds = null;

    [Header("Sticker")]
    [SerializeField] private StickerPool _stickerPool = null;
    [SerializeField] private float _stickerRotationSpeed = 180.0f;

    [Header("UI")]
    [SerializeField] private ReadyRadial _radial = null;
    [SerializeField] private GameObject _pendingReadyText = null;
    [SerializeField] private GameObject _isReadyText = null;

    [Header("Audio")]
    [SerializeField] private AudioHook _pickUpSFX = null;
    [SerializeField] private AudioHook _dropSFX = null;

    [Header("Anim")]
    [SerializeField] private Animator _animator = null;
    [SerializeField] private string _grabBool = "Grab";
    [SerializeField] private string _waitBool = "Wait";

    // properties
    public InputDevice InputDevice
    {
        get; private set;
    }

    public int KeyboardPlayerId
    {
        get; private set;
    }

    public bool IsDone
    {
        get; private set;
    }

    #region Unity Methods
    protected virtual void FixedUpdate()
    {
        if (InputDevice == null || _player == null)
        {
            return;
        }

        var movement = JoystickManager.Instance.GetMovement(JoystickManager.Joystick.Left, InputDevice, KeyboardPlayerId);
        if (movement != Vector2.zero)
        {
            Vector3 movement3 = movement * _movementVelocity * Time.fixedDeltaTime;
            movement3.x = Mathf.Clamp(_player.transform.position.x + movement3.x, _playerBounds.bounds.min.x, _playerBounds.bounds.max.x);
            movement3.y = Mathf.Clamp(_player.transform.position.y + movement3.y, _playerBounds.bounds.min.y, _playerBounds.bounds.max.y);
            movement3.z = _player.transform.position.z;

            _player.transform.position = movement3;
        }

        if (!IsDone)
        {
            if (JoystickManager.Instance.IsButtonDown(JoystickManager.Button.Xbox_A, InputDevice, KeyboardPlayerId))
            {
                if (_animator != null)
                {
                    _animator.SetBool(_grabBool, true);
                }

                if (_sticker == null)
                {
                    GrabSticker();
                }
                else
                {
                    TryRotate(Time.fixedDeltaTime);
                }
            }
            else
            {
                if (_animator != null)
                {
                    _animator.SetBool(_grabBool, false);
                }

                if (_sticker != null)
                {
                    ReleaseSticker();
                }
            }

            TryReady();
        }
    }
    #endregion

    #region Public Methods
    public virtual void Initialize(Action<PlayerController> onReady = null)
    {
        if (_radial != null)
        {
            _radial.Initialize();
        }

        _onReady = onReady;
        IsDone = false;

        RefreshIsReady(false);
    }

    public virtual void AssignInputDevice(InputDevice inputDevice, int keyboardPlayerId)
    {
        InputDevice = inputDevice;
        KeyboardPlayerId = keyboardPlayerId;
    }

    public void Begin()
    {
        IsDone = false;

        if (_animator != null)
        {
            _animator.SetBool(_grabBool, false);
            _animator.SetBool(_waitBool, false);
        }
    }

    public void GameDone()
    {
        if (IsDone)
        {
            return;
        }

        IsDone = true;

        if (_animator != null)
        {
            _animator.SetBool(_grabBool, false);
            _animator.SetBool(_waitBool, true);
        }

        if (_pendingReadyText != null)
        {
            _pendingReadyText.SetActive(false);
        }

        if (_isReadyText != null)
        {
            _isReadyText.SetActive(false);
        }
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void GrabSticker()
    {
        if (_stickerPool == null || _player == null || _stickerAnchor == null)
        {
            return;
        }

        // Raycast and find nearest sticker.
        var hits = Physics2D.RaycastAll(_player.position, Vector3.back);
        if (hits != null && hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                var sticker = hit.transform.GetComponentInParent<Sticker>();
                if (sticker != null)
                {
                    if (!sticker.IsGrabbed)
                    {
                        _stickerPool.TryTake(sticker);
                        sticker.IsGrabbed = true;

                        _stickerAnchor.transform.localEulerAngles = Vector3.zero;

                        _sticker = sticker;
                        _sticker.transform.parent = _stickerAnchor;

                        // Play SFX
                        if (_pickUpSFX != null)
                        {
                            _pickUpSFX.Play();
                        }

                        break;
                    }
                }
            }
        }
    }

    private void ReleaseSticker()
    {
        if (_sticker == null || _stickerPool == null || _stickerAnchor == null)
        {
            return;
        }

        _sticker.IsGrabbed = false;
        _sticker.transform.parent = _stickerPool.transform;

        // Raycast and find nearest sticker.
        var stickerBounds = _sticker.gameObject.GetBounds();

        var hits = Physics2D.BoxCastAll(stickerBounds.center, stickerBounds.size, 0.0f, Vector3.back);
        if (hits != null && hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                var convey = hit.transform.GetComponentInParent<ConveyBelt>();
                if (convey != null)
                {
                    _stickerPool.PutBack(_sticker);
                    break;
                }
            }
        }

        // Play SFX
        if (_dropSFX != null)
        {
            _dropSFX.Play();
        }

        _sticker = null;
    }

    private void TryReady()
    {
        if (_radial == null)
        {
            return;
        }

        if (JoystickManager.Instance.IsButtonDown(JoystickManager.Button.Xbox_Y, InputDevice, KeyboardPlayerId))
        {
            if (!_radial.IsHolding)
            {
                _radial.BeginHold(OnReady);
            }
        }
        else if (_radial.IsHolding)
        {
            _radial.StopHold();
        }
    }

    private void OnReady()
    {
        RefreshIsReady(true);

        _onReady?.Invoke(this);
    }

    private void RefreshIsReady(bool isReady)
    {
        if (_pendingReadyText != null)
        {
            _pendingReadyText.SetActive(!isReady);
        }

        if (_isReadyText != null)
        {
            _isReadyText.SetActive(isReady);
        }
    }

    private void TryRotate(float deltaTime)
    {
        if (_sticker == null || _stickerAnchor == null || InputDevice == null)
        {
            return;
        }

        // Turn left.
        var trigger = JoystickManager.Instance.IsTriggerDown(JoystickManager.Trigger.Left, InputDevice, KeyboardPlayerId);
        if (trigger != 0.0f)
        {
            Rotate(_stickerAnchor, trigger, deltaTime);
        }
        else if (JoystickManager.Instance.IsButtonDown(JoystickManager.Button.Xbox_LB, InputDevice, KeyboardPlayerId))
        {
            Rotate(_stickerAnchor, 1.0f, deltaTime);
        }

        // Turn right.
        trigger = JoystickManager.Instance.IsTriggerDown(JoystickManager.Trigger.Right, InputDevice, KeyboardPlayerId);
        if (trigger != 0.0f)
        {
            Rotate(_stickerAnchor, -trigger, deltaTime);
        }
        else if (JoystickManager.Instance.IsButtonDown(JoystickManager.Button.Xbox_RB, InputDevice, KeyboardPlayerId))
        {
            Rotate(_stickerAnchor, -1.0f, deltaTime);
        }
    }

    private void Rotate(Transform anchor, float trigger, float deltaTime)
    {
        var angles = _stickerAnchor.localEulerAngles;
        angles.z += trigger * _stickerRotationSpeed * deltaTime;
        anchor.localEulerAngles = angles;
    }
    #endregion
}
