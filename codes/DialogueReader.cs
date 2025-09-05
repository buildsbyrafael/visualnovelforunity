using System;
using System.Collections.Generic;
using UnityEngine;

public static class DialogueReader
{
    public struct ParsedSentence
    {
        public string text;
        public string speakerName;
    }

    public static List<ParsedSentence> ParseDialogue(TextAsset textFile)
    {
        var sentences = new List<ParsedSentence>();
        if (textFile == null) return sentences;

        return ParseDialogue(textFile.text);
    }

    public static List<ParsedSentence> ParseDialogue(string rawText)
    {
        var sentences = new List<ParsedSentence>();
        if (string.IsNullOrEmpty(rawText)) return sentences;

        var lines = rawText.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        string currentSpeakerName = null;

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            if (line.StartsWith("[") && line.EndsWith("]"))
            {
                var parsedName = line.Substring(1, line.Length - 2).Trim();
                currentSpeakerName = parsedName.Equals("None", StringComparison.OrdinalIgnoreCase) ? null : parsedName;
            }
            else
            {
                sentences.Add(new ParsedSentence { text = line, speakerName = currentSpeakerName });
            }
        }

        return sentences;
    }
}