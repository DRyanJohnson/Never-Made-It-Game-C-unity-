using UnityEngine;

// Handles short camera shake effects during an attack on player
public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0.25f; // How long the shake lasts in seconds
    public float shakeStrength = 0.12f; // How far the camera moves during the shake

    private Vector3 startPos; // Original camera position to return to
    private float timer = 0f; // Tracks remaining shake time

    void Start()
    {
        // Save the camera's starting position
        startPos = transform.localPosition;
    }

    void Update()
    {
        // Shake the camera while the timer is counting down
        if (timer > 0f)
        {
            // Pick a random X and Y offset around the starting position
            float x = Random.Range(-1f, 1f) * shakeStrength;
            float y = Random.Range(-1f, 1f) * shakeStrength;

            // Apply the offset while keeping the original Z depth
            transform.localPosition = new Vector3(startPos.x + x, startPos.y + y, startPos.z);
            timer -= Time.deltaTime;
        }
        else
        {
            // Reset the camera back to its original position when done
            transform.localPosition = startPos;
        }
    }

    // Call this from other scripts to start the camera shake
    public void StartShake()
    {
        timer = shakeDuration;
    }
}

