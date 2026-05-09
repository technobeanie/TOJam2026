using Common.Flow;
using Common.Joystick;
using Common.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GameSceneView : SokobanView
{
    // const
    public const string FlowParameter_Player1 = "FlowParameter_Player1";
    public const string FlowParameter_Player2 = "FlowParameter_Player2";
    public const string FlowParameter_Player1KeyboardId = "FlowParameter_Player1KeyboardId";
    public const string FlowParameter_Player2KeyboardId = "FlowParameter_Player2KeyboardId";

    // public

    // private
    private List<StickerPack> _packs = new List<StickerPack>();

    [Header("Belt")]
    [SerializeField] private ConveyBelt _belt = null;

    [Header("Stickers")]
    [SerializeField] private StickerPool _stickerPool = null;

    [Header("Players")]
    [SerializeField] private PlayerController _player1 = null;
    [SerializeField] private PlayerController _player2 = null;

    [Header("Debug")]
    [SerializeField] private List<StickerPack> _defaultPacks = new List<StickerPack>();

    // properties

    #region Unity Methods
    private void FixedUpdate()
    {
        if (ViewState != ViewStateIds.Opened)
        {
            return;
        }

        // TODO
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
                if (_parameters.ContainsKey(FlowParameter_Player1) && _parameters.ContainsKey(FlowParameter_Player1KeyboardId))
                {
                    var player1 = _parameters[FlowParameter_Player1] as InputDevice;
                    var player1Id = (int)_parameters[FlowParameter_Player1KeyboardId];

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
                if (_parameters.ContainsKey(FlowParameter_Player2) && _parameters.ContainsKey(FlowParameter_Player2KeyboardId))
                {
                    var player2 = _parameters[FlowParameter_Player2] as InputDevice;
                    var player1Id = (int)_parameters[FlowParameter_Player2KeyboardId];

                    _player2.AssignInputDevice(player2, player1Id);
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

            // TODO: Pass the sticker pack.
            _packs.Clear();
            _packs.AddRange(_defaultPacks);
        }
    }

    protected override void OnViewOpened()
    {
        base.OnViewOpened();

        // Let's begin the game.
        if (_belt != null)
        {
            _belt.Begin();
        }

        if (_stickerPool != null)
        {
            _stickerPool.Initiatize(_packs);
        }
    }

    protected override void OnViewClosed(Dictionary<string, object> parameters)
    {
        base.OnViewClosed(parameters);

        // TODO:
    }

    protected override void OnViewReturned(Dictionary<string, object> parameters)
    {
        base.OnViewReturned(parameters);

        // TODO:
    }
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    #endregion

    #region UI Methods
    #endregion
}
