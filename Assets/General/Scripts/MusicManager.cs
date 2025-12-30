using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField, Range(0f, 1f)] private float volume = 0.5f;
    [SerializeField] private bool loop = true;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = musicClip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        TryPlayMusic();
    }

    public void TryPlayMusic()
    {
        if (PlayerData.Instance == null)
            return;

        if (!PlayerData.Instance.MusicOn)
            return;

        if (musicClip != null)
            audioSource.Play();
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    private void LateUpdate()
    {
        RefreshMusicState();
    }

    public void RefreshMusicState()
    {
        if (PlayerData.Instance != null && PlayerData.Instance.MusicOn)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }

    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
    }
}
