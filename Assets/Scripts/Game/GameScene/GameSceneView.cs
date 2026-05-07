using Common.Flow;
using Common.Joystick;
using Common.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utils;

public class GameSceneView : SokobanView
{
    // const

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

        // TODO
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
