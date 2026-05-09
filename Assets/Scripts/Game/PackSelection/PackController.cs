using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PackController : MonoBehaviour
{
    // const

    // public

    // protected

    // private
    private Action<PackController> _onOpened = null;

    [Header("Setup")]
    [SerializeField] private GameObject _packNormal = null;
    [SerializeField] private GameObject _packOpened = null;
    [SerializeField] private TextMeshPro _packName = null;

    [Header("Images")]
    [SerializeField] private SpriteRenderer _packNormalRenderer = null;
    [SerializeField] private SpriteRenderer _packOpenedLeftRenderer = null;
    [SerializeField] private SpriteRenderer _packOpenedRightRenderer = null;

    // properties
    public StickerPack Pack
    {
        get; private set;
    }

    public bool IsOpened
    {
        get; private set;
    }

    #region Unity Methods
    #endregion

    #region Public Methods
    public void Initialize(StickerPack stickerPack, Action<PackController> onOpened)
    {
        Pack = stickerPack;
        _onOpened = onOpened;

        IsOpened = false;

        _packNormal.SetActive(true);
        _packOpened.SetActive(false);

        _packNormalRenderer.sprite = stickerPack._packNormal;
        _packOpenedLeftRenderer.sprite = stickerPack._packOpenedLeft;
        _packOpenedRightRenderer.sprite = stickerPack._packOpenedRight;
        _packName.text = stickerPack._name;
    }

    public void Open()
    {
        if (IsOpened)
        {
            return;
        }

        IsOpened = true;

        _packNormal.SetActive(false);
        _packOpened.SetActive(true);

        _onOpened?.Invoke(this);
        _onOpened = null;
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion
}
