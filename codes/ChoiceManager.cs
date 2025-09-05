using System.Collections.Generic;
using UnityEngine;

public class ChoiceManager : MonoBehaviour
{
    public static ChoiceManager Instance { get; private set; }
    private Dictionary<string, string> decisions = new Dictionary<string, string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetDecision(string decisionId, string selectionId)
    {
        decisions[decisionId] = selectionId;
    }

    public string GetDecision(string decisionId)
    {
        decisions.TryGetValue(decisionId, out var result);
        return result;
    }

    public bool IsDecision(string decisionId, string selectionId)
    {
        return GetDecision(decisionId) == selectionId;
    }
}