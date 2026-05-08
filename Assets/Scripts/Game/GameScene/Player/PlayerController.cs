using Common.Joystick;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
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

    [Header("Convey")]
    [SerializeField] private ConveyBelt _conveyBelt = null;

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

            // Sticker need to follow while holding
            if (_sticker != null)
            {
                // TODO: Keep the sticker offset.

                var position = _player.transform.position;
                position.z = _sticker.transform.position.z;
                _sticker.transform.position = position;
            }
        }

        if (JoystickManager.Instance.IsButtonDown(JoystickManager.Button.Xbox_A, InputDevice))
        {
            if (_sticker == null)
            {
                GrabSticker();
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
        if (_conveyBelt == null)
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
                    _conveyBelt.TryTake(sticker);

                    _sticker = sticker;

                    break;
                }
            }
        }
    }

    private void ReleaseSticker()
    {
        if (_sticker == null)
        {
            return;
        }

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
                    _conveyBelt.PutBack(_sticker);
                    break;
                }
            }
        }

        _sticker = null;
    }
    #endregion
}
