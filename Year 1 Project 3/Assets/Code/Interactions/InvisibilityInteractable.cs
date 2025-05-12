// InvisibilityInteractable.cs
// Implements IInteractable to make a target object invisible when interacted.
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InvisibilityInteractable : MonoBehaviour, IInteractable {
    [Tooltip("The GameObject to make invisible upon interaction")]  
    [SerializeField] private GameObject targetObject;

    private void Reset() {
        // Auto-assign to self if no target specified
        if (targetObject == null) targetObject = gameObject;
    }

    public void OnInteract() {
        if (targetObject != null) {
            targetObject.SetActive(false);
        }
    }
}