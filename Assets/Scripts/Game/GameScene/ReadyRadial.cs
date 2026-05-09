using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReadyRadial : MonoBehaviour
{
    // const

    // public

    // protected

    // private
    private Action _onReady = null;

    [Header("Setup")]
    [SerializeField] private Image _radial = null;
    [SerializeField] private float _speed = 180.0f;

    // properties
    public bool IsHolding
    {
        get; private set;
    }

    #region Unity Methods
    private void FixedUpdate()
    {
        if (!IsHolding || _radial == null)
        {
            return;
        }

        _radial.fillAmount += (_speed * Time.fixedDeltaTime);
        if (_radial.fillAmount >= 1.0f)
        {
            _onReady?.Invoke();

            StopHold();
        }
    }
    #endregion

    #region Public Methods
    public void Initialize()
    {
        StopHold();
    }

    public void BeginHold(Action onReady)
    {
        IsHolding = true;
        _onReady = onReady;

        if (_radial != null)
        {
            _radial.gameObject.SetActive(true);
            _radial.fillAmount = 0.0f;
        }
    }

    public void StopHold()
    {
        IsHolding = false;
        _onReady = null;

        if (_radial != null)
        {
            _radial.gameObject.SetActive(false);
            _radial.fillAmount = 0.0f;
        }
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion
}
