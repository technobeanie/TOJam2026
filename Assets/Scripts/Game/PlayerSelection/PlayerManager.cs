using Common.Joystick;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class PlayerManager : MonoBehaviour
{
    // const

    // public

    // protected

    // private
    private List<InputDevice> _gamepads = new List<InputDevice>();
    private Dictionary<int, InputDevice> _keyboards = new Dictionary<int, InputDevice>();

    private bool _detecting = false;
    private Action<InputDevice, int> _onJoin = null;
    private Action<InputDevice, int> _onLeave = null;

    // properties
    public IReadOnlyList<InputDevice> Gamepads
    {
        get { return _gamepads; }
    }

    #region Unity Methods
    public void FixedUpdate()
    {
        if (!_detecting)
        {
            return;
        }

        var gamepads = Gamepad.all;
        foreach (var gamepad in gamepads)
        {
            if (!_gamepads.Contains(gamepad))
            {
                // Max 2 players!
                if (_gamepads.Count + _keyboards.Count < 2 && JoystickManager.Instance.IsButtonDownThisFrame(JoystickManager.Button.Xbox_A, gamepad))
                {
                    OnJoin(gamepad, 0);
                    _onJoin?.Invoke(gamepad, 0);

                    return;
                }
            }
            else
            {
                if (JoystickManager.Instance.IsButtonDownThisFrame(JoystickManager.Button.Xbox_B, gamepad))
                {
                    OnLeave(gamepad, 0);
                    _onLeave?.Invoke(gamepad, 0);

                    return;
                }
            }
        }

        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            // Max 2 players!
            if (_gamepads.Count + _keyboards.Count < 2)
            {
                if (JoystickManager.Instance.IsButtonDownThisFrame(JoystickManager.Button.Xbox_A, keyboard, out int keyboardId))
                {
                    if (!_keyboards.ContainsKey(keyboardId))
                    {
                        OnJoin(keyboard, keyboardId);
                        _onJoin?.Invoke(keyboard, keyboardId);
                    }

                    return;
                }
            }
            else
            {
                if (JoystickManager.Instance.IsButtonDownThisFrame(JoystickManager.Button.Xbox_B, keyboard, out int keyboardId))
                {
                    if (_keyboards.ContainsKey(keyboardId))
                    {
                        OnLeave(keyboard, keyboardId);
                        _onLeave?.Invoke(keyboard, keyboardId);
                    }

                    return;
                }
            }
        }
    }
    #endregion

    #region Public Methods
    public void StartDetectingPlayers(Action<InputDevice, int> onJoin, Action<InputDevice, int> onLeave)
    {
        _detecting = true;

        _onJoin = onJoin;
        _onLeave = onLeave;
    }

    public void StopDetectingPlayers()
    {
        _detecting = false;

        _onJoin = null;
        _onLeave = null;
    }

    public void OnJoin(InputDevice inputDevice, int keyboardPlayerId)
    {
        if (inputDevice is Gamepad)
        {
            _gamepads.Add(inputDevice);
        }
        else if (inputDevice is Keyboard)
        {
            _keyboards.Add(keyboardPlayerId, inputDevice);
        }
    }

    public void OnLeave(InputDevice inputDevice, int keyboardPlayerId)
    {
        if (inputDevice is Gamepad)
        {
            _gamepads.Remove(inputDevice);
        }
        else if (inputDevice is Keyboard)
        {
            _keyboards.Remove(keyboardPlayerId);
        }
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion
}
