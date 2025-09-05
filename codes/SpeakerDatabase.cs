using UnityEngine;
using System.Collections.Generic;

public class SpeakerDatabase : MonoBehaviour
{
    public static SpeakerDatabase Instance { get; private set; }

    [Tooltip("Lista os Speakers Disponíveis")]
    public List<Speaker> speakers = new List<Speaker>();

    private Dictionary<string, Speaker> lookup = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (var speaker in speakers)
            {
                if (speaker != null && !string.IsNullOrEmpty(speaker.speakerName))
                    lookup[speaker.speakerName.ToLower()] = speaker;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static Speaker GetByName(string name)
    {
        if (Instance == null || string.IsNullOrEmpty(name)) return null;

        Instance.lookup.TryGetValue(name.ToLower(), out var speaker);
        return speaker;
    }
}