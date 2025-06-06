using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeController : MonoBehaviour
{
    [Header("Drag‐in the AudioSource that plays your background music")]
    public AudioSource musicSource;

    [Header("Drag‐in the Slider from OptionsPanel")]
    public Slider volumeSlider;

    private const string VolumePrefKey = "MusicVolume";

    void Start()
    {
        // 1) Read saved volume (default = 1)
        float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, 1f);

        // 2) Apply it to the AudioSource
        if (musicSource != null)
            musicSource.volume = savedVolume;

        // 3) Initialize the slider’s value so it matches
        if (volumeSlider != null)
            volumeSlider.value = savedVolume;

        // 4) Hook up the slider event
        if (volumeSlider != null)
            volumeSlider.onValueChanged.AddListener(OnVolumeSliderChanged);
    }

    // Called whenever the slider is moved
    public void OnVolumeSliderChanged(float newValue)
    {
        if (musicSource != null)
            musicSource.volume = newValue;

        PlayerPrefs.SetFloat(VolumePrefKey, newValue);
        PlayerPrefs.Save();
    }

    void OnDestroy()
    {
        // Clean up listener to avoid leaks
        if (volumeSlider != null)
            volumeSlider.onValueChanged.RemoveListener(OnVolumeSliderChanged);
    }
}
