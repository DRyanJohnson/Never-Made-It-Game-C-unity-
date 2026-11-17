using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controls button interactions on the Main Menu screen,
/// including starting the game and toggling the rules panel.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    // The exact name of the first gameplay scene to load
    [SerializeField] private string firstSceneName = "Scene1_Platform";

    // Reference to the UI Panel that displays the survival rules
    [SerializeField] private GameObject survivePanel;

    /// <summary>
    /// Loads the opening gameplay scene when the Play button is clicked.
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene(firstSceneName);
    }

    /// <summary>
    /// Makes the rules panel visible when the "How to Survive" button is clicked.
    /// </summary>
    public void OpenHowToPlay()
    {
        if (survivePanel != null)
        {
            survivePanel.SetActive(true);
        }
    }

    /// <summary>
    /// Hides the rules panel when the "Back" button is clicked.
    /// </summary>
    public void CloseHowToPlay()
    {
        if (survivePanel != null)
        {
            survivePanel.SetActive(false);
        }
    }
}