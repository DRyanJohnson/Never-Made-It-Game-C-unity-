using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0.25f;
    public float shakeStrength = 0.12f;

    private Vector3 startPos;
    private float timer = 0f;

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
            float x = Random.Range(-1f, 1f) * shakeStrength;
            float y = Random.Range(-1f, 1f) * shakeStrength;

            transform.localPosition = new Vector3(startPos.x + x, startPos.y + y, startPos.z);
            timer -= Time.deltaTime;
        }
        else
        {
            // Put the camera back when finished
            transform.localPosition = startPos;
        }
    }

    // Call this method to trigger the camera jolt
    public void StartShake()
    {
        timer = shakeDuration;
    }
}