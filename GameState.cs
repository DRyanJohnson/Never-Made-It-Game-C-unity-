using UnityEngine;
using UnityEngine.SceneManagement;

// Keeps track of player health (strikes) and items across different scenes
public static class GameState
{
    public static int strikes = 0;
    public static int maxStrikes = 3; // Player dies on the 3rd strike
    public static bool hasDogBone = false;
    public static bool hasDog = false;
    public static bool visitedSmoker = false;

    // Resets everything back to the start for a new game
    public static void ResetRun()
    {
        strikes = 0;
        hasDogBone = false;
        hasDog = false;
        visitedSmoker = false;
    }

    // Adds 1 strike (-1 health) and checks if the player died
    public static bool TakeStrike()
    {
        strikes = strikes + 1;

        if (strikes >= maxStrikes)
        {
            SceneManager.LoadScene("Scene_Death");
            return true; // Player died
        }

        return false; // Player is still alive
    }
}
