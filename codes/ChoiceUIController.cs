using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceUIController : MonoBehaviour
{
    public static ChoiceUIController Instance { get; private set; }

    public GameObject choicePanel;
    public Button buttonA;
    public Button buttonB;
    public TextMeshProUGUI textA;
    public TextMeshProUGUI textB;

    private Action onChoiceComplete;
    private string currentDecisionId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (choicePanel != null)
            choicePanel.SetActive(false);
    }

    public void ShowChoice(string decisionId, StoryCapPlayer player, Action onComplete)
    {
        currentDecisionId = decisionId;
        onChoiceComplete = onComplete;

        List<SceneBlock> options = player.GetBlocksForDecision(decisionId);

        if (options == null || options.Count < 2)
        {
            return;
        }

        SceneBlock optionA = null;
        SceneBlock optionB = null;

        foreach (var block in options)
        {
            if (block.conditionSelectionId == "A" && optionA == null)
                optionA = block;
            else if (block.conditionSelectionId == "B" && optionB == null)
                optionB = block;
        }

        if (optionA == null || optionB == null)
        {
            return;
        }

        string textOptionA = "[Opção A não encontrada]";
        string textOptionB = "[Opção B não encontrada]";

        try
        {
            var parsedA = DialogueReader.ParseDialogue(optionA.rawText);
            if (parsedA.Count > 0)
                textOptionA = parsedA[0].text.Trim();

            var parsedB = DialogueReader.ParseDialogue(optionB.rawText);
            if (parsedB.Count > 0)
                textOptionB = parsedB[0].text.Trim();
        }
        catch (Exception)
        {
            
        }

        if (textA != null) textA.text = textOptionA;
        if (textB != null) textB.text = textOptionB;

        buttonA.onClick.RemoveAllListeners();
        buttonB.onClick.RemoveAllListeners();

        buttonA.onClick.AddListener(() => SelectOption("A"));
        buttonB.onClick.AddListener(() => SelectOption("B"));

        if (choicePanel != null)
            choicePanel.SetActive(true);
    }

    private void SelectOption(string optionId)
    {
        ChoiceManager.Instance.SetDecision(currentDecisionId, optionId);

        if (choicePanel != null)
            choicePanel.SetActive(false);

        onChoiceComplete?.Invoke();
    }
}