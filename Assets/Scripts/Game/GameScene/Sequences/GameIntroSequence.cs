using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class GameIntroSequence : MonoBehaviour
{
    // const

    // public

    // protected

    // private
    private Action _onDone = null;
    private Timer _timer = null;

    [Header("Setup")]
    [SerializeField] private GameObject _title = null;
    [SerializeField] private LittleGuy _guy = null;

    [Header("Timing")]
    [SerializeField] private float _beforeDelay = 5.0f;
    [SerializeField] private float _punchTitleScale = 1.2f;
    [SerializeField] private float _punchTitleDuration = 0.2f;

    // properties

    #region Unity Methods
    private void FixedUpdate()
    {
        if (_timer != null)
        {
            _timer.Step(Time.fixedDeltaTime);
        }
    }
    #endregion

    #region Public Methods
    public void Initialize()
    {
        _title.SetActive(false);
        _guy.Initialize();
    }

    public void Begin(Action onDone)
    {
        _onDone = onDone;

        _title.SetActive(true);

        var previousScale = _title.transform.localScale;
        _title.transform.localScale = previousScale * _punchTitleScale;
        _title.transform.DOScale(previousScale, _punchTitleDuration).SetEase(Ease.InQuad);

        _guy.Show();

        // First delay.
        _timer = new Timer(_beforeDelay, OnEndFirstDelay);
        _timer.Begin();
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void OnEndFirstDelay(Timer timer)
    {
        _title.SetActive(false);

        _guy.Hide();

        _onDone?.Invoke();
        _onDone = null;
    }
    #endregion
}
