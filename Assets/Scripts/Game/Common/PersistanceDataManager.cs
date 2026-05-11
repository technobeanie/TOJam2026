using Common.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class PersistanceDataManager : Singleton<PersistanceDataManager>
{
    // const
    private const float MinAvailablePercentage = 0.1f;

    // public

    // protected

    // private
    private Theme _theme = null;
    private List<string> _availableThemes = new List<string>();
    private List<string> _takenThemes = new List<string>();

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
            if (_theme != theme)
            {
                _theme = theme;

                _availableThemes.Clear();
                _takenThemes.Clear();

                _availableThemes.AddRange(_theme._themes);
                _availableThemes.Shuffle();
            }
        }
        else
        {
            // Nothing to do?
        }
    }

    public string FindTheme()
    {
        if (_theme == null)
        {
            return "Do anything you like!";
        }

        var percentage = (float)_availableThemes.Count / (float)_theme._themes.Count;
        if (percentage <= MinAvailablePercentage)
        {
            _takenThemes.Shuffle();
            _availableThemes.AddRange(_takenThemes);
            _takenThemes.Clear();
        }

        var theme = "";
        if (_availableThemes.Count > 0)
        {
            theme = _availableThemes[0];
            _availableThemes.RemoveAt(0);
            _takenThemes.Add(theme);
        }

        return theme;
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion
}
