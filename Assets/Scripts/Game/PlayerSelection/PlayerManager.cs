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
    private bool _detecting = false;
    private Action<InputDevice> _onJoin = null;
    private Action<InputDevice> _onLeave = null;

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
                if (JoystickManager.Instance.IsButtonDownThisFrame(JoystickManager.Button.Xbox_A, gamepad))
                {
                    _gamepads.Add(gamepad);
                    _onJoin?.Invoke(gamepad);
                }
            }
            else
            {
                if (JoystickManager.Instance.IsButtonDownThisFrame(JoystickManager.Button.Xbox_B, gamepad))
                {
                    _gamepads.Remove(gamepad);
                    _onLeave?.Invoke(gamepad);
                }
            }
        }
    }
    #endregion

    #region Public Methods
    public void StartDetectingPlayers(Action<InputDevice> onJoin, Action<InputDevice> onLeave)
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
