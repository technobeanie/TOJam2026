using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleGuy : MonoBehaviour
{
    // const

    // public

    // protected

    // private
    private Action _onDone = null;

    [Header("Setup")]
    [SerializeField] private GameObject _littleGuy = null;
    [SerializeField] private Transform _startPosition = null;
    [SerializeField] private Transform _endPosition = null;

    [Header("Timing")]
    [SerializeField] private float _animateDuration = 1.0f;
    [SerializeField] private Ease _animateEase = Ease.InOutQuad;

    // properties

    #region Unity Methods
    #endregion

    #region Public Methods
    public void Initialize()
    {
        _littleGuy.SetActive(false);

        _littleGuy.transform.position = _startPosition.position;
    }

    public void Show(Action onDone = null)
    {
        _onDone = onDone;

        _littleGuy.SetActive(true);

        var seq = DOTween.Sequence();
        seq.Append(_littleGuy.transform.DOMove(_endPosition.position, _animateDuration, true).SetEase(_animateEase));
        seq.AppendCallback(OnShowDone);
        seq.Play();
    }

    public void Hide(Action onDone = null)
    {
        _onDone = onDone;

        _littleGuy.SetActive(true);

        var seq = DOTween.Sequence();
        seq.Append(_littleGuy.transform.DOMove(_startPosition.position, _animateDuration, true).SetEase(_animateEase));
        seq.AppendCallback(OnHideDone);
        seq.Play();
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void OnShowDone()
    {
        _onDone?.Invoke();
        _onDone = null;
    }

    private void OnHideDone()
    {
        _littleGuy.SetActive(false);

        _onDone?.Invoke();
        _onDone = null;
    }
    #endregion
}
