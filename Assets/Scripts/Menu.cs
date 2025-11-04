using UnityEngine;

public class Menu : MonoBehaviour
{
    public void StartButtonClick()
    {
        // log that button was clicked
        Debug.Log("Button clicked!");
        
        if (GameManager.instance != null)
        {
            GameManager.instance.StartNewGame();
        }
    }
}
