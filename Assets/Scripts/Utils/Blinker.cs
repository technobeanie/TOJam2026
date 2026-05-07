using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class Blinker : MonoBehaviour
{
    // const
    public enum BlinkingSource
    {
        Renderers,
        GameObject
    }

    // public

    // protected

    // private
    [SerializeField] bool _playOnStart = false;
    [SerializeField] float _displayDuration = 0.5f;
    [SerializeField] float _hideDuration = 0.5f;
    [SerializeField] GameObject _blinkingObject = null;
    [SerializeField] BlinkingSource _blinkingSource = 0;

    private Timer _displayTimer = null;
    private Timer _hideTimer = null;

    private Renderer[] _renderers = null;
    private Graphic[] _graphics = null;

    // properties

    #region Unity Methods
    private void Start()
    {
        if (_playOnStart)
        {
            Begin();
        }
    }

    private void Update()
    {
        _displayTimer?.Step(Time.deltaTime);
        _hideTimer?.Step(Time.deltaTime);
    }
    #endregion

    #region Public Methods
    public void Begin()
    {
        if (_blinkingSource == BlinkingSource.Renderers && _blinkingObject != null)
        {
            _renderers = _blinkingObject.GetComponentsInChildren<Renderer>(true);
            _graphics = _blinkingObject.GetComponentsInChildren<Graphic>(true);
        }

        _displayTimer = new Timer(_displayDuration, OnDisplayEnded);
        _hideTimer = new Timer(_hideDuration, OnHidingEnded);

        Refresh(true);
        _displayTimer.Begin();
    }

    public void Pause()
    {
        _displayTimer?.Pause();
        _hideTimer?.Pause();
    }

    public void Resume()
    {
        _displayTimer?.Resume();
        _hideTimer?.Resume();
    }

    public void Stop()
    {
        _displayTimer = null;
        _hideTimer = null;

        _renderers = null;
        _graphics = null;
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void OnDisplayEnded(Timer timer)
    {
        Refresh(false);
        _hideTimer.Begin();
    }

    private void OnHidingEnded(Timer timer)
    {
        Refresh(true);
        _displayTimer.Begin();
    }

    private void Refresh(bool visible)
    {
        if (_blinkingSource == BlinkingSource.Renderers)
        {
            if (_renderers != null && _renderers.Length > 0)
            {
                for (int i = 0; i < _renderers.Length; ++i)
                {
                    _renderers[i].enabled = visible;
                }
            }
            if (_graphics != null && _graphics.Length > 0)
            {
                for (int i = 0; i < _graphics.Length; ++i)
                {
                    _graphics[i].enabled = visible;
                }
            }
        }
        else if (_blinkingSource == BlinkingSource.GameObject)
        {
            if (_blinkingObject != null)
            {
                _blinkingObject.SetActive(visible);
            }
        }
    }
    #endregion
}
