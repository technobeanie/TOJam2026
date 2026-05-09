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
    [Header("Setup")]
    [SerializeField] private GameObject _medal1 = null;
    [SerializeField] private GameObject _medal2 = null;
    [SerializeField] private GameObject _titlePanel = null;
    [SerializeField] private GameObject _holdPanel = null;
    [SerializeField] private ReadyRadial _radial = null;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _winningText = null;
    [SerializeField] private TextMeshProUGUI _leftVotesCountText = null;
    [SerializeField] private TextMeshProUGUI _rightVotesCountText = null;

    // properties

    #region Unity Methods
    #endregion

    #region Public Methods
    public void Initialize()
    {
        _medal1.SetActive(false);
        _medal2.SetActive(false);

        _titlePanel.SetActive(false);
        _holdPanel.SetActive(false);

        _leftVotesCountText.gameObject.SetActive(false);
        _rightVotesCountText.gameObject.SetActive(false);

        _winningText.gameObject.SetActive(false);

        _radial.gameObject.SetActive(false);
    }

    public void Begin(int player1Votes, int player2Votes)
    {
        _titlePanel.SetActive(true);
        _holdPanel.SetActive(true);

        _winningText.gameObject.SetActive(true);

        if (player1Votes > player2Votes)
        {
            _medal1.SetActive(true);
            _winningText.text = string.Format(WinningTextFormat, 1);
        }
        else if (player1Votes < player2Votes)
        {
            _medal2.SetActive(true);
            _winningText.text = string.Format(WinningTextFormat, 2);
        }
        // DRAW
        else
        {
            _medal1.SetActive(true);
            _medal2.SetActive(true);
            _winningText.text = "Draw!";
        }

        _leftVotesCountText.gameObject.SetActive(true);
        _rightVotesCountText.gameObject.SetActive(true);

        _leftVotesCountText.text = string.Format(VotesTextFormat, player1Votes);
        _rightVotesCountText.text = string.Format(VotesTextFormat, player2Votes);

        _radial.gameObject.SetActive(true);

        // TODO: Radial to start a new match.
    }
    #endregion

    #region Protected Methods
    #endregion

    #region Private Methods
    #endregion
}
