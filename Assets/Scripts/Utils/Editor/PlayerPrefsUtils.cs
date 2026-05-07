using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerPrefsUtils
{
    // const

    // public

    // protected

    // private

    // properties

    #region Unity Methods
    #endregion

    #region Public Methods
    [MenuItem("Game/Clear PlayerPrefs", false, 4002)]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion
}
