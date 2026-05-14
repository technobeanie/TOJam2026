using Common.Flow;
using Common.Joystick;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSelectionView : SokobanView
{
    // const

    // public

    // protected

    // private
    [Header("Players")]
    [SerializeField] private PlayerManager _playerManager = null;
    [SerializeField] private PlayerSelectionController _player1 = null;
    [SerializeField] private PlayerSelectionController _player2 = null;

    [Header("")]
    [SerializeField] private Color _player1Color = Color.white;
    [SerializeField] private Color _player2Color = Color.white;

    [Header("Ready")]
    [SerializeField] private Transform _readyText = null;

    // properties
    private bool IsReady
    {
        get 
        {
            if (_player1 != null && _player2 != null)
            {
                return _player1.HasJoined && _player2.HasJoined;
            }
            return false;
        }
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
            FlowManager.Instance.OpenView("TitleScreen", null, false, "Loading");
        }

        if (IsReady)
        {
            InputDevice inputDevice = null;
            if (JoystickManager.Instance.IsButtonDownThisFrame(JoystickManager.Button.Xbox_Menu, out inputDevice))
            {
                UI_BeginGame();
            }
        }
    }
    #endregion

    #region View Methods
    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        if (_player1 != null)
        {
            _player1.Initialize();

            if (_parameters != null && _parameters.ContainsKey(GameSceneView.FlowParameter_Player1) && _parameters.ContainsKey(GameSceneView.FlowParameter_Player1KeyboardId))
            {
                var player1 = _parameters[GameSceneView.FlowParameter_Player1] as InputDevice;
                var player1Id = (int)_parameters[GameSceneView.FlowParameter_Player1KeyboardId];

                _player1.AssignInputDevice(player1, player1Id);
                if (_playerManager != null)
                {
                    _playerManager.OnJoin(player1, player1Id);
                }

                SetIsReady();
            }
        }

        if (_player2 != null)
        {
            _player2.Initialize();

            if (_parameters != null && _parameters.ContainsKey(GameSceneView.FlowParameter_Player2) && _parameters.ContainsKey(GameSceneView.FlowParameter_Player2KeyboardId))
            {
                var player2 = _parameters[GameSceneView.FlowParameter_Player2] as InputDevice;
                var player2Id = (int)_parameters[GameSceneView.FlowParameter_Player2KeyboardId];

                _player2.AssignInputDevice(player2, player2Id);
                if (_playerManager != null)
                {
                    _playerManager.OnJoin(player2, player2Id);
                }

                SetIsReady();
            }
        }

        SetIsReady();
    }

    protected override void OnViewOpened()
    {
        base.OnViewOpened();

        // Let's begin the game.
        _playerManager.StartDetectingPlayers(OnJoin, OnLeave);
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

    #region Protected Methods
    #endregion

    #region Private Methods
    private void OnJoin(InputDevice inputDevice, int keyboardPlayerId)
    {
        if (_player1 != null && _player2 != null)
        {
            if (!_player1.HasJoined)
            {
                _player1.AssignInputDevice(inputDevice, keyboardPlayerId);
                JoystickManager.Instance.SetColor(inputDevice, _player1Color);
            }
            else if (!_player2.HasJoined)
            {
                _player2.AssignInputDevice(inputDevice, keyboardPlayerId);
                JoystickManager.Instance.SetColor(inputDevice, _player2Color);
            }
        }

        SetIsReady();
    }

    private void OnLeave(InputDevice inputDevice, int keyboardPlayerId)
    {
        if (_player1 == null || _player2 == null)
        {
            return;
        }

        if (_player1.InputDevice == inputDevice)
        {
            _player1.AssignInputDevice(null, keyboardPlayerId);
            JoystickManager.Instance.ResetColor(inputDevice);
        }
        else if (_player2.InputDevice == inputDevice)
        {
            _player2.AssignInputDevice(null, keyboardPlayerId);
            JoystickManager.Instance.ResetColor(inputDevice);
        }

        SetIsReady();
    }

    private void SetIsReady()
    {
        if (_readyText != null)
        {
            _readyText.gameObject.SetActive(IsReady);
        }
    }
    #endregion

    #region UI
    public void UI_BeginGame()
    {
        if (_player1 == null || _player2 == null)
        {
            return;
        }

        Dictionary<string, object> parameters = new Dictionary<string, object>();

        parameters.Add(GameSceneView.FlowParameter_Player1, _player1.InputDevice);
        parameters.Add(GameSceneView.FlowParameter_Player1KeyboardId, _player1.KeyboardPlayerId);

        parameters.Add(GameSceneView.FlowParameter_Player2, _player2.InputDevice);
        parameters.Add(GameSceneView.FlowParameter_Player2KeyboardId, _player2.KeyboardPlayerId);

        FlowManager.Instance.OpenView("PackSelection", parameters, false, "Loading");
    }
    #endregion
}
