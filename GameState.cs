using UnityEngine;
using UnityEngine.SceneManagement;

// Static class that stores player health (strikes) and story flags across all scenes
public static class GameState
{
    public static int strikes = 0; // Current number of damage taken
    public static int maxStrikes = 3; // Player dies on the 3rd strike

    // Story flags for items and locations visited during the current run
    public static bool hasDogBone = false;
    public static bool hasDog = false;
    public static bool visitedSmoker = false;

    // Resets all stats and flags back to default when starting a new playthrough
    public static void ResetRun()
    {
        strikes = 0;
        hasDogBone = false;
        hasDog = false;
        visitedSmoker = false;
    }

    // Adds 1 strike and sends the player to the death scene if they hit max strikes
    public static bool TakeStrike()
    {
        strikes = strikes + 1;

        // Check if the player has run out of chances
        if (strikes >= maxStrikes)
        {
            SceneManager.LoadScene("Scene_Death");
            return true; // Player died
        }

        return false; // Player is still alive
    }
}
