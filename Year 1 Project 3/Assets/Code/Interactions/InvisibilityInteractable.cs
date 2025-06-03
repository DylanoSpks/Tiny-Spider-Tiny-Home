// InvisibilityInteractable.cs
// Implements IInteractable to swap between two GameObjects (e.g., heather on/off) when interacted,
// and also plays an optional click sound.

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InvisibilityInteractable : MonoBehaviour, IInteractable
{
    [Tooltip("The GameObject that is currently active (e.g., Heather On)")]
    [SerializeField] private GameObject onObject;

    [Tooltip("The GameObject to activate upon interaction (e.g., Heather Off)")]
    [SerializeField] private GameObject offObject;

    [Header("Optional Sound")]
    [Tooltip("If provided, this AudioSource will play the click sound when interacted")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("If provided (and no AudioSource is set), this AudioClip will be played at this object's position")]
    [SerializeField] private AudioClip clickSound;

    private void Reset()
    {
        // If no onObject specified, default it to this GameObject
        if (onObject == null)
        {
            onObject = gameObject;
        }
    }

    public void OnInteract()
    {
        // 1) Play click sound (if any)
        PlayClickSound();

        // 2) Swap visibility of onObject/offObject
        if (onObject != null)
        {
            onObject.SetActive(false);
        }

        if (offObject != null)
        {
            offObject.SetActive(true);
            // Optionally match transform of onObject
            offObject.transform.position = onObject.transform.position;
            offObject.transform.rotation = onObject.transform.rotation;
        }
    }

    /// <summary>
    /// Plays the assigned click sound. 
    /// - If an AudioSource is assigned, uses AudioSource.PlayOneShot.
    /// - Otherwise, if only an AudioClip is assigned, uses PlayClipAtPoint.
    /// </summary>
    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            // Play through the assigned AudioSource
            audioSource.PlayOneShot(clickSound);
        }
        else if (audioSource == null && clickSound != null)
        {
            // No AudioSource provided → play the clip at this object's position
            AudioSource.PlayClipAtPoint(clickSound, transform.position);
        }
        // If neither audioSource nor clickSound assigned, do nothing.
    }
}
