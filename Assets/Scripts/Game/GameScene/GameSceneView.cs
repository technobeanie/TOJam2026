using Common.Flow;
using Common.Joystick;
using Common.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
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
    private int _playerReadyCount = 0;
    private Timer _readyTimer = null;

    [Header("Belt")]
    [SerializeField] private ConveyBelt _belt = null;

    [Header("Stickers")]
    [SerializeField] private StickerPool _stickerPool = null;

    [Header("Players")]
    [SerializeField] private PlayerController _player1 = null;
    [SerializeField] private PlayerController _player2 = null;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _timer = null;
    [SerializeField] private int _readyTimerDuration = 30;
    [SerializeField] private VotingController _votingController = null;
    [SerializeField] private WinningController _winningController = null;

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

        if (_readyTimer != null)
        {
            _readyTimer.Step(Time.fixedDeltaTime);
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
                    var player2Id = (int)_parameters[FlowParameter_Player2KeyboardId];

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

            // TODO: Pass the sticker pack.
            _packs.Clear();
            _packs.AddRange(_defaultPacks);
        }

        if (_stickerPool != null)
        {
            _stickerPool.Initiatize(_packs);
        }

        if (_player1 != null)
        {
            _player1.Initialize(OnReady);
        }

        if (_player2 != null)
        {
            _player2.Initialize(OnReady);
        }

        if (_votingController != null)
        {
            _votingController.Initialize();
        }

        if (_winningController != null)
        {
            _winningController.Initialize();
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

        _playerReadyCount = 0;
        _readyTimer = null;
        SetTimerVisuals(false, 0.0f);
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
    private void OnReady(PlayerController player)
    {
        ++_playerReadyCount;
        if (_playerReadyCount == 1)
        {
            BeginReadyTimer();
        }
        else if (_playerReadyCount > 1)
        {
            OnReadyPhaseCompleted(null);
        }

        player.GameDone();
    }

    private void BeginReadyTimer()
    {
        _readyTimer = new Timer(_readyTimerDuration, OnReadyPhaseCompleted, false, OnReadyTimeUpdated);
        _readyTimer.Begin();

        SetTimerVisuals(true, _readyTimerDuration);
    }

    private void SetTimerVisuals(bool isActive, float time)
    {
        if (_timer == null)
        {
            return;
        }

        if (_timer.gameObject.activeSelf != isActive)
        {
            _timer.gameObject.SetActive(isActive);
        }

        _timer.text = Mathf.CeilToInt(time).ToString();
    }

    private void OnReadyTimeUpdated(Timer timer, float progress)
    {
        SetTimerVisuals(true, timer.CurrentTime);
    }

    private void OnReadyPhaseCompleted(Timer timer)
    {
        if (_readyTimer != null)
        {
            _readyTimer.Stop();
            _readyTimer = null;
        }

        SetTimerVisuals(false, 0.0f);
        if (_stickerPool != null)
        {
            _stickerPool.StopSpawning();
        }

        if (_player1 != null)
        {
            _player1.GameDone();
        }

        if (_player2 != null)
        {
            _player2.GameDone();
        }

        // Start Voting!
        if (_votingController != null)
        {
            _votingController.Begin(OnVotingDone);
        }
    }

    private void OnVotingDone()
    {
        if (_votingController != null)
        {
            _votingController.Hide();
        }

        if (_winningController != null)
        {
            _winningController.Begin(_votingController.VotesPlayer1, _votingController.VotesPlayer2, OnNewGame);
        }
    }

    private void OnNewGame()
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>();

        parameters.Add(GameSceneView.FlowParameter_Player1, _player1.InputDevice);
        parameters.Add(GameSceneView.FlowParameter_Player1KeyboardId, _player1.KeyboardPlayerId);

        parameters.Add(GameSceneView.FlowParameter_Player2, _player2.InputDevice);
        parameters.Add(GameSceneView.FlowParameter_Player2KeyboardId, _player2.KeyboardPlayerId);

        FlowManager.Instance.OpenView("GameScene", parameters, false, "Loading");
    }
    #endregion

    #region UI Methods
    #endregion
}
