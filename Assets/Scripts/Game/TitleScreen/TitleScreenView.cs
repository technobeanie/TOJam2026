using Common.Flow;
using Common.Joystick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TitleScreenView : SokobanView
{
    // public

    // private

    // properties

    #region Unity Methods
    public void FixedUpdate()
    {
        if (ViewState != ViewStateIds.Opened)
        {
            return;
        }

        if (Keyboard.current != null && Keyboard.current[Key.Escape].wasPressedThisFrame)
        {
            Application.Quit();
        }

        InputDevice inputDevice = null;
        if (JoystickManager.Instance.IsButtonDownThisFrame(JoystickManager.Button.Xbox_A, out inputDevice) || JoystickManager.Instance.IsButtonDownThisFrame(JoystickManager.Button.Xbox_Menu, out inputDevice))
        {
            UI_Play();
        }
    }
    #endregion

    #region View Methods
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    #endregion

    #region UI Methods
    public void UI_Play()
    {
        FlowManager.Instance.OpenView("PlayerSelection", loadingViewName: "Loading");
    }
    #endregion
}
