using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Utils;

public class StickerPool : MonoBehaviour
{
    // const
    private const float AssignedZPositionStickerOffset = -50.0f;

    // public

    // protected

    // private
    [Header("Setup")]
    [SerializeField] private Sticker _stickerPrefab = null;
    [SerializeField] private Transform _topAnchorPoint = null;
    [SerializeField] private Transform _leftAnchorPoint = null;
    [SerializeField] private Transform _rightAnchorPoint = null;
    [SerializeField] private Transform _bottomAnchorPoint = null;

    [Header("Offset Vertical")]
    [SerializeField] private float _minVerticalOffset = 20.0f;
    [SerializeField] private float _maxVerticalOffset = 100.0f;

    [Header("Offset Horizontal")]
    [SerializeField] private float _minHorizontalOffset = -20.0f;
    [SerializeField] private float _maxHorizontalOffset = 20.0f;

    private List<Sticker> _stickers = new List<Sticker>();

    private List<Sticker> _visibleStickerPool = new List<Sticker>();
    private List<Sticker> _availableStickerPool = new List<Sticker>();
    private List<Sticker> _assignedStickerPool = new List<Sticker>();

    private bool _flipSide = false;

    // properties
    public bool IsSpawning
    {
        get; set;
    }

    public bool AnyVisible
    {
        get { return _visibleStickerPool.Count > 0; }
    }

    #region Unity Methods
    #endregion

    #region Public Methods
    public void Initiatize(List<StickerPack> packs)
    {
        // Clean.
        foreach (var sticker in _stickers)
        {
            Destroy(sticker.gameObject);
        }
        _stickers.Clear();

        _visibleStickerPool.Clear();
        _availableStickerPool.Clear();
        _assignedStickerPool.Clear();

        // Create strickers.
        foreach (var pack in packs)
        {
            foreach (var sticker in pack._stickers)
            {
                if (sticker != null)
                {
                    CreateSticker(sticker);
                }
            }
        }

        _availableStickerPool.AddRange(_stickers);

        // Randomize.
        _availableStickerPool.Shuffle();

        // Distribute them at the top.
        ResumeSpawning();
        PlaceAtTop();
    }

    public void ResumeSpawning()
    {
        IsSpawning = true;
    }

    public void StopSpawning()
    {
        IsSpawning = false;
        
        if (_topAnchorPoint == null)
        {
            return;
        }

        // Remove all above the screen stickers.
        for (int i = _visibleStickerPool.Count - 1; i >= 0; --i)
        {
            var sticker = _visibleStickerPool[i];

            var bounds = sticker.gameObject.GetBounds();
            if (bounds.min.y > _topAnchorPoint.position.y)
            {
                _visibleStickerPool.Remove(sticker);
                _availableStickerPool.Add(sticker);
            }
        }
    }

    public void MoveStickers(float offset)
    {
        // Move.
        for (int i = _visibleStickerPool.Count - 1; i >= 0; --i)
        {
            var sticker = _visibleStickerPool[i];

            var position = sticker.transform.position;
            position.y -= offset;
            sticker.transform.position = position;

            if (_bottomAnchorPoint != null)
            {
                var bounds = sticker.gameObject.GetBounds();

                // If at the bottom of the screen, we put it back up!
                if (bounds.max.y < _bottomAnchorPoint.position.y)
                {
                    _visibleStickerPool.Remove(sticker);
                    _availableStickerPool.Add(sticker);
                }
            }
        }

        // Any out of bounds? Then, put back at top.
        if (IsSpawning)
        {
            PlaceAtTop();
        }
    }

    public bool TryTake(Sticker sticker)
    {
        if (_visibleStickerPool.Contains(sticker))
        {
            _visibleStickerPool.Remove(sticker);
            _assignedStickerPool.Insert(0, sticker);

            ReorderAssignedStickers();

            return true;
        }
        else
        {
            // Reorder. More in front, higher prio.
            _assignedStickerPool.Remove(sticker);
            _assignedStickerPool.Insert(0, sticker);

            ReorderAssignedStickers();
        }

        return false;
    }

