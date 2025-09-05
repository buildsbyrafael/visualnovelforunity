using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BottomBarController : MonoBehaviour
{
    public TextMeshProUGUI barText;
    public TextMeshProUGUI personNameText;
    public Image backgroundImage;
    public ScreenFader screenFader;
    public TextMeshProUGUI notificationText;
    public CharacterLayerManager characterLayerManager;

    public Button SkipButton;

    private int sentenceIndex = -1;
    private List<DialogueReader.ParsedSentence> sentences;
    private TextArchitect architect;
    private TextArchitect nameArchitect;
    private Coroutine dialogueRoutine;
    private bool isTransitioning = false;
    private bool sceneFinished = false;
    private bool inputLocked = false;
    private bool isLastSentence = false;
    private bool waitingForFinalClick = false;

    private void Awake()
    {
        if (barText != null) barText.text = string.Empty;
        if (personNameText != null) personNameText.text = string.Empty;
        if (notificationText != null) notificationText.gameObject.SetActive(false);
        if (barText != null) architect = new TextArchitect(barText);
        if (personNameText != null) nameArchitect = new TextArchitect(personNameText);
        if (SkipButton != null) SkipButton.onClick.AddListener(OnSkipButtonClicked);
    }

    private void Update()
    {
        if (isTransitioning || sentences == null || architect == null || nameArchitect == null || inputLocked)
            return;

        if (StoryGamePlayer.IsInputLocked)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool building = architect.isBuilding || nameArchitect.isBuilding;
            if (building) return;

            if (waitingForFinalClick)
            {
                waitingForFinalClick = false;
                sceneFinished = true;
                return;
            }

            if (sceneFinished) return;

            DisplayNextSentence();
        }
    }

    private void OnSkipButtonClicked()
    {
        if (architect == null || nameArchitect == null || inputLocked)
            return;

        bool isBuilding = architect.isBuilding || nameArchitect.isBuilding;

        if (isBuilding)
        {
            architect.ForceComplete();
            nameArchitect.ForceComplete();
        }
        else if (!sceneFinished && !isTransitioning)
        {
            DisplayNextSentence();
        }
    }

    private void DisplayNextSentence()
    {
        if (sceneFinished || isTransitioning) return;

        inputLocked = true;
        StartCoroutine(UnlockInputNextFrame());

        sentenceIndex++;

        if (sentenceIndex < sentences.Count)
        {
            var sentence = sentences[sentenceIndex];

            if (sentence.text.StartsWith("{choice ") && sentence.text.EndsWith("}"))
            {
                string decisionId = sentence.text.Substring(8, sentence.text.Length - 9).Trim();
                HandleChoiceCommand(decisionId);
                return;
            }

            string currentSpeaker = sentence.speakerName;

            if (dialogueRoutine != null)
                StopCoroutine(dialogueRoutine);

            isLastSentence = sentenceIndex == sentences.Count - 1;
            dialogueRoutine = StartCoroutine(HandleDialogue(sentence.text, currentSpeaker));
        }
        else
        {
            sceneFinished = true;
        }
    }

    private void HandleChoiceCommand(string decisionId)
    {
        gameObject.SetActive(false);

        var player = FindFirstObjectByType<StoryCapPlayer>();
        if (player == null) return;

        ChoiceUIController.Instance.ShowChoice(decisionId, player, () =>
        {
            gameObject.SetActive(true);
            DisplayNextSentence();
        });
    }

    private IEnumerator UnlockInputNextFrame()
    {
        yield return new WaitForEndOfFrame();
        inputLocked = false;
    }

    private IEnumerator HandleDialogue(string rawText, string currentSpeaker)
    {
        if (architect == null) yield break;

        DL_DIALOGUE_DATA parsed = new DL_DIALOGUE_DATA(rawText);

        if (barText != null)
            barText.color = new Color(1f, 1f, 1f, 1f);
        if (personNameText != null)
            personNameText.color = new Color(1f, 1f, 1f, 1f);

        bool isOnlySFX = parsed.segments.TrueForAll(seg =>
            seg.startSignal == DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.SFX ||
            seg.startSignal == DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.SPRITE ||
            seg.startSignal == DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.HIDE_SPRITE
        );

        if (nameArchitect != null && personNameText != null && !isOnlySFX)
        {
            personNameText.text = string.Empty;

            if (!string.IsNullOrEmpty(currentSpeaker) && currentSpeaker != "None")
            {
                nameArchitect.buildMethod = architect.buildMethod;
                nameArchitect.Build(currentSpeaker);
            }
        }

        foreach (var segment in parsed.segments)
        {
            if (segment.startSignal == DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.SFX)
            {
                barText.text = string.Empty;
                personNameText.text = string.Empty;

                if (!string.IsNullOrEmpty(segment.sfxName))
                {
                    var player = FindFirstObjectByType<StoryCapPlayer>();
                    if (player != null)
                    {
                        isTransitioning = true;
                        yield return player.PlaySFXAndWait(segment.sfxName);
                        isTransitioning = false;
                    }

                    yield return new WaitForSeconds(0.1f);
                    DisplayNextSentence();
                    yield break;
                }

                continue;
            }

            if (segment.startSignal == DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.SPRITE)
            {
                var speaker = SpeakerDatabase.GetByName(segment.spriteName);
                if (speaker != null && characterLayerManager != null)
                {
                    characterLayerManager.ShowCharacter(
                        speaker,
                        segment.variantId,
                        segment.pos,
                        segment.width,
                        segment.height,
                        segment.flip,
                        segment.fade
                    );
                }

                yield return new WaitForSeconds(0.1f);
                DisplayNextSentence();
                yield break;
            }

            if (segment.startSignal == DL_DIALOGUE_DATA.DIALOGUE_SEGMENT.StartSignal.HIDE_SPRITE)
            {
                if (characterLayerManager != null)
                {
                    characterLayerManager.HideCharacter(segment.spriteName, segment.fade);
                }

                yield return new WaitForSeconds(0.1f);
                DisplayNextSentence();
                yield break;
            }

            if (segment.signalDelay > 0)
                yield return new WaitForSeconds(segment.signalDelay);

            if (segment.appendText)
                architect.Append(segment.dialogue);
            else
                architect.Build(segment.dialogue);

            while (architect.isBuilding || nameArchitect.isBuilding)
                yield return null;
        }

        if (isLastSentence)
            waitingForFinalClick = true;
    }

    public void ClearText()
    {
        if (dialogueRoutine != null)
        {
            StopCoroutine(dialogueRoutine);
            dialogueRoutine = null;
        }

        architect?.ForceComplete();
        nameArchitect?.ForceComplete();
        architect?.Stop();
        nameArchitect?.Stop();

        if (barText != null) barText.text = string.Empty;
        if (personNameText != null) personNameText.text = string.Empty;

        sceneFinished = true;
        inputLocked = false;
    }

    public void Play(List<DialogueReader.ParsedSentence> parsedSentences)
    {
        ClearText();
        sentences = parsedSentences;
        sentenceIndex = -1;
        sceneFinished = false;
        isLastSentence = false;
        waitingForFinalClick = false;
        inputLocked = true;
        StartCoroutine(UnlockInputNextFrame());
        DisplayNextSentence();
    }

    public bool IsRunningScene() => !sceneFinished;

    public void ShowNotification(string message, float duration = 2f)
    {
        if (notificationText != null)
        {
            StopCoroutine("ShowNotificationRoutine");
            StartCoroutine(ShowNotificationRoutine(message, duration));
        }
    }

    private IEnumerator ShowNotificationRoutine(string message, float duration)
    {
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        notificationText.gameObject.SetActive(false);
    }

    public void FadeOutTextInstantly()
    {
        if (barText != null)
            barText.color = new Color(barText.color.r, barText.color.g, barText.color.b, 0f);
            
        if (personNameText != null)
            personNameText.color = new Color(personNameText.color.r, personNameText.color.g, personNameText.color.b, 0f);
    }
}
