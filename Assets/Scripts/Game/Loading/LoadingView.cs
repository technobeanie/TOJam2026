using Common.Flow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingView : View
{
    // const

    // public

    // protected

    // private
    [SerializeField] private Animator _underlayAnimator = null;
    [SerializeField] private string _animationFadeInTrigger = "";
    [SerializeField] private string _animationFadeOutTrigger = "";

    // properties

    #region Unity Methods
    #endregion

    #region View Methods
    protected override void OnViewOpening()
    {
        // NOTE: Not calling the base yet.

        if (_underlayAnimator != null)
        {
            _underlayAnimator.SetTrigger(_animationFadeInTrigger);
        }
    }

    protected override void OnViewPrepareClosing()
    {
        // NOTE: Not calling the base yet.

        if (_underlayAnimator != null)
        {
            _underlayAnimator.SetTrigger(_animationFadeOutTrigger);
        }
    }
    #endregion

    #region Public Methods
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion

    #region UI Methods
    public void UI_OnFadedIn()
    {
        // Now we call the base.
        base.OnViewOpening();
    }

    public void UI_OnFadedOut()
    {
        // Now we call the base.
        base.OnViewPrepareClosing();
    }
    #endregion
}
