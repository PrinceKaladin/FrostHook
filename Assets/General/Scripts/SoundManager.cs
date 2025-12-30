using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Sources")]
    public AudioSource sfxSource;

    [Header("SFX")]
    public AudioClip buttonClick;

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

    }

    void PlaySFX(AudioClip clip)
    {
        if (!PlayerData.Instance.SoundOn || clip == null)
            return;

        sfxSource.PlayOneShot(clip);
    }

    public void PlayButton()
    {
        if(buttonClick != null)
            PlaySFX(buttonClick);
    }

}
