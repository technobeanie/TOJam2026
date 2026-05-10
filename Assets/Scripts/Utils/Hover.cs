using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    // const

    // public

    // protected

    // private
    private Vector3 _startLocalPosition = Vector3.zero;

    [Header("Values")]
    [SerializeField] private Vector3 _movement = Vector3.zero;
    [SerializeField] private float _durationMin = 1.0f;
    [SerializeField] private float _durationMax = 1.0f;

    // properties

    #region Unity Methods
    #endregion

    #region Public Methods
    private void Start()
    {
        _startLocalPosition = transform.localPosition;

        Move();
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void Move()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOLocalMove(_startLocalPosition + _movement, Random.Range(_durationMin, _durationMax) * 0.5f, false).SetEase(Ease.InOutQuad));
        mySequence.AppendCallback(MoveAgain);
        mySequence.Play();
    }

    private void MoveAgain()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(transform.DOLocalMove(_startLocalPosition - _movement, Random.Range(_durationMin, _durationMax), false).SetEase(Ease.InOutQuad));
        mySequence.Append(transform.DOLocalMove(_startLocalPosition + _movement, Random.Range(_durationMin, _durationMax), false).SetEase(Ease.InOutQuad));
        mySequence.AppendCallback(MoveAgain);
        mySequence.Play();
    }
    #endregion
}
