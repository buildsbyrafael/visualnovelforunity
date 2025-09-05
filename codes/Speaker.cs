using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSpeaker", menuName = "Data/New Speaker")]
[System.Serializable]
public class Speaker : ScriptableObject
{
    public string speakerName;
    public Color textColor;
    public Color nameColor;

    [Header("Sprite Padrão")]
    public Sprite defaultSprite;

    [System.Serializable]
    public class SpriteVariant
    {
        public string id;
        public Sprite sprite;
    }

    [Header("Variações Visuais")]
    public List<SpriteVariant> variants = new List<SpriteVariant>();

    public Sprite GetVariant(string variantId)
    {
        foreach (var variant in variants)
            if (variant.id == variantId)
                return variant.sprite;

        return defaultSprite;
    }
}