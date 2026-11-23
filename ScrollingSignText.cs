using UnityEngine;

// Scrolls the station sign text continuously from left to right across the sign box
public class ScrollingSignText : MonoBehaviour
{
    [SerializeField] private RectTransform textRect; // UI transform for the scrolling text
    [SerializeField] private float scrollSpeed = 120f; // Speed of the scroll in pixels per second

    private bool initialized = false; // Tracks if the starting position has been set yet

    private void Start()
    {
        // Grab the RectTransform on this object if one wasn't assigned in the Inspector
        if (textRect == null)
        {
            textRect = GetComponent<RectTransform>();
        }
    }

    private void Update()
    {
        float boxWidth = textRect.rect.width;

        // Wait until Unity finishes sizing the UI box, then move it off-screen to the left
        if (!initialized && boxWidth > 0f)
        {
            textRect.localPosition = new Vector3(-boxWidth, textRect.localPosition.y, 0f);
            initialized = true;
        }

        // Slide the text box to the right every frame
        textRect.localPosition += Vector3.right * scrollSpeed * Time.deltaTime;

        // Once the text scrolls completely past the right edge, loop it back to the left
        if (textRect.localPosition.x >= boxWidth)
        {
            textRect.localPosition = new Vector3(-boxWidth, textRect.localPosition.y, 0f);
        }
    }
}
