using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public enum GameState { Playing,  Menu }
public class GameManager : MonoBehaviour {
    // inspector settings

    // Game objects
    public GameObject asteroidPrefab;
    public GameObject playerShip;

    // UI GameObjects
    public GameObject MenuGui;
    public GameObject PlayingGui;

    // High score 
    private int HighScore;

    // Key for saving high score in PlayerPrefs - peristent storage for high scores
    private const string HighScoreKey = "HighScore";

    // Current game state set to Menu initiallly
    public GameState currentGameState = GameState.Menu;

    // Player lives
    public int playerLives = 3;

    // Player score
    public int score = 0;

    // UI text for playing gui
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public TMP_Text livesText;

    // Enum for asteroid size

    public enum AsteroidSize { Small, Big }

    public AsteroidSize asteroidSize = AsteroidSize.Big;

    // class-level statics
    public static GameManager instance;
    public static int currentGameLevel;
    public static Vector3 screenBottomLeft, screenTopRight;
    public static float screenWidth, screenHeight;
    //
    // Use this for initialization
    void Start()
    {

        currentGameState = GameState.Menu;
        UpdateGameState(currentGameState);
        instance = this;
        Camera.main.transform.position = new Vector3(0f, 30f, 0f);
        Camera.main.transform.LookAt(Vector3.zero, new Vector3(0f, 0f, 1f));       
        screenBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 30f));
        screenTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 30f));
        screenWidth = screenTopRight.x - screenBottomLeft.x;
        screenHeight = screenTopRight.z - screenBottomLeft.z;

        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        UpdateHighScoreText();
        

    }


    public static void StartNextLevel()
    {
        currentGameLevel++;
        // create some asteroids near the edges of the screen
        for (int i = 0; i < currentGameLevel * 2 + 3; i++)
        {
            GameObject go = Instantiate(instance.asteroidPrefab);
            Asteroid asteroid = go.GetComponent<Asteroid>();
            if (asteroid != null)
            {
                int randomSize = Random.Range(0, 3);
                asteroid.asteroidSize = (Asteroid.AsteroidSize)randomSize;

                if (instance.asteroidSize == AsteroidSize.Small)
                {
                    asteroid.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                }
                else{
                    asteroid.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
 
                }
            }
            float x, z;
            if (Random.Range(0f, 1f) < 0.5f)
                x = screenBottomLeft.x + Random.Range(0f, 0.15f) * screenWidth; // near the left edge
            else
                x = screenTopRight.x - Random.Range(0f, 0.15f) * screenWidth; // near the right edge
            if (Random.Range(0f, 1f) < 0.5f)
                z = screenBottomLeft.z + Random.Range(0f, 0.15f) * screenHeight; // near the bottom edge
            else
                z = screenTopRight.z - Random.Range(0f, 0.15f) * screenHeight; // near the top edge
            go.transform.position = new Vector3(x, 0f, z);
        }
    }

    // start next level if all asteroids destroyed
    public static void AsteroidDestroyed()
    {
        if (GameObject.FindGameObjectsWithTag("Asteroid").Length == 0)
            StartNextLevel();
    }
    
    //quit game method
    public void QuitGame()
    {
        Application.Quit();
    }

    // respawn method
    public void respawn()
    {
        if (playerShip == null)
        {
            Debug.LogError("GameManager: No playerShip prefab assigned");
            return;

        }

        // Check if an active spaceship already exists in the scene
        GameObject existingShip = GameObject.FindGameObjectWithTag("Spaceship");
        if (existingShip != null && existingShip.activeInHierarchy)
        {
            Debug.Log("GameManager: Spaceship already exists, skipping respawn");
            return;
        }

        Debug.Log("GameManager: Creating player ship");
        Vector3 pos = new Vector3(0f, 0f, 0f);
        Instantiate(playerShip, pos, Quaternion.identity);
    }

    public int GetScore()
    {
        return score;
    }
    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
        CheckHighScore();

    }

    // method to reset score and lives

    public void ResetVariables()
    {
        score = 0;
        UpdateScoreText();
        playerLives = 3;
        UpdateLivesText();
        currentGameLevel = 0;
    }

    // function to update game state
    public void UpdateGameState(GameState newState)
    {
        currentGameState = newState;
        switch (newState)
        {
            case GameState.Playing:
                // Handle transition to Playing state
                HandlePlayingState();
                Debug.Log("Game State changed to Playing");

                break;
            case GameState.Menu:
                // Handle transition to Menu state
                HandleMenuState();
                Debug.Log("Game State changed to Menu");
                break;
            default:
                Debug.LogWarning("Unknown Game State");
                break;
        }
    }
    private void HandlePlayingState()
    {
        MenuGui.SetActive(false);
        PlayingGui.SetActive(true);
    }

    private void HandleMenuState()
    {
        MenuGui.SetActive(true);
        PlayingGui.SetActive(false);
    }

    public void StartNewGame()
    {

        Debug.Log("Starting New Game");
        currentGameState = GameState.Playing;
        UpdateGameState(currentGameState);
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        respawn();

        StartNextLevel();
    }

    public void onQuitButtonPressed()
    {
        QuitGame();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    private void UpdateLivesText()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + playerLives.ToString();
        }
    }

    private void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "High Score: " + HighScore.ToString();
        }
    }

    public void CheckHighScore()
    {
        if (score > HighScore)
        {
            HighScore = score;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save();
            UpdateHighScoreText();

        }
    }

    public void LoseLife()
    {
        playerLives--;
        UpdateLivesText();
        if (playerLives <= 0)
        {
            Debug.Log("Game Over!");

            //Set values to reset game
            ResetVariables();

            // Destroy all remaining asteroids

            GameObject[] asteroids = GameObject.FindGameObjectsWithTag("Asteroid");
            foreach (GameObject asteroid in asteroids)
            {
                Destroy(asteroid);
            }
         
            UpdateGameState(GameState.Menu);

        }
        else
        {
            respawn();
        }
    }

    
}