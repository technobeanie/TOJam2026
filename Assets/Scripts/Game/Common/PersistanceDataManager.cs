using Common.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistanceDataManager : Singleton<PersistanceDataManager>
{
    // const

    // public

    // protected

    // private
    private List<string> _allThemes = new List<string>();

    // properties

    #region Unity Methods
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region Public Methods
    public void Initialize(Theme theme)
    {
        if (theme != null)
        {
            _allThemes.AddRange(theme._themes);
        }
    }

    public string FindTheme()
    {
        var theme = "";
        if (_allThemes.Count > 0)
        {
            theme = _allThemes[UnityEngine.Random.Range(0, _allThemes.Count)];
        }

        return theme;
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion
}
