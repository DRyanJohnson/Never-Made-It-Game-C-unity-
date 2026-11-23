using UnityEngine;
using UnityEngine.SceneManagement;

// Handles main menu buttons for starting the game and opening/closing the rules panel
public class MainMenuController : MonoBehaviour
{
    // first gameplay scene
    [SerializeField] private string firstSceneName = "Scene1_Platform";

    // UI panel that shows the survival rules
    [SerializeField] private GameObject survivePanel;

    // Loads the first scene when the Play button is clicked
    public void PlayGame()
    {
        SceneManager.LoadScene(firstSceneName);
    }

    // Turns on the rules panel when "How to Survive" is clicked
    public void OpenHowToPlay()
    {
        if (survivePanel != null)
        {
            survivePanel.SetActive(true);
        }
    }

    // Hides the rules panel when "Back" is clicked
    public void CloseHowToPlay()
    {
        if (survivePanel != null)
        {
            survivePanel.SetActive(false);
        }
    }
}
