// InvisibilityInteractable.cs
// Implements IInteractable to swap between two GameObjects (e.g., heather on/off) when interacted.
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InvisibilityInteractable : MonoBehaviour, IInteractable
{
    [Tooltip("The GameObject that is currently active (e.g., Heather On)")]
    [SerializeField] private GameObject onObject;

    [Tooltip("The GameObject to activate upon interaction (e.g., Heather Off)")]
    [SerializeField] private GameObject offObject;

    private void Reset()
    {
        // If no target specified, default onObject to this GameObject
        if (onObject == null)
            onObject = gameObject;
    }

    public void OnInteract()
    {
        // Disable the 'on' object and enable the 'off' object
        if (onObject != null)
            onObject.SetActive(false);

        if (offObject != null)
        {
            offObject.SetActive(true);
            // Optionally match transform of onObject
            offObject.transform.position = onObject.transform.position;
            offObject.transform.rotation = onObject.transform.rotation;
        }
    }
}