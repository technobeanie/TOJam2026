using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

public class PackSelectionView : SokobanView
{
    // const

    // public

    // protected

    // private
    [Header("Players")]
    [SerializeField] private PlayerController _player1 = null;
    [SerializeField] private PlayerController _player2 = null;

    [Header("Packs")]
    [SerializeField] private List<StickerPack> _allPacks = new List<StickerPack>();
    [SerializeField] private List<PackController> _availablePacks = new List<PackController>();

    // properties

    #region Unity Methods
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

        if (_player1 != null)
        {
            _player1.Initialize(null);
        }

        if (_player2 != null)
        {
            _player2.Initialize(null);
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
                packController.Initialize(pack);
            }
            else
            {
                break;
            }

            ++i;
        }
    }
    #endregion
}
