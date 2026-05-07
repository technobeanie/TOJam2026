using Common.Flow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPopupView : SokobanView
{
    // const

    // public

    // protected

    // private

    // properties

    #region Unity Methods
    #endregion

    #region View Methods
    #endregion

    #region Public Methods
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion

    #region UI Methods
    public void UI_Yes()
    {
        var parameters = new Dictionary<string, object>();

        // TODO:

        FlowManager.Instance.CloseView(ViewName, parameters);
    }
    public void UI_No()
    {
        var parameters = new Dictionary<string, object>();

        // TODO:

        FlowManager.Instance.CloseView(ViewName, parameters);
    }
    #endregion
}
