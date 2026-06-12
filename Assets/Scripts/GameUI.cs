using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject gameLoseUI;
    public GameObject gameWinUI;
    bool gameIsOver;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Guard.onGuardHasSpottedPlayer += ShowGameLoseUI;
        FindAnyObjectByType<Player>().onReachedEndOfLevel += ShowGameWinUI;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    void ShowGameWinUI()
    {
        onGameOver(gameWinUI);
    }

    void ShowGameLoseUI()
    {
        onGameOver(gameLoseUI);
    }

    void onGameOver(GameObject gameOverUI)
    {
        gameOverUI.SetActive(true);
        gameIsOver = true;
        Guard.onGuardHasSpottedPlayer -= ShowGameLoseUI;
        FindAnyObjectByType<Player>().onReachedEndOfLevel -= ShowGameWinUI;
    }
}
