using Common.Joystick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerSelectionController : PlayerController
{
    // const

    // public

    // protected

    // private
    [Header("Label")]
    [SerializeField] private Transform _joinText = null;
    [SerializeField] private Transform _leaveText = null;

    // properties
    public bool HasJoined
    {
        get { return InputDevice != null; }
    }

    #region Unity Methods
    #endregion

    #region Public Methods
    public override void Initialize()
    {
        base.Initialize();

        SetupTexts(false);
        DisplayPlayer(false);
    }

    public override void AssignInputDevice(InputDevice inputDevice)
    {
        base.AssignInputDevice(inputDevice);

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
