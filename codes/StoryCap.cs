using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStoryCap", menuName = "Data/New Story Cap")]

public class StoryCap : ScriptableObject
{
    public TextAsset chapterTextAsset;
    public List<AudioClip> backgroundMusicClips;
    public float musicVolume = 1f;

    [System.Serializable]
    public class SoundEffect
    {
        public string effectName;
        public AudioClip effectClip;
        public float volume = 1f;
    }

    public List<SoundEffect> soundEffects;
}