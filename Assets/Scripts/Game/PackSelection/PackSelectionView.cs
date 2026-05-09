using Common.Flow;
using Common.Joystick;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public class PackSelectionView : SokobanView
{
    // const

    // public

    // protected

    // private
    private List<StickerPack> _selectedStickerPacks = new List<StickerPack>();

    [Header("Players")]
    [SerializeField] private PlayerPackController _player1 = null;
    [SerializeField] private PlayerPackController _player2 = null;

    [Header("Packs")]
    [SerializeField] private List<StickerPack> _allPacks = new List<StickerPack>();
    [SerializeField] private List<PackController> _availablePacks = new List<PackController>();
    [SerializeField] private int _openedRequiredAmount = 2;
    [SerializeField] private int _openedByPlayer = 1;

    [Header("Theme")]
    [SerializeField] private Theme _theme = null;
    [SerializeField] private TextMeshProUGUI _themeText = null;

    [Header("UI")]
    [SerializeField] private GameObject _beginText = null;

    // properties
    private bool IsReady
    {
        get; set;
    }

    #region Unity Methods
    private void FixedUpdate()
    {
        if (ViewState != ViewStateIds.Opened)
        {
            return;
        }

        // TEMP. Go back.
        if (Keyboard.current != null && Keyboard.current[Key.Escape].wasPressedThisFrame)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add(GameSceneView.FlowParameter_Player1, _player1.InputDevice);
            parameters.Add(GameSceneView.FlowParameter_Player1KeyboardId, _player1.KeyboardPlayerId);

            parameters.Add(GameSceneView.FlowParameter_Player2, _player2.InputDevice);
            parameters.Add(GameSceneView.FlowParameter_Player2KeyboardId, _player2.KeyboardPlayerId);

            FlowManager.Instance.OpenView("PlayerSelection", parameters);
        }

        if (IsReady)
        {
            if (JoystickManager.Instance.IsButtonDownThisFrame(JoystickManager.Button.Xbox_Menu))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();

                parameters.Add(GameSceneView.FlowParameter_Player1, _player1.InputDevice);
                parameters.Add(GameSceneView.FlowParameter_Player1KeyboardId, _player1.KeyboardPlayerId);

                parameters.Add(GameSceneView.FlowParameter_Player2, _player2.InputDevice);
                parameters.Add(GameSceneView.FlowParameter_Player2KeyboardId, _player2.KeyboardPlayerId);

                parameters.Add(GameSceneView.FlowParameter_Theme, _themeText.text);
                parameters.Add(GameSceneView.FlowParameter_Packs, _selectedStickerPacks);

                FlowManager.Instance.OpenView("GameScene", parameters);
            }
        }
    }
    #endregion

    #region View Methods
    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        if (_parameters != null)
        {
            if (_player1 != null)
            {
                if (_parameters.ContainsKey(GameSceneView.FlowParameter_Player1) && _parameters.ContainsKey(GameSceneView.FlowParameter_Player1KeyboardId))
                {
                    var player1 = _parameters[GameSceneView.FlowParameter_Player1] as InputDevice;
                    var player1Id = (int)_parameters[GameSceneView.FlowParameter_Player1KeyboardId];

                    _player1.AssignInputDevice(player1, player1Id);
                }
                else
                {
                    // CHEAT
                    if (Gamepad.all.Count > 0)
                    {
                        _player1.AssignInputDevice(Gamepad.all[0], 0);
                    }
                    else
                    {
                        _player1.AssignInputDevice(Keyboard.current, 0);
                    }
                }
            }

            if (_player2 != null)
            {
                if (_parameters.ContainsKey(GameSceneView.FlowParameter_Player2) && _parameters.ContainsKey(GameSceneView.FlowParameter_Player2KeyboardId))
                {
                    var player2 = _parameters[GameSceneView.FlowParameter_Player2] as InputDevice;
                    var player2Id = (int)_parameters[GameSceneView.FlowParameter_Player2KeyboardId];

                    _player2.AssignInputDevice(player2, player2Id);
                }
                else
                {
                    // CHEAT
                    if (Gamepad.all.Count > 1)
                    {
                        _player2.AssignInputDevice(Gamepad.all[1], 0);
                    }
                    else
                    {
                        _player2.AssignInputDevice(Keyboard.current, 1);
                    }
                }
            }
        }

        if (_theme != null)
        {
            // CHEAT
            if (_theme._themes.Count > 0)
            {
                var theme = _theme._themes[UnityEngine.Random.Range(0, _theme._themes.Count)];
                _themeText.text = theme;
            }
        }

        if (_player1 != null)
        {
            _player1.Initialize(null);
            _player1.MaxToOpen = _openedByPlayer;
        }

        if (_player2 != null)
        {
            _player2.Initialize(null);
            _player2.MaxToOpen = _openedByPlayer;
        }

        IsReady = false;

        if (_beginText != null)
        {
            _beginText.SetActive(false);
        }

        SelectPacks();
    }

    protected override void OnViewOpened()
    {
        base.OnViewOpened();

        // Begin!
    }
    #endregion

    #region Public Methods
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void SelectPacks()
    {
        var allPacks = new List<StickerPack>(_allPacks);
        allPacks.Shuffle();

        int i = 0;
        foreach (var packController in _availablePacks)
        {
            if (i < allPacks.Count)
            {
                var pack = allPacks[i];
                packController.Initialize(pack, OnOpened);
            }
            else
            {
                break;
            }

            ++i;
        }
    }

    private void OnOpened(PackController packController)
    {
        _selectedStickerPacks.Add(packController.Pack);

        if (_selectedStickerPacks.Count >= _openedRequiredAmount)
        {
            IsReady = true;

            if (_player1 != null)
            {
                _player1.GameDone();
            }

            if (_player2 != null)
            {
                _player2.GameDone();
            }

            if (_beginText != null)
            {
                _beginText.SetActive(true);
            }
        }
    }
    #endregion
}
