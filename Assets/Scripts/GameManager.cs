using UnityEngine;
public class GameManager : MonoBehaviour {
    // inspector settings

    public GameObject asteroidPrefab;
    public GameObject playerShip;

    public enum AsteroidSize { Small, Big }
    public AsteroidSize asteroidSize = AsteroidSize.Big;
//
// class-level statics
public static GameManager instance;
public static int currentGameLevel;
public static Vector3 screenBottomLeft, screenTopRight;
public static float screenWidth, screenHeight;
    //
    // Use this for initialization
    void Start()
    {
        instance = this;
        Camera.main.transform.position = new Vector3(0f, 30f, 0f);
        Camera.main.transform.LookAt(Vector3.zero, new Vector3(0f, 0f, 1f));
        currentGameLevel = 0;
        // find screen corners and size, in world coordinates
        // for ViewportToWorldPoint, the z value specified is in world units from the camera
        screenBottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 30f));
        screenTopRight = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, 30f));
        screenWidth = screenTopRight.x - screenBottomLeft.x;
        screenHeight = screenTopRight.z - screenBottomLeft.z;
        respawn();

        StartNextLevel();

    }


    public static void StartNextLevel()
    {
        currentGameLevel++;
        // create some asteroids near the edges of the screen
        for (int i = 0; i < currentGameLevel * 2 + 3; i++)
        {
            GameObject go = Instantiate(instance.asteroidPrefab) as GameObject;
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
        else
        {
            Debug.Log("GameManager: Creating player ship");
            Vector3 pos = new Vector3(0f, 0f, 0f);
            Instantiate(playerShip, pos, Quaternion.identity);
        }
    }
}