using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class ThemeSequence : MonoBehaviour
{
    // const

    // public

    // protected

    // private
    private Action _onDone = null;
    private Timer _timer = null;

    [Header("Setup")]
    [SerializeField] private GameObject _title = null;
    [SerializeField] private RectTransform _startAnchor = null;
    [SerializeField] private RectTransform _normalAnchor = null;
    [SerializeField] private GameObject _themePanel = null;
    [SerializeField] private LittleGuy _guy = null;

    [Header("Timing")]
    [SerializeField] private float _beforeDelay = 5.0f;
    [SerializeField] private float _afterDelay = 1.0f;
    [SerializeField] private float _themeMoveDuration = 2.0f;
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
        _themePanel.SetActive(false);
        _guy.Initialize();
    }

    public void Begin(Action onDone)
    {
        _onDone = onDone;

        // Setup.
        _title.SetActive(true);
        _themePanel.SetActive(true);

        _themePanel.transform.localPosition = _startAnchor.localPosition;
        _themePanel.transform.localScale = _startAnchor.localScale;

        // Punch.
        var previousScale = _title.transform.localScale;
        _title.transform.localScale = previousScale * _punchTitleScale;
        _title.transform.DOScale(previousScale, _punchTitleDuration).SetEase(Ease.InQuad);

        previousScale = _themePanel.transform.localScale;
        _themePanel.transform.localScale = previousScale * _punchTitleScale;
        _themePanel.transform.DOScale(previousScale, _punchTitleDuration).SetEase(Ease.InQuad);

        // First delay.
        _timer = new Timer(_beforeDelay, OnEndFirstDelay);
        _timer.Begin();

        _guy.Show();
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void OnEndFirstDelay(Timer timer)
    {
        _title.SetActive(false);

        _timer = new Timer(_afterDelay, OnSecondDelay);
        _timer.Begin();

        _guy.Hide();
    }

    private void OnSecondDelay(Timer timer)
    {
        _themePanel.transform.DOScale(_normalAnchor.localScale, _themeMoveDuration).SetEase(Ease.OutBounce);

        var sequence = DOTween.Sequence();
        sequence.Append(_themePanel.transform.DOLocalMove(_normalAnchor.localPosition, _themeMoveDuration, true).SetEase(Ease.InOutQuad));
        sequence.AppendCallback(OnThemeMoveDone);
        sequence.Play();
    }

    private void OnThemeMoveDone()
    {
        _onDone?.Invoke();
        _onDone = null;
    }
    #endregion
}
