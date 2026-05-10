using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class PackSequence : MonoBehaviour
{
    // const

    // public

    // protected

    // private
    private Action _onDone = null;
    private Timer _timer = null;

    [Header("Setup")]
    [SerializeField] private List<PackController> _packs = new List<PackController>();
    [SerializeField] private GameObject _title = null;

    [Header("Timing")]
    [SerializeField] private float _beforeDelay = 5.0f;
    [SerializeField] private float _afterDelay = 0.25f;
    [SerializeField] private float _packDelay = 0.5f;
    [SerializeField] private Vector3 _packPunch = Vector3.zero;
    [SerializeField] private float _packPunchDuration = 1.0f;
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

        foreach (var pack in _packs)
        {
            if (pack != null)
            {
                pack.gameObject.SetActive(false);
            }
        }
    }

    public void Begin(Action onDone)
    {
        _onDone = onDone;

        _title.SetActive(true);

        var previousScale = _title.transform.localScale;
        _title.transform.localScale = previousScale * _punchTitleScale;
        _title.transform.DOScale(previousScale, _punchTitleDuration).SetEase(Ease.InQuad);

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

        _timer = new Timer(_afterDelay, OnSecondDelay);
        _timer.Begin();
    }

    private void OnSecondDelay(Timer timer)
    {
        bool found = false;

        foreach (var pack in _packs)
        {
            if (pack != null && !pack.gameObject.activeSelf)
            {
                found = true;

                pack.gameObject.SetActive(true);
                pack.transform.DOPunchScale(_packPunch, _packPunchDuration);
                break;
            }
        }

        if (found)
        {
            _timer = new Timer(_packDelay, OnSecondDelay);
            _timer.Begin();
        }
        else
        {
            _onDone?.Invoke();
            _onDone = null;
        }
    }
    #endregion
}
