using Common.Joystick;
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

    [Header("Player")]
    [SerializeField] protected Transform _player = null;
    [SerializeField] private float _movementVelocity = 1000.0f;
    [SerializeField] private Transform _stickerAnchor = null;

    [Header("Sticker")]
    [SerializeField] private StickerPool _stickerPool = null;

    // properties
    public InputDevice InputDevice
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

        var movement = JoystickManager.Instance.GetMovement(JoystickManager.Joystick.Left, InputDevice);
        if (movement != Vector2.zero)
        {
            Vector3 movement3 = movement;
            _player.transform.localPosition += (movement3 * _movementVelocity * Time.fixedDeltaTime);
        }

        if (JoystickManager.Instance.IsButtonDown(JoystickManager.Button.Xbox_A, InputDevice))
        {
            if (_sticker == null)
            {
                GrabSticker();
            }
            else
            {
                //TryRotate();
            }
        }
        else if (_sticker != null)
        {
            ReleaseSticker();
        }
    }
    #endregion

    #region Public Methods
    public virtual void Initialize()
    {
    }

    public virtual void AssignInputDevice(InputDevice inputDevice)
    {
        InputDevice = inputDevice;
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
        _sticker.transform.parent = transform;

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
                    _sticker.transform.parent = _stickerPool.transform;
                    break;
                }
            }
        }

        _sticker = null;
    }

    private void TryRotate()
    {
        if (_sticker == null || _stickerAnchor == null || InputDevice == null)
        {
            return;
        }

        // Turn left.
        var trigger = JoystickManager.Instance.IsTriggerDown(JoystickManager.Trigger.Left, InputDevice);
        if (trigger != 0.0f)
        {
            Debug.Log("LEFT" + trigger.ToString());
        }

        // Turn right.
        trigger = JoystickManager.Instance.IsTriggerDown(JoystickManager.Trigger.Right, InputDevice);
        if (trigger != 0.0f)
        {
            Debug.Log("RIGHT" + trigger.ToString());
        }
    }
    #endregion
}
