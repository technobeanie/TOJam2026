using Common.Joystick;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VotingController : MonoBehaviour
{
    // const
    private const string VotesInTextFormat = "{0} Votes In";

    // public

    // protected

    // private
    public int VotesPlayer1
    {
        get; private set;
    }

    public int VotesPlayer2
    {
        get; private set;
    }

    private Action _onReady = null;

    [Header("Setup")]
    [SerializeField] private GameObject _titlePanel = null;
    [SerializeField] private GameObject _votingAmountPanel = null;
    [SerializeField] private GameObject _leftVotingText = null;
    [SerializeField] private GameObject _rightVotingText = null;
    [SerializeField] private GameObject _confirmText = null;
    [SerializeField] private ReadyRadial _radial = null;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _votingAmountText = null;

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

        if (JoystickManager.Instance.IsButtonDownThisFrame(JoystickManager.Button.Xbox_LB) || JoystickManager.Instance.IsButtonDownThisFrame(JoystickManager.Button.Xbox_Left_Trigger))
        {
            ++VotesPlayer1;
            UpdateVotes();
        }

        if (JoystickManager.Instance.IsButtonDownThisFrame(JoystickManager.Button.Xbox_RB) || JoystickManager.Instance.IsButtonDownThisFrame(JoystickManager.Button.Xbox_Right_Trigger))
        {
            ++VotesPlayer2;
            UpdateVotes();
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
        Hide();
    }

    public void Begin(Action onReady)
    {
        _onReady = onReady;

        _titlePanel.SetActive(true);
        _votingAmountPanel.SetActive(true);
        _leftVotingText.SetActive(true);
        _rightVotingText.SetActive(true);
        _confirmText.SetActive(false);

        VotesPlayer1 = 0;
        VotesPlayer2 = 0;

        _radial.Initialize();

        UpdateVotes();

        HasBegun = true;
    }

    public void Hide()
    {
        _titlePanel.SetActive(false);
        _votingAmountPanel.SetActive(false);
        _leftVotingText.SetActive(false);
        _rightVotingText.SetActive(false);
        _confirmText.SetActive(false);

        HasBegun = false;
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    private void UpdateVotes()
    {
        int totalVotes = VotesPlayer1 + VotesPlayer2;
        _votingAmountText.text = string.Format(VotesInTextFormat, totalVotes);

        if (!_confirmText.activeSelf)
        {
            _confirmText.SetActive(totalVotes > 0);
        }
    }

    private void OnReady()
    {
        HasBegun = false;

        _onReady?.Invoke();
        _onReady = null;
    }
    #endregion
}
