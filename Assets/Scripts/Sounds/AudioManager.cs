using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    [SerializeField] private GameObject soundPerfab = null;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource ambientSoundAudioSource = null;
    [SerializeField] private AudioSource gameMusicAudioSource = null;

    [Header("Audio Mixers")]
    [SerializeField] private AudioMixer gameAudioMixer = null;

    [Header("Audio Snapshots")]
    [SerializeField] private AudioMixerSnapshot gameMusicSnapshot = null;
    [SerializeField] private AudioMixerSnapshot gameAmbientSnapshot = null;

    [Header("Others")]
    [SerializeField] private SO_SoundList sO_SoundList = null;

    [SerializeField] private SO_SceneSoundList sO_SceneSoundList = null;
    [SerializeField] private float defaultSceneMusicPlayTimeSeconds = 120f;
    [SerializeField] private float sceneMusicStartMinSecs = 20f;
    [SerializeField] private float sceneMusicStartMaxSecs = 40f;
    [SerializeField] private float musicTransitionSecs = 8f;

    private Dictionary<SoundName, SoundItem> soundDictionary;
    private Dictionary<SceneName, SceneSoundItem> sceneSoundDictionary;

    private Coroutine playSceneSoundCoroutine;

    protected override void Awake()
    {
        base.Awake();

        soundDictionary = new Dictionary<SoundName, SoundItem>();

        sceneSoundDictionary = new Dictionary<SceneName, SceneSoundItem>();

        foreach(SoundItem soundItem in sO_SoundList.soundDetails)
        {
            soundDictionary.Add(soundItem.soundName, soundItem);
        }

        foreach(SceneSoundItem sceneSoundItem in sO_SceneSoundList.sceneSoundDetails)
        {
            sceneSoundDictionary.Add(sceneSoundItem.sceneName, sceneSoundItem);
        }
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += PlaySceneSound;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= PlaySceneSound;
    }

    private void PlaySceneSound ()
    {
        SoundItem musicSoundItem = null;
        SoundItem ambientSoundItem = null;

        float musicPlayTime = defaultSceneMusicPlayTimeSeconds;

        if (Enum.TryParse<SceneName>(SceneManager.GetActiveScene().name, out SceneName sceneName))
        {
          if (sceneSoundDictionary.TryGetValue(sceneName, out SceneSoundItem sceneSoundItem))
            {
                soundDictionary.TryGetValue(sceneSoundItem.musicForScene, out musicSoundItem);
                soundDictionary.TryGetValue(sceneSoundItem.ambientSoundForScene, out ambientSoundItem);
            } else
            {
                return;
            }

          if (playSceneSoundCoroutine != null)
            {
                StopCoroutine(playSceneSoundCoroutine);
            }
            playSceneSoundCoroutine = StartCoroutine(PlaySceneSoundRoutine(musicPlayTime, musicSoundItem, ambientSoundItem));
        }
    }

    private IEnumerator PlaySceneSoundRoutine(float playTime, SoundItem musicSoundItem, SoundItem ambientSoundItem)
    {
        if (musicSoundItem != null && ambientSoundItem != null) {
            PlayAmbientSoundClip(ambientSoundItem, 0f);

            yield return new WaitForSeconds(UnityEngine.Random.Range(sceneMusicStartMinSecs, sceneMusicStartMaxSecs));

            PlayMusicSoundClip(musicSoundItem, musicTransitionSecs);

            yield return new WaitForSeconds(playTime);

            PlayAmbientSoundClip(ambientSoundItem, musicTransitionSecs);

        }

    }

    private void PlayAmbientSoundClip(SoundItem ambientSoundItem, float transitionTimeSeconds)
    {
        gameAudioMixer.SetFloat("AmbientVolume", ConverSoundVolumnDecimalFractionToDecibels(ambientSoundItem.soundVolumn));

        ambientSoundAudioSource.clip = ambientSoundItem.soundClip;
        ambientSoundAudioSource.Play();

        gameAmbientSnapshot.TransitionTo(transitionTimeSeconds);
    }

    private float ConverSoundVolumnDecimalFractionToDecibels(float soundVolumn)
    {
        return (soundVolumn * 100f - 80f);
    }

    private void PlayMusicSoundClip(SoundItem musicSoundItem, float transitionTimeSeconds)
    {
        gameAudioMixer.SetFloat("MusicVolume", ConverSoundVolumnDecimalFractionToDecibels(musicSoundItem.soundVolumn));

        gameMusicAudioSource.clip = musicSoundItem.soundClip;
        gameMusicAudioSource.Play();

        gameMusicSnapshot.TransitionTo(transitionTimeSeconds);
    }

    public void PlaySound(SoundName soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out SoundItem soundItem) && soundPerfab != null)
        {
            GameObject soundGameObject = PoolManager.Instance.ReuseObject(soundPerfab, Vector3.zero, Quaternion.identity);

            Sound sound = soundGameObject.GetComponent<Sound>();
            sound.SetSound(soundItem);

            soundGameObject.SetActive(true);

            StartCoroutine(DisableSound(soundGameObject, soundItem.soundClip.length));
        }
    }

    private IEnumerator DisableSound(GameObject soudGameObject, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);

        soudGameObject.SetActive(false);
    }
}
