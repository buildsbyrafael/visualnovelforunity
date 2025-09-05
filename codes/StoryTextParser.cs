using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class StoryTextParser
{
    public static List<SceneBlock> Parse(TextAsset textAsset)
    {
        if (textAsset == null)
            return new List<SceneBlock>();

        return Parse(textAsset.text);
    }

    public static List<SceneBlock> Parse(string rawText)
    {
        var blocks = new List<SceneBlock>();
        if (string.IsNullOrEmpty(rawText))
            return blocks;

        var lines = rawText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        SceneBlock current = null;

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();

            if (line.StartsWith("{scene"))
            {
                if (current != null)
                    blocks.Add(current);

                current = new SceneBlock
                {
                    sceneId = null,
                    background = null,
                    conditionDecisionId = null,
                    conditionSelectionId = null,
                    rawText = ""
                };

                var header = line.Trim('{', '}').Trim();
                var match = Regex.Match(header, @"^scene\s+([^\s\[]+)(.*)$");
                if (match.Success)
                {
                    current.sceneId = match.Groups[1].Value;

                    string attributes = match.Groups[2].Value;
                    var attrMatches = Regex.Matches(attributes, @"\[(.*?)\]");

                    foreach (Match attrMatch in attrMatches)
                    {
                        var attr = attrMatch.Groups[1].Value;

                        if (attr.StartsWith("bg="))
                        {
                            current.background = attr.Substring(3).Trim();
                        }
                        else if (attr.StartsWith("if="))
                        {
                            var parts = attr.Substring(3).Split('=');
                            if (parts.Length == 2)
                            {
                                current.conditionDecisionId = parts[0].Trim();
                                current.conditionSelectionId = parts[1].Trim();
                            }
                        }
                    }
                }
            }
            else if (current != null)
            {
                current.rawText += line + "\n";
            }
        }

        if (current != null)
            blocks.Add(current);

        return blocks;
    }
}