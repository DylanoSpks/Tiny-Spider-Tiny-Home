using System;
using UnityEngine;

/// <summary>
/// Attach this to an electric heater object with a trigger collider.
/// When the spider (tagged "Spider") enters the trigger, it gets launched upward.
/// </summary>
[RequireComponent(typeof(Collider))]
public class HeaterLaunch : MonoBehaviour
{
    [Header("Launch Settings")]
    [Tooltip("Upward impulse force applied to the spider")] public float launchForce = 10f;
    [Tooltip("Optional: Sound played when launching")] public AudioClip launchSound;

    private AudioSource _audioSource;

    void Awake()
    {
        // Ensure the collider is a trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        // Setup AudioSource if a sound is provided
        if (launchSound != null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.clip = launchSound;
            _audioSource.playOnAwake = false;
        }
    }

    [Obsolete("Obsolete")]
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Spider")) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            // Reset vertical velocity for consistent launch
            Vector3 velocity = rb.velocity;
            velocity.y = 0f;
            rb.velocity = velocity;

            // Apply upward impulse
            rb.AddForce(Vector3.up * launchForce, ForceMode.Impulse);

            // Play sound if available
            if (_audioSource != null)
                _audioSource.Play();
        }
    }
}