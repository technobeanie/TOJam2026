using Common.Joystick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerController : MonoBehaviour
{
    // const

    // public

    // protected

    // private
    [Header("Label")]
    [SerializeField] private Transform _joinText = null;
    [SerializeField] private Transform _leaveText = null;

    [Header("Player")]
    [SerializeField] private Transform _player = null;
    [SerializeField] private float _movementVelocity = 10.0f;

    // properties
    public bool HasJoined
    {
        get { return InputDevice != null; }
    }

    public InputDevice InputDevice
    {
        get; private set;
    }

    #region Unity Methods
    private void FixedUpdate()
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
    public void Initialize()
    {
        SetupTexts(false);
        DisplayPlayer(false);
    }

    public void AssignInputDevice(InputDevice inputDevice)
    {
        InputDevice = inputDevice;

        SetupTexts(InputDevice != null);
        DisplayPlayer(InputDevice != null);
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void DisplayPlayer(bool joined)
    {
        if (_player != null)
        {
            _player.localPosition = Vector3.zero;
            _player.gameObject.SetActive(joined);
        }
    }

    private void SetupTexts(bool joined)
    {
        if (_joinText != null)
        {
            _joinText.gameObject.SetActive(!joined);
        }
        if (_leaveText != null)
        {
            _leaveText.gameObject.SetActive(joined);
        }
    }
    #endregion
}
