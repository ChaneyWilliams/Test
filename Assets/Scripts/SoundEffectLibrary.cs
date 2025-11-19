using System;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectLibrary : MonoBehaviour
{
    [SerializeField] private SoundEffectGroup[] soundEffectGroups;

    private Dictionary<string, List<AudioClip>> soundDictionary;
    public static SoundEffectLibrary instance;

    private void Awake()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        soundDictionary = new Dictionary<string, List<AudioClip>>();
        foreach(SoundEffectGroup soundEffectGroup in soundEffectGroups)
        {
            soundDictionary[soundEffectGroup.name] = soundEffectGroup.audioClip;
        }
    }
    public AudioClip GetClip(string name, int index)
    {
        if (soundDictionary.ContainsKey(name))
        {
            
            List<AudioClip> audioClips = soundDictionary[name];
            if (audioClips.Count > 0)
            {
                return audioClips[index];
            }

        }
        return null;
    }
    [System.Serializable]
    public struct SoundEffectGroup
    {
        public string name;
        public List<AudioClip> audioClip;
    }
}
