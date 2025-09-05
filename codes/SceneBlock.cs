using System;
using UnityEngine;

[Serializable]
public class SceneBlock
{
    public string sceneId;
    public string background;
    public string conditionDecisionId;
    public string conditionSelectionId;
    public string rawText;

    public bool IsConditionMet()
    {
        if (string.IsNullOrEmpty(conditionDecisionId) || string.IsNullOrEmpty(conditionSelectionId))
            return true;
        return ChoiceManager.Instance.IsDecision(conditionDecisionId, conditionSelectionId);
    }

    public string GetEffectiveBackground()
    {
        return !string.IsNullOrEmpty(background) ? background : null;
    }

    public string GetDisplayText()
    {
        return rawText;
    }
}