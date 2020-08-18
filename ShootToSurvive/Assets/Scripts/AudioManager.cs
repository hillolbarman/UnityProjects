using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel { Master, SFX, Music };

    public float masterVolumePercent { get; private set; }
    public float sfxVolumePercent { get; private set; }
    public float musicVolumePercent { get; private set; }

    AudioSource sfx2DSource;
    AudioSource[] musicSources;
    int activeMusicSoucreIndex;

    public static AudioManager instance;

    Transform audioListener;
    Transform playerT;

    SoundLibrary library;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            musicSources = new AudioSource[2];
            for (int i = 0; i < musicSources.Length; i++)
            {
                GameObject newMusicSource = new GameObject("Music source " + (i + 1));
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }
            GameObject newSfx2DSource = new GameObject("2D SFX Source");
            sfx2DSource = newSfx2DSource.AddComponent<AudioSource>();
            DontDestroyOnLoad(newSfx2DSource);
            audioListener = FindObjectOfType<AudioListener>().transform;
            if(FindObjectOfType<Player>()!=null)
                playerT = FindObjectOfType<Player>().transform;
            library = GetComponent<SoundLibrary>();

            masterVolumePercent = PlayerPrefs.GetFloat("Master Volume", 1);
            musicVolumePercent = PlayerPrefs.GetFloat("Music Volume", 1);
            sfxVolumePercent = PlayerPrefs.GetFloat("SFX Volume", 1);
        }
    }

    void Start()
    {

    }

    void Update()
    {
        if (playerT != null)
        {
            audioListener.position = playerT.position;
        }
    }

    public void SetVolume(float newVolumePercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolumePercent = newVolumePercent;
                break;
            case AudioChannel.Music:
                musicVolumePercent = newVolumePercent;
                break;
            case AudioChannel.SFX:
                sfxVolumePercent = newVolumePercent;
                break;
        }
        musicSources[0].volume = musicVolumePercent * masterVolumePercent;
        musicSources[1].volume = musicVolumePercent * masterVolumePercent;

        PlayerPrefs.SetFloat("Master Volume", masterVolumePercent);
        PlayerPrefs.SetFloat("Music Volume", musicVolumePercent);
        PlayerPrefs.SetFloat("SFX Volume", sfxVolumePercent);
        PlayerPrefs.Save();
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        if (clip != null)
        {
            activeMusicSoucreIndex = 1 - activeMusicSoucreIndex;
            musicSources[activeMusicSoucreIndex].clip = clip;
            musicSources[activeMusicSoucreIndex].Play();
            StartCoroutine(AnimateMusicCrossFade(fadeDuration));
        }
    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        }
    }

    public void PlaySound(string soundName, Vector3 pos)
    {
        PlaySound(library.GetClipFromName(soundName), pos);
    }

    public void PlaySound2D(string name)
    {
        sfx2DSource.PlayOneShot(instance.library.GetClipFromName(name), sfxVolumePercent * masterVolumePercent);
    }

    IEnumerator AnimateMusicCrossFade(float duration)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSoucreIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
            musicSources[1 - activeMusicSoucreIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
            yield return null;
        }
    }
}
