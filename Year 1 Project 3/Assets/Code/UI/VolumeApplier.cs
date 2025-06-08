using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VolumeApplier : MonoBehaviour
{
    private const string VolumePrefKey = "MusicVolume"; // same key your slider uses

    void Start()
    {
        float vol = PlayerPrefs.GetFloat(VolumePrefKey, 1f);
        GetComponent<AudioSource>().volume = vol;
    }
}