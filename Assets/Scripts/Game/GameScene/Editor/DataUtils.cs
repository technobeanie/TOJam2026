using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataUtils
{
    // public

    // private

    // properties

    #region Unity Methods
    [UnityEditor.MenuItem("Assets/Create/ScriptableObject/StickerPack")]
    public static void CreateStickerPack()
    {
        // TODO: Find the furthest number, and increment it.
        ScriptableObjectUtils.CreateScriptableAsset<StickerPack>("StickerPack_0000.asset");
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    #endregion
}
