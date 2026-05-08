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
    [SerializeField] private PlayerController _player1 = null;
    [SerializeField] private PlayerController _player2 = null;

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

        if (IsReady)
        {
            if (JoystickManager.Instance.IsButtonDownThisFrame(JoystickManager.Button.Xbox_Menu))
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
        }
        if (_player2 != null)
        {
            _player2.Initialize();
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
    private void OnJoin(InputDevice inputDevice)
    {
        if (_player1 != null && _player2 != null)
        {
            if (!_player1.HasJoined)
            {
                _player1.AssignInputDevice(inputDevice);
            }
            else if (!_player2.HasJoined)
            {
                _player2.AssignInputDevice(inputDevice);
            }
        }

        SetIsReady();
    }

    private void OnLeave(InputDevice inputDevice)
    {
        if (_player1 == null || _player2 == null)
        {
            return;
        }

        if (_player1.InputDevice == inputDevice)
        {
            _player1.AssignInputDevice(null);
        }
        else if (_player2.InputDevice == inputDevice)
        {
            _player2.AssignInputDevice(null);
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
        parameters.Add(GameSceneView.FlowParameter_Player2, _player2.InputDevice);

        FlowManager.Instance.OpenView("GameScene", parameters);
    }
    #endregion
}
