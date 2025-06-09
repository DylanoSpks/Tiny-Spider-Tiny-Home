using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VolumeApplier : MonoBehaviour
{
    private const string VolumePrefKey = "MusicVolume";

    void Awake()
    {
        float vol = PlayerPrefs.GetFloat(VolumePrefKey, 1f);
        GetComponent<AudioSource>().volume = vol;
    }
}