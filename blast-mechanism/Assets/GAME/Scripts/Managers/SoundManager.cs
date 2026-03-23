using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static Dictionary<string, AudioClip> soundDictionary;
    private static AudioSource audioSrc;
    void Awake()
    {
        audioSrc = gameObject.AddComponent<AudioSource>();
        soundDictionary = new Dictionary<string, AudioClip>();

        LoadAllSounds();
    }

    private void LoadAllSounds()
    {
        AudioClip[] allSounds = Resources.LoadAll<AudioClip>("");
        foreach (AudioClip clip in allSounds)
        {
            if (clip != null && !soundDictionary.ContainsKey(clip.name))
            {
                soundDictionary.Add(clip.name, clip);
            }
        } 
    }

    public static void PlaySound(string clipName)
    {
        if (PlayerPrefs.GetInt("SoundStatus") == 0 && soundDictionary.ContainsKey(clipName))
        {
            audioSrc.PlayOneShot(soundDictionary[clipName]);
        }
    }
}

