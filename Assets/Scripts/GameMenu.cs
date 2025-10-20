// using UnityEngine;
// using UnityEngine.UI;

// // Menu script to handle game start
// public class GameMenu : MonoBehaviour
// {
//     public GameObject menuPanel;
//     public Button startButton;
//     private GameManager gameManager;

//     void Start()
//     {
//         gameManager = FindFirstObjectByType<GameManager>();
//         startButton.onClick.AddListener(StartGame);

//         // Pause the game initially
//         Time.timeScale = 0f;
//     }

//     void StartGame()
//     {
//         // Hide menu
//         menuPanel.SetActive(false);

//         // Resume game
//         Time.timeScale = 1f;

//         // Start the first level
//         gameManager.StartGame();
//     }
// }
