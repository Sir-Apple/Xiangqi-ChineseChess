using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [Header("Winner Panel")]
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private Button restartButton;

	// Start is called before the first frame update
	void Start()
    {
        gamePanel.SetActive(false);
        restartButton.onClick.AddListener(RestartGame);
	}

	public void ShowWinner(int winningTeam)
    {
        string winner = winningTeam == 0 ? "Red" : "Blue";
        winnerText.text = winner + "Wins by checkmate";
        gamePanel.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
