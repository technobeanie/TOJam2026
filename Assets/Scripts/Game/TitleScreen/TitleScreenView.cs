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
    public void Update()
    {
        if (Keyboard.current != null && Keyboard.current[Key.Escape].isPressed)
        {
            Application.Quit();
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
