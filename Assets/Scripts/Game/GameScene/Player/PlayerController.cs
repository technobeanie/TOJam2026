using Common.Joystick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // const

    // public

    // protected

    // private
    [Header("Player")]
    [SerializeField] protected Transform _player = null;
    [SerializeField] private float _movementVelocity = 10.0f;

    // properties
    public InputDevice InputDevice
    {
        get; private set;
    }

    #region Unity Methods
    protected virtual void FixedUpdate()
    {
        if (InputDevice == null)
        {
            return;
        }

        var movement = JoystickManager.Instance.GetMovement(JoystickManager.Joystick.Left, InputDevice);
        if (movement != Vector2.zero)
        {
            Vector3 movement3 = movement;
            _player.transform.localPosition += (movement3 * _movementVelocity * Time.fixedDeltaTime);
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
    #endregion
}
