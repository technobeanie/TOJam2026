using Common.Flow;
using Common.Joystick;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinningController : MonoBehaviour
{
    // const
    private const string VotesTextFormat = "{0} Votes!";
    private const string WinningTextFormat = "Congrats Player {0}!";

    // public

    // protected

    // private
    private Action _onNewGame = null;

    [Header("Setup")]
    [SerializeField] private GameObject _medal1 = null;
    [SerializeField] private GameObject _medal2 = null;
    [SerializeField] private GameObject _titlePanel = null;
    [SerializeField] private GameObject _holdPanel = null;
    [SerializeField] private ReadyRadial _radial = null;
    [SerializeField] private LittleGuy _guy = null;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _winningText = null;
    [SerializeField] private TextMeshProUGUI _leftVotesCountText = null;
    [SerializeField] private TextMeshProUGUI _rightVotesCountText = null;

    [Header("Timing")]
    [SerializeField] private Vector3 _packPunch = Vector3.zero;
    [SerializeField] private float _packPunchDuration = 0.5f;

    // properties
    public bool HasBegun
    {
        get; private set;
    }

    #region Unity Methods
    private void FixedUpdate()
    {
        if (!HasBegun)
        {
            return;
        }

        if (JoystickManager.Instance.IsButtonDown(JoystickManager.Button.Xbox_Y))
        {
            if (!_radial.IsHolding)
            {
                _radial.BeginHold(OnReady);
            }
        }
        else if (_radial.IsHolding)
        {
            _radial.StopHold();
        }
    }
    #endregion

    #region Public Methods
    public void Initialize()
    {
        HasBegun = false;

        _guy.Initialize();

        Hide();
    }

    public void Begin(int player1Votes, int player2Votes, Action onNewGame)
    {
        HasBegun = true;

        _onNewGame = onNewGame;

        _titlePanel.SetActive(true);
        _holdPanel.SetActive(true);

        _winningText.gameObject.SetActive(true);

        if (player1Votes > player2Votes)
        {
            _medal1.SetActive(true);
            _medal1.transform.DOPunchScale(_packPunch, _packPunchDuration);

            _winningText.text = string.Format(WinningTextFormat, 1);
        }
        else if (player1Votes < player2Votes)
        {
            _medal2.SetActive(true);
            _medal2.transform.DOPunchScale(_packPunch, _packPunchDuration);

            _winningText.text = string.Format(WinningTextFormat, 2);
        }
        // DRAW
        else
        {
            _medal1.SetActive(true);
            _medal2.SetActive(true);
            _medal1.transform.DOPunchScale(_packPunch, _packPunchDuration);
            _medal2.transform.DOPunchScale(_packPunch, _packPunchDuration);

            _winningText.text = "Draw!";
        }

        _leftVotesCountText.gameObject.SetActive(true);
        _rightVotesCountText.gameObject.SetActive(true);

        _leftVotesCountText.text = string.Format(VotesTextFormat, player1Votes);
        _rightVotesCountText.text = string.Format(VotesTextFormat, player2Votes);

        _radial.gameObject.SetActive(true);

        _guy.Show();
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void OnReady()
    {
        HasBegun = false;

        _holdPanel.SetActive(false);

        _onNewGame?.Invoke();
        _onNewGame = null;
    }

    private void Hide()
    {
        _medal1.SetActive(false);
        _medal2.SetActive(false);

        _titlePanel.SetActive(false);
        _holdPanel.SetActive(false);

        _leftVotesCountText.gameObject.SetActive(false);
        _rightVotesCountText.gameObject.SetActive(false);

        _winningText.gameObject.SetActive(false);

        _radial.gameObject.SetActive(false);

        _guy.Hide();
    }
    #endregion
}
