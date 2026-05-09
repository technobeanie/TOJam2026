using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackController : MonoBehaviour
{
    // const

    // public

    // protected

    // private
    [Header("Setup")]
    [SerializeField] private GameObject _packNormal = null;
    [SerializeField] private GameObject _packOpened = null;

    [Header("Images")]
    [SerializeField] private SpriteRenderer _packNormalRenderer = null;
    [SerializeField] private SpriteRenderer _packOpenedLeftRenderer = null;
    [SerializeField] private SpriteRenderer _packOpenedRightRenderer = null;

    // properties
    public StickerPack Pack
    {
        get; private set;
    }

    #region Unity Methods
    #endregion

    #region Public Methods
    public void Initialize(StickerPack stickerPack)
    {
        Pack = stickerPack;

        _packNormal.SetActive(true);
        _packOpened.SetActive(false);

        _packNormalRenderer.sprite = stickerPack._packNormal;
        _packOpenedLeftRenderer.sprite = stickerPack._packOpenedLeft;
        _packOpenedRightRenderer.sprite = stickerPack._packOpenedRight;
    }

    public void Open()
    {
        _packNormal.SetActive(false);
        _packOpened.SetActive(true);
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion
}
