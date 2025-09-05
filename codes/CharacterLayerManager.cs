using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLayerManager : MonoBehaviour
{
    [SerializeField] private CharacterDisplay[] slots;
    public float defaultFadeDuration = 1f;

    private Dictionary<string, CharacterDisplay> activeCharacters = new();

    public void ShowCharacter(Speaker speaker, string variantId, Vector2 pos, float width, float height, bool flip = false, bool fade = false)
    {
        string name = speaker.speakerName;
        Sprite sprite = speaker.GetVariant(variantId);

        if (sprite == null) return;

        if (activeCharacters.ContainsKey(name))
        {
            var slot = activeCharacters[name];
            UpdateSlot(slot, name, sprite, pos, width, height, flip, fade);
            return;
        }

        foreach (var slot in slots)
        {
            if (!slot.gameObject.activeSelf)
                slot.gameObject.SetActive(true);

            if (slot.IsAvailable)
            {
                activeCharacters[name] = slot;
                UpdateSlot(slot, name, sprite, pos, width, height, flip, fade);
                return;
            }
        }
    }

    public void HideCharacter(string name, bool fade = false)
    {
        if (!activeCharacters.ContainsKey(name)) return;

        var slot = activeCharacters[name];
        StartCoroutine(HideSlot(slot, name, fade));
    }

    public void HideAllCharacters(bool fade = false)
    {
        foreach (var pair in activeCharacters)
        {
            var slot = pair.Value;
            StartCoroutine(HideSlot(slot, pair.Key, fade));
        }

        activeCharacters.Clear();
    }

    private void UpdateSlot(CharacterDisplay slot, string name, Sprite sprite, Vector2 pos, float width, float height, bool flip, bool fade)
    {
        slot.AssignName(name);
        slot.SetSprite(sprite);
        slot.SetPosition(pos);
        slot.SetSize(width, height);
        slot.SetFlip(flip);

        if (fade)
            StartCoroutine(slot.FadeIn(defaultFadeDuration));
        else
            slot.ShowInstant();
    }

    private IEnumerator HideSlot(CharacterDisplay slot, string name, bool fade)
    {
        if (fade)
            yield return slot.FadeOut(defaultFadeDuration);
        else
            slot.ClearSlot();

        slot.ClearSlot();
        activeCharacters.Remove(name);
    }
}