using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class StorySceneController : MonoBehaviour
{
    public enum ConditionType
    {
        None,
        RequiresDogBone,
        RequiresDog,
        RequiresNoDog,
        RequiresNotVisitedSmoker
    }

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

    [System.Serializable]
    public class ChoiceOption
    {
        public string buttonLabel;
        public ConditionType showCondition = ConditionType.None;
        public StateEffect effect = StateEffect.None;
        public string targetSceneName;
        public int nextBeatIndex = -1;
    }

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

    private Coroutine typingCoroutine;

    void Awake()
    {
        // Automatically ensure Main Camera has CameraShake attached
        if (Camera.main != null && Camera.main.GetComponent<CameraShake>() == null)
        {
            Camera.main.gameObject.AddComponent<CameraShake>();
        }
    }

    void Start()
    {
        if (beats != null && beats.Length > 0)
        {
            ShowBeat(0);
        }
    }

    public void ShowBeat(int index)
    {
        if (index < 0 || index >= beats.Length)
        {
            return;
        }

        StoryBeat currentBeat = beats[index];

        if (speakerText != null)
        {
            speakerText.text = currentBeat.speakerName;
        }

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

        if (sfxSource != null && currentBeat.beatSFX != null)
        {
            sfxSource.PlayOneShot(currentBeat.beatSFX);
        }

        if (currentBeat.shakeOnStart)
        {
            TriggerCameraShake();
        }

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] != null)
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeDialogue(currentBeat));
    }

    IEnumerator TypeDialogue(StoryBeat beat)
    {
        dialogueText.text = "";

        for (int i = 0; i < beat.dialogueText.Length; i++)
        {
            dialogueText.text += beat.dialogueText[i];

            if (typeSound != null && sfxSource != null && i % 3 == 0)
            {
                sfxSource.PlayOneShot(typeSound, 0.15f);
            }

            yield return new WaitForSeconds(typingSpeed);
        }

        ShowChoices(beat);
    }

    void ShowChoices(StoryBeat beat)
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (beat.choices != null && i < beat.choices.Length && CheckCondition(beat.choices[i].showCondition))
            {
                ChoiceOption option = beat.choices[i];

                choiceButtons[i].gameObject.SetActive(true);
                choiceButtonLabels[i].text = option.buttonLabel;

                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(delegate { OnChoiceClicked(option); });
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

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

    void OnChoiceClicked(ChoiceOption option)
    {
        if (clickSound != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clickSound);
        }

        if (option.effect == StateEffect.TakeOneStrike)
        {
            TriggerCameraShake();

            bool died = GameState.TakeStrike();
            if (died)
            {
                return;
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

        if (option.targetSceneName != "")
        {
            SceneManager.LoadScene(option.targetSceneName);
        }
        else if (option.nextBeatIndex >= 0)
        {
            ShowBeat(option.nextBeatIndex);
        }
    }

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