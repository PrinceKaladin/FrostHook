using UnityEngine;
using UnityEngine.UI;

public class SoundToggleSlider : MonoBehaviour
{
    public enum SoundType
    {
        Music,
        Sound
    }

    [SerializeField] private SoundType soundType;
    [SerializeField] private Slider slider;

    private void Reset()
    {
        slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        UpdateSliderValue();
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    private void UpdateSliderValue()
    {
        bool isOn = soundType == SoundType.Music ? PlayerData.Instance.MusicOn : PlayerData.Instance.SoundOn;
        slider.value = isOn ? 1f : 0f;
    }

    private void OnSliderChanged(float value)
    {
        bool newValue = value > 0.5f;

        if (soundType == SoundType.Music)
            PlayerData.Instance.MusicOn = newValue;
        else
            PlayerData.Instance.SoundOn = newValue;

        slider.value = newValue ? 1f : 0f;
    }
}