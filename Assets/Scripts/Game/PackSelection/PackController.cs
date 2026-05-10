using Common.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils;

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
    [SerializeField] private AudioHook _voChoosePack = null;

    [Header("Images")]
    [SerializeField] private SpriteRenderer _packNormalRenderer = null;
    [SerializeField] private Animator _packOpenedRenderer = null;
    [SerializeField] private string _openBool = "Open";

    [Header("Sticker")]
    [SerializeField] private List<SpriteRenderer> _stickers = new List<SpriteRenderer>();

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

        if (_packName != null)
        {
            _packName.text = stickerPack._name;
        }

        if (_packOpenedRenderer != null)
        {
            _packOpenedRenderer.SetBool(_openBool, false);
        }

        AssignStickers();
    }

    public void Open()
    {
        if (IsOpened)
        {
            return;
        }

        if (_voChoosePack != null)
        {
            _voChoosePack.Play();
        }

        IsOpened = true;

        _packNormal.SetActive(false);
        _packOpened.SetActive(true);

        if (_packOpenedRenderer != null)
        {
            _packOpenedRenderer.SetBool(_openBool, true);
        }

        _onOpened?.Invoke(this);
        _onOpened = null;
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void AssignStickers()
    {
        var stickers = new List<UnityEngine.Object>(Pack._stickers);
        stickers.Shuffle();

        int i = 0;
        foreach (var sprite in _stickers)
        {
            var texture2D = stickers[i] as Texture2D;
            sprite.sprite = Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 2);
            sprite.transform.localEulerAngles = new Vector3(0.0f, 0.0f, UnityEngine.Random.Range(0.0f, 360.0f));

            ++i;
        }
    }
    #endregion
}
