using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }

    private const string MusicKey = "MusicOn";
    private const string VibrationKey = "VibrationOn";
    private const string SoundKey = "SoundOn";
    private const string BestScoreKey = "BestScore";

    public bool MusicOn
    {
        get => PlayerPrefs.GetInt(MusicKey, 1) == 1;
        set => PlayerPrefs.SetInt(MusicKey, value ? 1 : 0);
    }

    public bool VibrationOn
    {
        get => PlayerPrefs.GetInt(VibrationKey, 1) == 1;
        set => PlayerPrefs.SetInt(VibrationKey, value ? 1 : 0);
    }

    public bool SoundOn
    {
        get => PlayerPrefs.GetInt(SoundKey, 1) == 1;
        set => PlayerPrefs.SetInt(SoundKey, value ? 1 : 0);
    }

    public int BestScore
    {
        get => PlayerPrefs.GetInt(BestScoreKey, 0);
        set => PlayerPrefs.SetInt(BestScoreKey, value);
    }

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
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }

}
