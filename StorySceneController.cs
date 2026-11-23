using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// Main controller for dialogue beats, typewriter text, branching choices, and scene transitions
public class StorySceneController : MonoBehaviour
{
    // Conditions checked against GameState to decide if a choice button should show up
    public enum ConditionType
    {
        None,
        RequiresDogBone,
        RequiresDog,
        RequiresNoDog,
        RequiresNotVisitedSmoker
    }

    // Actions that happen in GameState when the player clicks a choice
    public enum StateEffect
    {
        None,
        TakeOneStrike,
        GiveDogBone,
        AdoptDog,
        MarkSmokerVisited,
        InstantDeath,
        ResetRun
    }

    // Holds the text, condition, state change, and next destination for a single choice button
    [System.Serializable]
    public class ChoiceOption
    {
        public string buttonLabel;
        public ConditionType showCondition = ConditionType.None;
        public StateEffect effect = StateEffect.None;
        public string targetSceneName;
        public int nextBeatIndex = -1;
    }

    // Represents one screen of dialogue, portrait art, sound effect, and choices
    [System.Serializable]
    public class StoryBeat
    {
        public string speakerName;
        [TextArea(3, 6)]
        public string dialogueText;
        public Sprite portraitSprite;
        public AudioClip beatSFX;
        public bool shakeOnStart;
        public ChoiceOption[] choices;
    }

    [Header("UI References")]
    public TMP_Text speakerText;
    public TMP_Text dialogueText;
    public Image portraitBox;
    public Button[] choiceButtons;
    public TMP_Text[] choiceButtonLabels;

    [Header("Typewriter Speed")]
    public float typingSpeed = 0.025f;

    [Header("Audio Clips")]
    public AudioSource sfxSource;
    public AudioClip clickSound;
    public AudioClip typeSound;

    [Header("Scene Story Beats")]
    public StoryBeat[] beats;

    // Keeps track of the currently running typewriter coroutine so we can stop it if needed
    private Coroutine typingCoroutine;

    void Awake()
    {
        // Make sure the Main Camera has a CameraShake script attached before the scene starts
        if (Camera.main != null && Camera.main.GetComponent<CameraShake>() == null)
        {
            Camera.main.gameObject.AddComponent<CameraShake>();
        }
    }

    void Start()
    {
        // Load the first dialogue beat when the scene opens
        if (beats != null && beats.Length > 0)
        {
            ShowBeat(0);
        }
    }

    // Updates the UI, portrait, audio, and starts typing out the dialogue for a given beat index
    public void ShowBeat(int index)
    {
        // Stop if the index is out of bounds
        if (index < 0 || index >= beats.Length)
        {
            return;
        }

        StoryBeat currentBeat = beats[index];

        // Set the speaker name
        if (speakerText != null)
        {
            speakerText.text = currentBeat.speakerName;
        }

        // Show the character portrait if one is assigned, otherwise hide the portrait box
        if (portraitBox != null)
        {
            if (currentBeat.portraitSprite != null)
            {
                portraitBox.gameObject.SetActive(true);
                portraitBox.sprite = currentBeat.portraitSprite;
            }
            else
            {
                portraitBox.gameObject.SetActive(false);
            }
        }

        // Play the beat's sound effect if one exists
        if (sfxSource != null && currentBeat.beatSFX != null)
        {
            sfxSource.PlayOneShot(currentBeat.beatSFX);
        }

        // Shake the camera at the start of the beat if checked in the Inspector
        if (currentBeat.shakeOnStart)
        {
            TriggerCameraShake();
        }

        // Hide all choice buttons until the dialogue finishes typing
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] != null)
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }

        // Stop any previous typing animation before starting the new one
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeDialogue(currentBeat));
    }

    // Types out the dialogue text one character at a time, then shows the choice buttons
    IEnumerator TypeDialogue(StoryBeat beat)
    {
        dialogueText.text = "";

        for (int i = 0; i < beat.dialogueText.Length; i++)
        {
            dialogueText.text += beat.dialogueText[i];

            // Play a quiet typing sound every 3rd character so it doesn't spam the audio
            if (typeSound != null && sfxSource != null && i % 3 == 0)
            {
                sfxSource.PlayOneShot(typeSound, 0.15f);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        ShowChoices(beat);
    }

    // Loops through the beat's choices and turns on buttons whose GameState conditions pass
    void ShowChoices(StoryBeat beat)
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (beat.choices != null && i < beat.choices.Length && CheckCondition(beat.choices[i].showCondition))
            {
                ChoiceOption option = beat.choices[i];

                choiceButtons[i].gameObject.SetActive(true);
                choiceButtonLabels[i].text = option.buttonLabel;

                // Clear old button listeners before adding the new choice callback
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(delegate { OnChoiceClicked(option); });
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // Checks static flags in GameState to see if a choice should be unlocked
    bool CheckCondition(ConditionType condition)
    {
        if (condition == ConditionType.RequiresDogBone)
        {
            return GameState.hasDogBone;
        }
        if (condition == ConditionType.RequiresDog)
        {
            return GameState.hasDog;
        }
        if (condition == ConditionType.RequiresNoDog)
        {
            return !GameState.hasDog;
        }
        if (condition == ConditionType.RequiresNotVisitedSmoker)
        {
            return !GameState.visitedSmoker;
        }

        return true;
    }

    // Applies the selected choice's GameState effect, then loads the next scene or dialogue beat
    void OnChoiceClicked(ChoiceOption option)
    {
        if (clickSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clickSound);
        }

        // Update GameState based on the choice's effect
        if (option.effect == StateEffect.TakeOneStrike)
        {
            TriggerCameraShake();

            bool died = GameState.TakeStrike();
            if (died)
            {
                return; // Stop here since TakeStrike already loaded Scene_Death
            }
        }
        else if (option.effect == StateEffect.GiveDogBone)
        {
            GameState.hasDogBone = true;
        }
        else if (option.effect == StateEffect.AdoptDog)
        {
            GameState.hasDog = true;
        }
        else if (option.effect == StateEffect.MarkSmokerVisited)
        {
            GameState.visitedSmoker = true;
        }
        else if (option.effect == StateEffect.InstantDeath)
        {
            SceneManager.LoadScene("Scene_Death");
            return;
        }
        else if (option.effect == StateEffect.ResetRun)
        {
            GameState.ResetRun();
        }

        // Go to a brand new scene if a scene name was set, otherwise advance within this scene
        if (option.targetSceneName != "")
        {
            SceneManager.LoadScene(option.targetSceneName);
        }
        else if (option.nextBeatIndex >= 0)
        {
            ShowBeat(option.nextBeatIndex);
        }
    }

    // Finds the active Camera and triggers its CameraShake component
    void TriggerCameraShake()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            cam = FindFirstObjectByType<Camera>();
        }

        if (cam != null)
        {
            CameraShake shake = cam.GetComponent<CameraShake>();
            if (shake == null)
            {
                shake = cam.gameObject.AddComponent<CameraShake>();
            }
            shake.StartShake();
        }
    }
}
