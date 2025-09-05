using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;

public class DL_DIALOGUE_DATA
{
    public List<DIALOGUE_SEGMENT> segments { get; private set; }

    private const string segmentIdentifierPattern = @"\{(wa|wc|a|c|sfx)(?:\s([^\}]*))?\}";
    
    private const string spriteCommandPattern = @"\{(sprite|hide_sprite)\s+([^\}]*)\}";

    public DL_DIALOGUE_DATA(string rawDialogue)
    {
        segments = RipSegments(rawDialogue);
    }

    private List<DIALOGUE_SEGMENT> RipSegments(string rawDialogue)
    {
        List<DIALOGUE_SEGMENT> result = new List<DIALOGUE_SEGMENT>();
        MatchCollection matches = Regex.Matches(rawDialogue, $"{segmentIdentifierPattern}|{spriteCommandPattern}", RegexOptions.IgnoreCase);

        int cursor = 0;

        foreach (Match match in matches)
        {
            if (match.Index > cursor)
            {
                result.Add(new DIALOGUE_SEGMENT
                {
                    dialogue = rawDialogue.Substring(cursor, match.Index - cursor),
                    startSignal = DIALOGUE_SEGMENT.StartSignal.NONE,
                    signalDelay = 0,
                    sfxName = null
                });
            }

            if (match.Groups[1].Success && !string.IsNullOrEmpty(match.Groups[1].Value))
            {
                string signalRaw = match.Groups[1].Value.ToUpper();
                string paramRaw = match.Groups[2].Value;

                if (signalRaw == "SFX")
                {
                    result.Add(new DIALOGUE_SEGMENT
                    {
                        dialogue = string.Empty,
                        startSignal = DIALOGUE_SEGMENT.StartSignal.SFX,
                        signalDelay = 0,
                        sfxName = string.IsNullOrEmpty(paramRaw) ? null : paramRaw.Trim()
                    });
                }
                else
                {
                    float delay = 0;
                    if (!string.IsNullOrEmpty(paramRaw))
                    {
                        paramRaw = paramRaw.Replace(',', '.');
                        float.TryParse(paramRaw, NumberStyles.Float, CultureInfo.InvariantCulture, out delay);
                    }

                    Enum.TryParse(signalRaw, out DIALOGUE_SEGMENT.StartSignal signal);

                    result.Add(new DIALOGUE_SEGMENT
                    {
                        dialogue = string.Empty,
                        startSignal = signal,
                        signalDelay = delay,
                        sfxName = null
                    });
                }
            }
            else if (match.Groups[3].Success)
            {
                string command = match.Groups[3].Value.Trim().ToLower();
                string parameters = match.Groups[4].Value.Trim();

                var parsed = ParseSpriteCommand(parameters);

                DIALOGUE_SEGMENT seg = new DIALOGUE_SEGMENT
                {
                    dialogue = string.Empty,
                    startSignal = command == "sprite" ? DIALOGUE_SEGMENT.StartSignal.SPRITE : DIALOGUE_SEGMENT.StartSignal.HIDE_SPRITE,
                    signalDelay = 0,
                    sfxName = null,
                    spriteName = parsed.GetValueOrDefault("name"),
                    variantId = parsed.GetValueOrDefault("variant"),
                    flip = parsed.ContainsKey("flip") && parsed["flip"].ToLower() == "true",
                    fade = parsed.ContainsKey("fade") && parsed["fade"].ToLower() == "true",
                    scale = parsed.ContainsKey("scale") && float.TryParse(parsed["scale"], NumberStyles.Float, CultureInfo.InvariantCulture, out float s) ? s : 1f,
                    pos = parsed.ContainsKey("pos") ? ParseVector2(parsed["pos"]) : new UnityEngine.Vector2(0.5f, 0f),
                    width = parsed.ContainsKey("width") && float.TryParse(parsed["width"], NumberStyles.Float, CultureInfo.InvariantCulture, out float w) ? w : 100f,
                    height = parsed.ContainsKey("height") && float.TryParse(parsed["height"], NumberStyles.Float, CultureInfo.InvariantCulture, out float h) ? h : 100f
                };

                result.Add(seg);
            }

            cursor = match.Index + match.Length;
        }

        if (cursor < rawDialogue.Length)
        {
            result.Add(new DIALOGUE_SEGMENT
            {
                dialogue = rawDialogue.Substring(cursor),
                startSignal = DIALOGUE_SEGMENT.StartSignal.NONE,
                signalDelay = 0,
                sfxName = null
            });
        }

        return result;
    }

    private Dictionary<string, string> ParseSpriteCommand(string raw)
    {
        var dict = new Dictionary<string, string>();
        var parts = Regex.Matches(raw, @"(\w+)\s*=\s*([^\s]+)");
        foreach (Match part in parts)
            dict[part.Groups[1].Value.ToLower()] = part.Groups[2].Value;
        return dict;
    }

    private UnityEngine.Vector2 ParseVector2(string raw)
    {
        var trimmed = raw.Trim('(', ')');
        var split = trimmed.Split(',');
        if (split.Length != 2) return new UnityEngine.Vector2(0.5f, 0f);

        float.TryParse(split[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x);
        float.TryParse(split[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y);
        return new UnityEngine.Vector2(x, y);
    }

    public struct DIALOGUE_SEGMENT
    {
        public string dialogue;
        public StartSignal startSignal;
        public float signalDelay;
        public string sfxName;

        public string spriteName;
        public string variantId;
        public UnityEngine.Vector2 pos;
        public float scale;
        public bool flip;
        public bool fade;
        public float width;
        public float height;

        public enum StartSignal { NONE, C, A, WA, WC, SFX, SPRITE, HIDE_SPRITE }

        public bool appendText => startSignal == StartSignal.A || startSignal == StartSignal.WA;
    }
}