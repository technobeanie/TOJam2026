using UnityEngine;

public static class ScriptableObjectUtils
{
    // public

    // private

    // properties

    #region Unity Methods
    #endregion

    #region Public Methods
    public static void CreateScriptableAsset<T>(string assetName) where T : ScriptableObject
    {
        var obj = UnityEditor.Selection.activeObject;
        if (obj != null)
        {
            var path = UnityEditor.AssetDatabase.GetAssetPath(obj.GetEntityId());

            var asset = ScriptableObject.CreateInstance<T>();
            UnityEditor.AssetDatabase.CreateAsset(asset, System.IO.Path.Combine(path, assetName));
            UnityEditor.AssetDatabase.SaveAssets();

            UnityEditor.EditorUtility.FocusProjectWindow();

            UnityEditor.Selection.activeObject = asset;
        }
    }
    #endregion

    #region Private Methods
    #endregion
}
