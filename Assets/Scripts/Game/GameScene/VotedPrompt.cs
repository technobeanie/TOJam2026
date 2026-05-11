using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class VotedPrompt : MonoBehaviour
{
    // const

    // public

    // protected

    // private
    private float _hidingCurrentTime = 0.0f;

    [Header("Setup")]
    [SerializeField] private Collider2D _bounds = null;
    [SerializeField] private float _maxAngle = 40.0f;
    [SerializeField] private float _punchTitleScale = 1.5f;
    [SerializeField] private float _punchTitleDuration = 0.1f;
    [SerializeField] private float _autoHideDuration = 1.0f;

    // properties

    #region Unity Methods
    public void FixedUpdate()
    {
        if (_hidingCurrentTime > 0.0f)
        {
            _hidingCurrentTime -= Time.fixedDeltaTime;
            if (_hidingCurrentTime <= 0.0f)
            {
                gameObject.SetActive(false);
            }
        }
    }
    #endregion

    #region Public Methods
    public void OnVoted()
    {
        gameObject.SetActive(true);

        transform.DOKill();

        transform.position = new Vector3(Random.Range(_bounds.bounds.min.x, _bounds.bounds.max.x), Random.Range(_bounds.bounds.min.y, _bounds.bounds.max.y), transform.position.z);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Random.Range(-_maxAngle, _maxAngle));

        var previousScale = Vector3.one;
        transform.localScale = previousScale * _punchTitleScale;
        transform.DOScale(previousScale, _punchTitleDuration).SetEase(Ease.InQuad);

        _hidingCurrentTime = _autoHideDuration;
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion
}
