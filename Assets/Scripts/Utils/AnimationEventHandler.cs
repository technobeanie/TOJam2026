using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEventHandler : MonoBehaviour
{
    // const

    // public

    // protected

    // private
    [SerializeField] private List<UnityEvent> _onAnimationEnded = new List<UnityEvent>();

    // properties

    #region Unity Methods
    #endregion

    #region Public Methods
    public void OnAnimationEnded(int callbackIndex)
    {
        if (callbackIndex < _onAnimationEnded.Count && _onAnimationEnded[callbackIndex] != null)
        {
            _onAnimationEnded[callbackIndex].Invoke();
        }
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion
}
