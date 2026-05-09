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

        // Max 2 players!
        if (_gamepads.Count + _keyboards.Count >= 2)
        {
            return;
        }

        var gamepads = Gamepad.all;
        foreach (var gamepad in gamepads)
        {
            if (!_gamepads.Contains(gamepad))
            {
                if (JoystickManager.Instance.IsButtonDownThisFrame(JoystickManager.Button.Xbox_A, gamepad))
                {
                    _gamepads.Add(gamepad);
                    _onJoin?.Invoke(gamepad, 0);

                    return;
                }
            }
            else
            {
                if (JoystickManager.Instance.IsButtonDownThisFrame(JoystickManager.Button.Xbox_B, gamepad))
                {
                    _gamepads.Remove(gamepad);
                    _onLeave?.Invoke(gamepad, 0);

                    return;
                }
            }
        }

        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            var keyboardId = JoystickManager.Instance.IsButtonDownThisFrame(JoystickManager.Button.Xbox_A, keyboard);
            if (keyboardId != null)
            {
                if (!_keyboards.ContainsKey(keyboardId.Value))
                {
                    _keyboards.Add(keyboardId.Value, keyboard);
                    _onJoin?.Invoke(keyboard, keyboardId.Value);
                }

                return;
            }
            else
            {
                keyboardId = JoystickManager.Instance.IsButtonDownThisFrame(JoystickManager.Button.Xbox_B, keyboard);
                if (keyboardId != null)
                {
                    if (_keyboards.ContainsKey(keyboardId.Value))
                    {
                        _keyboards.Remove(keyboardId.Value);
                        _onLeave?.Invoke(keyboard, keyboardId.Value);
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
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion
}
