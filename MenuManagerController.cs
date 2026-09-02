using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Holds the main menu UI panel so it can be hidden when starting the game
    public GameObject mainMenuUI;

    public void PlayGame()
    {
        // Hides the main menu UI if it has been assigned
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(false);
        }

        // Loads the tower defense gameplay scene
        SceneManager.LoadScene("Final Game");
    }
}