using UnityEngine;

// Keeps the background audio playing across scene loads without duplicating it
public class BackgroundMusic : MonoBehaviour
{
    // Holds the active instance so other scenes can see it already exists
    private static BackgroundMusic instance;

    void Awake()
    {
        // If another music object is already running, destroy this duplicate
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // Set this as the main instance and stop Unity from destroying it on scene changes
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