    public bool PutBack(Sticker sticker)
    {
        if (!_visibleStickerPool.Contains(sticker))
        {
            _visibleStickerPool.Insert(0, sticker);
            _assignedStickerPool.Remove(sticker);

            // Z position (depth)
            var position = sticker.transform.position;
            position.z = transform.position.z - _visibleStickerPool.Count;
            sticker.transform.position = position;

            return true;
        }

        return false;
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void PlaceAtTop()
    {
        if (_availableStickerPool.Count > 0)
        {
            // TODO: Just add a few, and not all.

            for (int i = _availableStickerPool.Count - 1; i >= 0; --i)
            {
                var sticker = _availableStickerPool[i];
                PlaceAtTop(sticker);
            }
        }
    }

    private void PlaceAtTop(Sticker sticker)
    {
        Vector3 startStickerPosition = Vector3.zero;

        // TODO?: Don't only look at the top one. You need to find the highest one (because of inserting).

        // Find starting point.
        if (_visibleStickerPool.Count > 0)
        {
            var previousSticker = _visibleStickerPool[_visibleStickerPool.Count - 1];
            var previousStickerBounds = previousSticker.gameObject.GetBounds();

            startStickerPosition = previousSticker.transform.position;
            startStickerPosition.y += previousStickerBounds.max.y - previousSticker.transform.position.y;

            startStickerPosition.y = Mathf.Clamp(startStickerPosition.y, _topAnchorPoint.transform.position.y, startStickerPosition.y);
        }
        else if (_topAnchorPoint != null)
        {
            startStickerPosition = _topAnchorPoint.transform.position;
        }

        // Activate the sticker.
        sticker.gameObject.SetActive(true);
        sticker.transform.localEulerAngles = new Vector3(0.0f, 0.0f, Random.Range(0.0f, 360.0f));

        // Offset it above.
        var stickerBounds = sticker.gameObject.GetBounds();

        startStickerPosition.y += sticker.transform.position.y - stickerBounds.min.y + Random.Range(_minVerticalOffset, _maxVerticalOffset);
        if (_leftAnchorPoint != null && _rightAnchorPoint != null)
        {
            if (_flipSide)
            {
                startStickerPosition.x = _leftAnchorPoint.position.x + Random.Range(_minHorizontalOffset, _maxHorizontalOffset);
            }
            else
            {
                startStickerPosition.x = _rightAnchorPoint.position.x + Random.Range(_minHorizontalOffset, _maxHorizontalOffset);
            }
            _flipSide = !_flipSide;
        }

        sticker.transform.position = startStickerPosition;

        // Z position (depth)
        var position = sticker.transform.position;
        position.z = transform.position.z - _visibleStickerPool.Count;
        sticker.transform.position = position;

        // Move it to the other pool.
        _availableStickerPool.Remove(sticker);
        _visibleStickerPool.Add(sticker);
    }

    private void CreateSticker(Object stickerObject)
    {
        if (_stickerPrefab == null)
        {
            return;
        }

        var sticker = GameObject.Instantiate<Sticker>(_stickerPrefab, transform, false);
        sticker.name = string.Format("Sticker_{0}", stickerObject.name);

        // Special case when they are Texture2Ds.
        if (stickerObject is Texture2D texture2D)
        {
            var stickerInstance = new GameObject(string.Format("Sticker_{0}", texture2D.name));
            stickerInstance.transform.parent = sticker.transform;
            stickerInstance.transform.localPosition = Vector3.zero;

            var spriteRenderer = stickerInstance.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Sprite.Create(texture2D, new Rect(0.0f, 0.0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 2);

            // Add collider.
            spriteRenderer.gameObject.AddComponent<PolygonCollider2D>();
        }
        else
        {
            // TODO: Check if there's anything interesting in there.
            var stickerInstance = GameObject.Instantiate(stickerObject, sticker.transform, false);
        }

        sticker.gameObject.SetActive(false);
        _stickers.Add(sticker);
    }
    
    private void ReorderAssignedStickers()
    {
        for (int i = 0; i < _assignedStickerPool.Count; ++i)
        {
            // Z position (depth)
            var position = _assignedStickerPool[i].transform.position;
            position.z = transform.position.z + AssignedZPositionStickerOffset - (_assignedStickerPool.Count - i);
            _assignedStickerPool[i].transform.position = position;
        }
    }
    #endregion
}
