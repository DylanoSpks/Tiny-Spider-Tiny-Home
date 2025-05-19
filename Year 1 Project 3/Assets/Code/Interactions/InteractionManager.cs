// InteractionManager.cs
// Handles raycasting from the center of the screen,
// shows/hides a custom UI panel with a key icon and action text,
// and invokes interaction when the player presses the configured key.
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour {
    //Dylano changes
    [SerializeField] private Slider _boltSlider;
    
    
    [Header("References")]
    [Tooltip("Camera used to perform raycasts")]   
    [SerializeField] private Camera playerCamera;

    [Tooltip("Parent GameObject for the prompt UI (small key icon + text)")]
    [SerializeField] private GameObject promptUI;
    [Tooltip("Text field for the key icon (e.g. 'E')")]
    [SerializeField] private TextMeshProUGUI keyText;
    [Tooltip("Text field for the action description (e.g. 'Interact')")]
    [SerializeField] private TextMeshProUGUI actionText;

    [Header("Settings")]
    [Tooltip("Max distance for detecting interactables")]  
    [SerializeField] private float interactDistance = 3f;
    [Tooltip("Key to press for interaction")]  
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private IInteractable _currentInteractable;

    private void Start() {
        if (promptUI == null || playerCamera == null) {
            Debug.LogError("InteractionManager: Assign all references in the inspector!");
            enabled = false;
            return;
        }

        // Hide the UI at the start
        promptUI.SetActive(false);

        // Set the key icon text dynamically
        keyText.text = interactKey.ToString();
    }
    
    private void Update() {
        CheckForInteractable();

        if (_currentInteractable != null && Input.GetKeyDown(interactKey)) {
            _currentInteractable.OnInteract();
            promptUI.SetActive(false);
            _currentInteractable = null;

            _boltSlider.value -= 10;
        }
    }

    private void CheckForInteractable() {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance)) {
            // Directly look for InvisibilityInteractable (or any IInteractable implementer)
            var invis = hit.collider.GetComponent<InvisibilityInteractable>();
            if (invis != null) {
                _currentInteractable = invis;
                promptUI.SetActive(true);
                return;
            }
        }

        // Nothing valid hit: hide UI
        if (_currentInteractable != null) {
            _currentInteractable = null;
            promptUI.SetActive(false);
        }
    }
}