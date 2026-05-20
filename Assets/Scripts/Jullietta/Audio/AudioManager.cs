using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    public List<AudioClip> musicClips;        // список треков
    [Range(0f, 1f)] public float musicVolume = 0.001f;
    private AudioSource musicSource;
    private int currentTrackIndex = 0;

    [Header("SFX")]
    [Range(0f, 1f)] public float sfxVolume = 0.7f;
    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = GetComponent<AudioSource>();
        sfxSource = GetComponents<AudioSource>()[1];
        
        // Загружаем сохранённые настройки
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.05f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
        ApplyVolumes();
        
        // Начинаем воспроизведение музыки
        PlayRandomMusic();
    }
    private void ApplyVolumes()
    {
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }
    public void SetMusicVolume(float vol)
    {
        musicVolume = vol;
        musicSource.volume = vol;
        PlayerPrefs.SetFloat("MusicVolume", vol);
    }
    public void SetSFXVolume(float vol)
    {
        sfxVolume = vol;
        sfxSource.volume = vol;
    }
    // Воспроизведение случайного трека
    private void PlayRandomMusic()
    {
        if (musicClips.Count == 0) return;
        currentTrackIndex = Random.Range(0, musicClips.Count);
        musicSource.clip = musicClips[currentTrackIndex];
        musicSource.Play();
        // Запланировать следующий трек
        Invoke(nameof(PlayNextMusic), musicSource.clip.length);
    }
    private void PlayNextMusic()
    {
        if (musicClips.Count == 0) return;
        currentTrackIndex = (currentTrackIndex + 1) % musicClips.Count;
        musicSource.clip = musicClips[currentTrackIndex];
        musicSource.Play();
        Invoke(nameof(PlayNextMusic), musicSource.clip.length);
    }
    // Звуки интерфейса (кнопки и пр.)
    public void PlayButtonClick(AudioClip clip)
    {
        if (clip != null) sfxSource?.PlayOneShot(clip, sfxVolume);
    }
    // Звук столкновения с собираемым объектом
    public void PlayCollectSound(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip, sfxVolume);
    }
    // Звук победы/поражения
    public void PlayVictorySound(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip, sfxVolume);
    }
    public void PlayDefeatSound(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip, sfxVolume);
    }
}