using Common.Flow;
using Common.Joystick;
using Common.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public class GameSceneView : SokobanView
{
    // const
    public const string FlowParameter_Player1 = "FlowParameter_Player1";
    public const string FlowParameter_Player2 = "FlowParameter_Player2";

    // public

    // private

    // properties

    #region Unity Methods
    private void FixedUpdate()
    {
        if (ViewState != ViewStateIds.Opened)
        {
            return;
        }

        // TODO
    }
    #endregion

    #region View Methods
    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        if (_parameters != null)
        {
            if (_parameters.ContainsKey(FlowParameter_Player1))
            {
                var player1 = _parameters[FlowParameter_Player1] as InputDevice;
            }

            if (_parameters.ContainsKey(FlowParameter_Player2))
            {
                var player2 = _parameters[FlowParameter_Player2] as InputDevice;
            }
        }
    }

    protected override void OnViewOpened()
    {
        base.OnViewOpened();

        // Let's begin the game.
        // TODO:
    }

    protected override void OnViewClosed(Dictionary<string, object> parameters)
    {
        base.OnViewClosed(parameters);

        // TODO:
    }

    protected override void OnViewReturned(Dictionary<string, object> parameters)
    {
        base.OnViewReturned(parameters);

        // TODO:
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    #endregion

    #region UI Methods
    #endregion
}
