using UnityEngine;

// Scrolls the text box left to right based on its own live RectTransform width
public class ScrollingSignText : MonoBehaviour
{
    [SerializeField] private RectTransform textRect;
    [SerializeField] private float scrollSpeed = 120f;

    private bool initialized = false;

    private void Start()
    {
        if (textRect == null)
        {
            textRect = GetComponent<RectTransform>();
        }
    }

    private void Update()
    {
        float boxWidth = textRect.rect.width;

        // Wait until Unity finishes sizing the UI box before placing it on the left
        if (!initialized && boxWidth > 0f)
        {
            textRect.localPosition = new Vector3(-boxWidth, textRect.localPosition.y, 0f);
            initialized = true;
        }

        // Move the text box right every frame
        textRect.localPosition += Vector3.right * scrollSpeed * Time.deltaTime;

        // Reset to the left edge as soon as it crosses the right edge
        if (textRect.localPosition.x >= boxWidth)
        {
            textRect.localPosition = new Vector3(-boxWidth, textRect.localPosition.y, 0f);
        }
    }
}